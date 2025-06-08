using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysBot.Base;
using static System.Windows.Forms.Design.AxImporter;

namespace SysBot.Pokemon.WinForms.WebApi;

public class BotServer(Main mainForm, int port = 8080, int tcpPort = 8081) : IDisposable
{
    private HttpListener? _listener;
    private Thread? _listenerThread;
    private readonly int _port = port;
    private readonly int _tcpPort = tcpPort;
    private readonly CancellationTokenSource _cts = new();
    private readonly Main _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
    private volatile bool _running;
    private string? _htmlTemplate;

    private string HtmlTemplate
    {
        get
        {
            if (_htmlTemplate == null)
            {
                _htmlTemplate = LoadEmbeddedResource("BotControlPanel.html");
            }
            return _htmlTemplate;
        }
    }

    private static string LoadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(fullResourceName))
        {
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found. Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
        }

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Could not load embedded resource '{fullResourceName}'");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Start()
    {
        if (_running) return;

        try
        {
            _listener = new HttpListener();

            try
            {
                _listener.Prefixes.Add($"http://+:{_port}/");
                _listener.Start();
                LogUtil.LogInfo($"Web server listening on all interfaces at port {_port}", "WebServer");
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 5)
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{_port}/");
                _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
                _listener.Start();

                LogUtil.LogError($"Web server requires administrator privileges for network access. Currently limited to localhost only.", "WebServer");
                LogUtil.LogInfo("To enable network access, either:", "WebServer");
                LogUtil.LogInfo("1. Run this application as Administrator", "WebServer");
                LogUtil.LogInfo("2. Or run this command as admin: netsh http add urlacl url=http://+:8080/ user=Everyone", "WebServer");
            }

            _running = true;

            _listenerThread = new Thread(Listen)
            {
                IsBackground = true,
                Name = "BotWebServer"
            };
            _listenerThread.Start();
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to start web server: {ex.Message}", "WebServer");
            throw;
        }
    }

    public void Stop()
    {
        if (!_running) return;

        try
        {
            _running = false;
            _cts.Cancel();
            _listener?.Stop();
            _listenerThread?.Join(5000);
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Error stopping web server: {ex.Message}", "WebServer");
        }
    }

    private void Listen()
    {
        while (_running && _listener != null)
        {
            try
            {
                var context = _listener.GetContext();
                _ = Task.Run(() => HandleRequest(context));
            }
            catch (HttpListenerException) when (!_running)
            {
                break;
            }
            catch (Exception ex)
            {
                LogUtil.LogError($"Error in listener: {ex.Message}", "WebServer");
            }
        }
    }

    private async Task HandleRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 200;
                response.Close();
                return;
            }

            string? responseString = request.Url?.LocalPath switch
            {
                "/" => HtmlTemplate,
                "/api/bot/instances" => GetInstances(),
                var path when path?.StartsWith("/api/bot/instances/") == true && path.EndsWith("/bots") =>
                    GetBots(ExtractPort(path)),
                var path when path?.StartsWith("/api/bot/instances/") == true && path.EndsWith("/command") =>
                    await RunCommand(request, ExtractPort(path)),
                "/api/bot/command/all" => await RunAllCommand(request),
                "/api/bot/update/check" => await CheckForUpdates(),
                "/api/bot/update/idle-status" => GetIdleStatus(),
                "/api/bot/update/all" => await UpdateAllInstances(request),
                _ => null
            };

            if (responseString == null)
            {
                response.StatusCode = 404;
                responseString = "Not Found";
            }
            else
            {
                response.StatusCode = 200;
                response.ContentType = request.Url?.LocalPath == "/" ? "text/html" : "application/json";
            }

            var buffer = Encoding.UTF8.GetBytes(responseString);
            await response.OutputStream.WriteAsync(buffer, _cts.Token);
            response.Close();
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Error processing request: {ex.Message}", "WebServer");

            try
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
            catch { }
        }
    }

    private async Task<string> UpdateAllInstances(HttpListenerRequest request)
    {
        try
        {
            // Read request body to check for stage parameter
            using var reader = new StreamReader(request.InputStream);
            var body = await reader.ReadToEndAsync();
            var stage = "start"; // default

            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var requestData = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                    if (requestData?.ContainsKey("stage") == true)
                    {
                        stage = requestData["stage"];
                    }
                }
                catch { }
            }

            if (stage == "proceed")
            {
                // Proceed with actual updates after all bots are idle
                var result = await UpdateManager.ProceedWithUpdatesAsync(_mainForm, _tcpPort);

                return JsonSerializer.Serialize(new
                {
                    Stage = "updating",
                    Success = result.UpdatesFailed == 0 && result.UpdatesNeeded > 0,
                result.TotalInstances,
                result.UpdatesNeeded,
                result.UpdatesStarted,
                result.UpdatesFailed,
                    Results = result.InstanceResults.Select(r => new
                    {
                        r.Port,
                        r.ProcessId,
                        r.CurrentVersion,
                        r.LatestVersion,
                        r.NeedsUpdate,
                        r.UpdateStarted,
                        r.Error
                    })
                });
            }
            else
            {
                // Start the idle process
                var result = await UpdateManager.StartUpdateProcessAsync(_mainForm, _tcpPort);

                return JsonSerializer.Serialize(new
                {
                    Stage = "idling",
                    Success = result.UpdatesFailed == 0,
                    TotalInstances = result.TotalInstances,
                    UpdatesNeeded = result.UpdatesNeeded,
                    Results = result.InstanceResults.Select(r => new
                    {
                        r.Port,
                        r.ProcessId,
                        r.CurrentVersion,
                        r.LatestVersion,
                        r.NeedsUpdate,
                        r.Error
                    })
                });
            }
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message);
        }
    }

    private string GetIdleStatus()
    {
        try
        {
            var instances = new List<object>();

            // Local instance
            var localBots = GetBotControllers();
            var localIdleCount = 0;
            var localTotalCount = localBots.Count;
            var localNonIdleBots = new List<object>();

            foreach (var controller in localBots)
            {
                var status = controller.ReadBotState();
                var upperStatus = status?.ToUpper() ?? "";

                if (upperStatus == "IDLE" || upperStatus == "STOPPED")
                {
                    localIdleCount++;
                }
                else
                {
                    localNonIdleBots.Add(new
                    {
                        Name = GetBotName(controller.State, GetConfig()),
                        Status = status
                    });
                }
            }

            instances.Add(new
            {
                Port = _tcpPort,
                ProcessId = Environment.ProcessId,
                TotalBots = localTotalCount,
                IdleBots = localIdleCount,
                NonIdleBots = localNonIdleBots,
                AllIdle = localIdleCount == localTotalCount
            });

            // Remote instances
            var remoteInstances = ScanRemoteInstances().Where(i => i.IsOnline);
            foreach (var instance in remoteInstances)
            {
                var botsResponse = QueryRemote(instance.Port, "LISTBOTS");
                if (botsResponse.StartsWith("{") && botsResponse.Contains("Bots"))
                {
                    try
                    {
                        var botsData = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(botsResponse);
                        if (botsData?.ContainsKey("Bots") == true)
                        {
                            var bots = botsData["Bots"];
                            var idleCount = 0;
                            var nonIdleBots = new List<object>();

                            foreach (var bot in bots)
                            {
                                if (bot.TryGetValue("Status", out var status))
                                {
                                    var statusStr = status?.ToString()?.ToUpperInvariant() ?? "";
                                    if (statusStr == "IDLE" || statusStr == "STOPPED")
                                    {
                                        idleCount++;
                                    }
                                    else
                                    {
                                        nonIdleBots.Add(new
                                        {
                                            Name = bot.TryGetValue("Name", out var name) ? name?.ToString() : "Unknown",
                                            Status = statusStr
                                        });
                                    }
                                }
                            }

                            instances.Add(new
                            {
                                Port = instance.Port,
                                ProcessId = instance.ProcessId,
                                TotalBots = bots.Count,
                                IdleBots = idleCount,
                                NonIdleBots = nonIdleBots,
                                AllIdle = idleCount == bots.Count
                            });
                        }
                    }
                    catch { }
                }
            }

            var totalBots = instances.Sum(i => (int)((dynamic)i).TotalBots);
            var totalIdle = instances.Sum(i => (int)((dynamic)i).IdleBots);
            var allInstancesIdle = instances.All(i => (bool)((dynamic)i).AllIdle);

            return JsonSerializer.Serialize(new
            {
                Instances = instances,
                TotalBots = totalBots,
                TotalIdleBots = totalIdle,
                AllBotsIdle = allInstancesIdle
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message);
        }
    }

    private async Task<string> CheckForUpdates()
    {
        try
        {
            var (updateAvailable, _, latestVersion) = await UpdateChecker.CheckForUpdatesAsync(false);
            var changelog = await UpdateChecker.FetchChangelogAsync();

            return JsonSerializer.Serialize(new
            {
                version = latestVersion,
                changelog,
                available = updateAvailable
            });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                version = "Unknown",
                changelog = "Unable to fetch update information",
                available = false,
                error = ex.Message
            });
        }
    }

    private static int ExtractPort(string path)
    {
        var parts = path.Split('/');
        return parts.Length > 4 && int.TryParse(parts[4], out var port) ? port : 0;
    }

    private string GetInstances()
    {
        var instances = new List<BotInstance>
        {
            CreateLocalInstance()
        };

        instances.AddRange(ScanRemoteInstances());

        return JsonSerializer.Serialize(new { Instances = instances });
    }

    private BotInstance CreateLocalInstance()
    {
        var config = GetConfig();
        var controllers = GetBotControllers();

        var mode = config?.Mode.ToString() ?? "Unknown";
        var name = "PokeBot";

        var version = "Unknown";
        try
        {
            var tradeBotType = Type.GetType("SysBot.Pokemon.Helpers.PokeBot, SysBot.Pokemon");
            if (tradeBotType != null)
            {
                var versionField = tradeBotType.GetField("Version",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (versionField != null)
                {
                    version = versionField.GetValue(null)?.ToString() ?? "Unknown";
                }
            }

            if (version == "Unknown")
            {
                version = _mainForm.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";
            }
        }
        catch
        {
            version = _mainForm.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";
        }

        var botStatuses = controllers.Select(c => new BotStatusInfo
        {
            Name = GetBotName(c.State, config),
            Status = c.ReadBotState()
        }).ToList();

        return new BotInstance
        {
            ProcessId = Environment.ProcessId,
            Name = name,
            Port = _tcpPort,
            Version = version,
            Mode = mode,
            BotCount = botStatuses.Count,
            IsOnline = true,
            IsMaster = true,
            BotStatuses = botStatuses
        };
    }

    private List<BotInstance> ScanRemoteInstances()
    {
        var instances = new List<BotInstance>();
        var currentPid = Environment.ProcessId;

        try
        {
            // Search for both PokeBot and SVRaidBot processes
            var pokeBotProcesses = Process.GetProcessesByName("PokeBot")
                .Where(p => p.Id != currentPid);
            
            var raidBotProcesses = Process.GetProcessesByName("SVRaidBot")
                .Where(p => p.Id != currentPid);

            var allProcesses = pokeBotProcesses.Concat(raidBotProcesses);

            foreach (var process in allProcesses)
            {
                var instance = TryCreateInstance(process);
                if (instance != null)
                    instances.Add(instance);
            }
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Error scanning remote instances: {ex.Message}", "WebServer");
        }

        return instances;
    }

    private static BotInstance? TryCreateInstance(Process process)
    {
        try
        {
            var exePath = process.MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
                return null;

            var exeDir = Path.GetDirectoryName(exePath)!;
            
            // Try both PokeBot and SVRaidBot port file formats
            var pokeBotPortFile = Path.Combine(exeDir, $"PokeBot_{process.Id}.port");
            var raidBotPortFile = Path.Combine(exeDir, $"SVRaidBot_{process.Id}.port");
            
            string? portFile = null;
            if (File.Exists(pokeBotPortFile))
                portFile = pokeBotPortFile;
            else if (File.Exists(raidBotPortFile))
                portFile = raidBotPortFile;
            
            if (portFile == null)
                return null;

            var portText = File.ReadAllText(portFile).Trim();
            if (portText.StartsWith("ERROR:") || !int.TryParse(portText, out var port))
                return null;

            var isOnline = IsPortOpen(port);
            var instance = new BotInstance
            {
                ProcessId = process.Id,
                Name = process.ProcessName, // Use actual process name
                Port = port,
                Version = "Unknown",
                Mode = "Unknown",
                BotCount = 0,
                IsOnline = isOnline
            };

            if (isOnline)
            {
                UpdateInstanceInfo(instance, port);
            }

            return instance;
        }
        catch
        {
            return null;
        }
    }

    private static void UpdateInstanceInfo(BotInstance instance, int port)
    {
        try
        {
            var infoResponse = QueryRemote(port, "INFO");
            if (infoResponse.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(infoResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("Version", out var version))
                    instance.Version = version.GetString() ?? "Unknown";

                if (root.TryGetProperty("Mode", out var mode))
                    instance.Mode = mode.GetString() ?? "Unknown";

                if (root.TryGetProperty("Name", out var name))
                    instance.Name = name.GetString() ?? "PokeBot";
            }

            var botsResponse = QueryRemote(port, "LISTBOTS");
            if (botsResponse.StartsWith("{") && botsResponse.Contains("Bots"))
            {
                var botsData = JsonSerializer.Deserialize<Dictionary<string, List<BotInfo>>>(botsResponse);
                if (botsData?.ContainsKey("Bots") == true)
                {
                    instance.BotCount = botsData["Bots"].Count;
                    instance.BotStatuses = [.. botsData["Bots"].Select(b => new BotStatusInfo
                    {
                        Name = b.Name,
                        Status = b.Status
                    })];
                }
            }
        }
        catch { }
    }

    private static bool IsPortOpen(int port)
    {
        try
        {
            using var client = new TcpClient();
            var result = client.BeginConnect("127.0.0.1", port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            if (success)
            {
                client.EndConnect(result);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private string GetBots(int port)
    {
        if (port == _tcpPort)
        {
            var config = GetConfig();
            var controllers = GetBotControllers();

            var bots = controllers.Select(c => new BotInfo
            {
                Id = $"{c.State.Connection.IP}:{c.State.Connection.Port}",
                Name = GetBotName(c.State, config),
                RoutineType = c.State.InitialRoutine.ToString(),
                Status = c.ReadBotState(),
                ConnectionType = c.State.Connection.Protocol.ToString(),
                IP = c.State.Connection.IP,
                Port = c.State.Connection.Port
            }).ToList();

            return JsonSerializer.Serialize(new { Bots = bots });
        }

        return QueryRemote(port, "LISTBOTS");
    }

    private async Task<string> RunCommand(HttpListenerRequest request, int port)
    {
        try
        {
            using var reader = new StreamReader(request.InputStream);
            var body = await reader.ReadToEndAsync();
            var commandRequest = JsonSerializer.Deserialize<BotCommandRequest>(body);

            if (commandRequest == null)
                return CreateErrorResponse("Invalid command request");

            if (port == _tcpPort)
            {
                return RunLocalCommand(commandRequest.Command);
            }

            var tcpCommand = $"{commandRequest.Command}All".ToUpper();
            var result = QueryRemote(port, tcpCommand);

            return JsonSerializer.Serialize(new CommandResponse
            {
                Success = !result.StartsWith("ERROR"),
                Message = result,
                Port = port,
                Command = commandRequest.Command,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message);
        }
    }

    private async Task<string> RunAllCommand(HttpListenerRequest request)
    {
        try
        {
            using var reader = new StreamReader(request.InputStream);
            var body = await reader.ReadToEndAsync();
            var commandRequest = JsonSerializer.Deserialize<BotCommandRequest>(body);

            if (commandRequest == null)
                return CreateErrorResponse("Invalid command request");

            var results = new List<CommandResponse>();

            var localResult = JsonSerializer.Deserialize<CommandResponse>(RunLocalCommand(commandRequest.Command));
            if (localResult != null)
            {
                localResult.InstanceName = _mainForm.Text;
                results.Add(localResult);
            }

            var remoteInstances = ScanRemoteInstances().Where(i => i.IsOnline);
            foreach (var instance in remoteInstances)
            {
                try
                {
                    var result = QueryRemote(instance.Port, $"{commandRequest.Command}All".ToUpper());
                    results.Add(new CommandResponse
                    {
                        Success = !result.StartsWith("ERROR"),
                        Message = result,
                        Port = instance.Port,
                        Command = commandRequest.Command,
                        InstanceName = instance.Name
                    });
                }
                catch { }
            }

            return JsonSerializer.Serialize(new BatchCommandResponse
            {
                Results = results,
                TotalInstances = results.Count,
                SuccessfulCommands = results.Count(r => r.Success)
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message);
        }
    }

    private string RunLocalCommand(string command)
    {
        try
        {
            var cmd = MapCommand(command);

            _mainForm.BeginInvoke((System.Windows.Forms.MethodInvoker)(() =>
            {
                var sendAllMethod = _mainForm.GetType().GetMethod("SendAll",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                sendAllMethod?.Invoke(_mainForm, new object[] { cmd });
            }));

            return JsonSerializer.Serialize(new CommandResponse
            {
                Success = true,
                Message = $"Command {command} sent successfully",
                Port = _tcpPort,
                Command = command,
                Timestamp = DateTime.Now
            });
        }
        catch
        {
            return JsonSerializer.Serialize(new CommandResponse
            {
                Success = true,
                Message = $"Command {command} sent successfully",
                Port = _tcpPort,
                Command = command,
                Timestamp = DateTime.Now
            });
        }
    }

    private static BotControlCommand MapCommand(string webCommand)
    {
        return webCommand.ToLower() switch
        {
            "start" => BotControlCommand.Start,
            "stop" => BotControlCommand.Stop,
            "idle" => BotControlCommand.Idle,
            "resume" => BotControlCommand.Resume,
            "restart" => BotControlCommand.Restart,
            "reboot" => BotControlCommand.RebootAndStop,
            "screenon" => BotControlCommand.ScreenOnAll,
            "screenoff" => BotControlCommand.ScreenOffAll,
            _ => BotControlCommand.None
        };
    }

    public static string QueryRemote(int port, string command)
    {
        try
        {
            using var client = new TcpClient();
            client.Connect("127.0.0.1", port);

            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.UTF8);

            writer.WriteLine(command);
            return reader.ReadLine() ?? "No response";
        }
        catch
        {
            return "Failed to connect";
        }
    }

    private List<BotController> GetBotControllers()
    {
        var flpBotsField = _mainForm.GetType().GetField("FLP_Bots",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (flpBotsField?.GetValue(_mainForm) is FlowLayoutPanel flpBots)
        {
            return [.. flpBots.Controls.OfType<BotController>()];
        }

        return new List<BotController>();
    }

    private ProgramConfig? GetConfig()
    {
        var configProp = _mainForm.GetType().GetProperty("Config",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return configProp?.GetValue(_mainForm) as ProgramConfig;
    }

    private static string GetBotName(PokeBotState state, ProgramConfig? config)
    {
        return state.Connection.IP;
    }

    private static string CreateErrorResponse(string message)
    {
        return JsonSerializer.Serialize(new CommandResponse
        {
            Success = false,
            Message = $"Error: {message}"
        });
    }

    public void Dispose()
    {
        Stop();
        _listener?.Close();
        _cts?.Dispose();
    }
}

public class BotInstance
{
    public int ProcessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Version { get; set; } = string.Empty;
    public int BotCount { get; set; }
    public string Mode { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public bool IsMaster { get; set; }
    public List<BotStatusInfo>? BotStatuses { get; set; }
}

public class BotStatusInfo
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class BotInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RoutineType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ConnectionType { get; set; } = string.Empty;
    public string IP { get; set; } = string.Empty;
    public int Port { get; set; }
}

public class BotCommandRequest
{
    public string Command { get; set; } = string.Empty;
    public string? BotId { get; set; }
}

public class CommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Command { get; set; } = string.Empty;
    public string? InstanceName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class BatchCommandResponse
{
    public List<CommandResponse> Results { get; set; } = [];
    public int TotalInstances { get; set; }
    public int SuccessfulCommands { get; set; }
}
