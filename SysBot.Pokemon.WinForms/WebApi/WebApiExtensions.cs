using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysBot.Base;
using System.Diagnostics;
using SysBot.Pokemon.Helpers;
using SysBot.Pokemon.WinForms.WebApi;

namespace SysBot.Pokemon.WinForms;

public static class WebApiExtensions
{
    private static BotServer? _server;
    private static TcpListener? _tcp;
    private static CancellationTokenSource? _cts;
    private static CancellationTokenSource? _monitorCts;
    private static Main? _main;

    private const int WebPort = 8080;
    private static int _tcpPort = 0;

    public static void InitWebServer(this Main mainForm)
    {
        _main = mainForm;

        try
        {
            CleanupStalePortFiles();

            if (IsPortInUse(WebPort))
            {
                _tcpPort = FindAvailablePort(8081);
                StartTcpOnly();

                StartMasterMonitor();
                return;
            }

            TryAddUrlReservation(WebPort);

            _tcpPort = FindAvailablePort(8081);
            StartFullServer();
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to initialize web server: {ex.Message}", "WebServer");
        }
    }

    private static void CleanupStalePortFiles()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
            var exeDir = Path.GetDirectoryName(exePath) ?? Program.WorkingDirectory;

            var portFiles = Directory.GetFiles(exeDir, "PokeBot_*.port");

            foreach (var portFile in portFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(portFile);
                    var pidStr = fileName.Substring("PokeBot_".Length);

                    if (int.TryParse(pidStr, out int pid))
                    {
                        if (pid == Environment.ProcessId)
                            continue;

                        try
                        {
                            var process = Process.GetProcessById(pid);
                            if (process.ProcessName.Contains("SysBot", StringComparison.OrdinalIgnoreCase) ||
                                process.ProcessName.Contains("PokeBot", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }
                        catch (ArgumentException)
                        {
                        }

                        File.Delete(portFile);
                        LogUtil.LogInfo($"Cleaned up stale port file: {Path.GetFileName(portFile)}", "WebServer");
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogError($"Error processing port file {portFile}: {ex.Message}", "WebServer");
                }
            }
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to cleanup stale port files: {ex.Message}", "WebServer");
        }
    }

    private static void StartMasterMonitor()
    {
        _monitorCts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            var random = new Random();

            while (!_monitorCts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(10000 + random.Next(5000), _monitorCts.Token);

                    if (!IsPortInUse(WebPort))
                    {
                        LogUtil.LogInfo("Master web server is down. Attempting to take over...", "WebServer");

                        await Task.Delay(random.Next(1000, 3000));

                        if (!IsPortInUse(WebPort))
                        {
                            TryTakeOverAsMaster();
                            break;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    LogUtil.LogError($"Error in master monitor: {ex.Message}", "WebServer");
                }
            }
        }, _monitorCts.Token);
    }

    private static void TryTakeOverAsMaster()
    {
        try
        {
            TryAddUrlReservation(WebPort);

            _server = new BotServer(_main!, WebPort, _tcpPort);
            _server.Start();

            _monitorCts?.Cancel();
            _monitorCts = null;

            LogUtil.LogInfo($"Successfully took over as master web server on port {WebPort}", "WebServer");
            LogUtil.LogInfo($"Web interface is now available at http://localhost:{WebPort}", "WebServer");

            if (_main != null)
            {
                _main.BeginInvoke((MethodInvoker)(() =>
                {
                    System.Windows.Forms.MessageBox.Show(
                        $"This instance has taken over as the master web server.\n\nWeb interface available at:\nhttp://localhost:{WebPort}",
                        "Master Server Takeover",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                }));
            }
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to take over as master: {ex.Message}", "WebServer");
            StartMasterMonitor();
        }
    }

    private static bool TryAddUrlReservation(int port)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = $"http add urlacl url=http://+:{port}/ user=Everyone",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            process?.WaitForExit(5000);
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static void StartTcpOnly()
    {
        CreatePortFile();
        StartTcp();
    }

    private static void StartFullServer()
    {
        CreatePortFile();
        _server = new BotServer(_main!, WebPort, _tcpPort);
        _server.Start();
        StartTcp();
    }

    private static void StartTcp()
    {
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            try
            {
                _tcp = new TcpListener(System.Net.IPAddress.Loopback, _tcpPort);
                _tcp.Start();

                while (!_cts.Token.IsCancellationRequested)
                {
                    var tcpTask = _tcp.AcceptTcpClientAsync();
                    var tcs = new TaskCompletionSource<bool>();

                    using (var registration = _cts.Token.Register(() => tcs.SetCanceled()))
                    {
                        var completedTask = await Task.WhenAny(tcpTask, tcs.Task);
                        if (completedTask == tcpTask && tcpTask.IsCompletedSuccessfully)
                        {
                            _ = Task.Run(() => HandleClient(tcpTask.Result));
                        }
                    }
                }
            }
            catch (Exception ex) when (!_cts.Token.IsCancellationRequested)
            {
                LogUtil.LogError($"TCP listener error: {ex.Message}", "TCP");
            }
        });
    }

    private static async Task HandleClient(TcpClient client)
    {
        try
        {
            using (client)
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                var command = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(command))
                {
                    var response = ProcessCommand(command);
                    await writer.WriteLineAsync(response);
                    await stream.FlushAsync();
                    await Task.Delay(100);
                }
            }
        }
        catch (Exception ex) when (!(ex is IOException { InnerException: SocketException }))
        {
            LogUtil.LogError($"Error handling TCP client: {ex.Message}", "TCP");
        }
    }

    private static string ProcessCommand(string command)
    {
        if (_main == null)
            return "ERROR: Main form not initialized";

        // Parse command and optional bot ID (format: "COMMAND:BOTID")
        var parts = command.Split(':');
        var cmd = parts[0].ToUpperInvariant();
        var botId = parts.Length > 1 ? parts[1] : null;

        return cmd switch
        {
            "STARTALL" => ExecuteGlobalCommand(BotControlCommand.Start),
            "STOPALL" => ExecuteGlobalCommand(BotControlCommand.Stop),
            "IDLEALL" => ExecuteGlobalCommand(BotControlCommand.Idle),
            "RESUMEALL" => ExecuteGlobalCommand(BotControlCommand.Resume),
            "RESTARTALL" => ExecuteGlobalCommand(BotControlCommand.Restart),
            "REBOOTALL" => ExecuteGlobalCommand(BotControlCommand.RebootAndStop),
            "SCREENONALL" => ExecuteGlobalCommand(BotControlCommand.ScreenOnAll),
            "SCREENOFFALL" => ExecuteGlobalCommand(BotControlCommand.ScreenOffAll),
            
            // Individual bot commands (with :botId)
            "START" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.Start, botId),
            "STOP" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.Stop, botId),
            "IDLE" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.Idle, botId),
            "RESUME" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.Resume, botId),
            "RESTART" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.Restart, botId),
            "REBOOT" when !string.IsNullOrEmpty(botId) => ExecuteIndividualCommand(BotControlCommand.RebootAndStop, botId),
            
            // Fallback: if no botId specified, execute on all bots
            "START" => ExecuteGlobalCommand(BotControlCommand.Start),
            "STOP" => ExecuteGlobalCommand(BotControlCommand.Stop),
            "IDLE" => ExecuteGlobalCommand(BotControlCommand.Idle),
            "RESUME" => ExecuteGlobalCommand(BotControlCommand.Resume),
            "RESTART" => ExecuteGlobalCommand(BotControlCommand.Restart),
            "REBOOT" => ExecuteGlobalCommand(BotControlCommand.RebootAndStop),
            "REFRESHMAP" => "OK: RefreshMap command received (feature not implemented for this bot configuration)",
            "SCREENON" => ExecuteGlobalCommand(BotControlCommand.ScreenOnAll),
            "SCREENOFF" => ExecuteGlobalCommand(BotControlCommand.ScreenOffAll),
            
            "LISTBOTS" => GetBotsList(),
            "STATUS" => GetBotStatuses(botId),
            "ISREADY" => CheckReady(),
            "INFO" => GetInstanceInfo(),
            "VERSION" => GetVersion(),
            "UPDATE" => TriggerUpdate(),
            _ => $"ERROR: Unknown command '{cmd}'. Use format 'COMMAND' or 'COMMAND:BOTID' for individual bot control."
        };
    }

    private static string TriggerUpdate()
    {
        try
        {
            if (_main == null)
                return "ERROR: Main form not initialized";

            _main.BeginInvoke((MethodInvoker)(async () =>
            {
                var (updateAvailable, _, newVersion) = await UpdateChecker.CheckForUpdatesAsync(false);
                if (updateAvailable)
                {
                    var updateForm = new UpdateForm(false, newVersion, true);
                    updateForm.PerformUpdate();
                }
            }));

            return "OK: Update triggered";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    private static string ExecuteGlobalCommand(BotControlCommand command)
    {
        try
        {
            _main!.BeginInvoke((MethodInvoker)(() =>
            {
                var sendAllMethod = _main.GetType().GetMethod("SendAll",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                sendAllMethod?.Invoke(_main, new object[] { command });
            }));

            return $"OK: {command} command sent to all bots";
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to execute {command} - {ex.Message}";
        }
    }

    private static string ExecuteIndividualCommand(BotControlCommand command, string botId)
    {
        try
        {
            var controllers = GetBotControllers();
            var targetBot = controllers.FirstOrDefault(c => 
                $"{c.State.Connection.IP}:{c.State.Connection.Port}" == botId ||
                c.State.Connection.IP == botId);

            if (targetBot == null)
            {
                return $"ERROR: Bot with ID '{botId}' not found. Available bots: {string.Join(", ", controllers.Select(c => $"{c.State.Connection.IP}:{c.State.Connection.Port}"))}";
            }

            _main!.BeginInvoke((MethodInvoker)(() =>
            {
                // Send command to specific bot controller
                var sendMethod = targetBot.GetType().GetMethod("SendCommand", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (sendMethod != null)
                {
                    sendMethod.Invoke(targetBot, new object[] { command });
                }
                else
                {
                    // Fallback: try to get the bot and send command directly
                    var bot = targetBot.GetBot();
                    if (bot != null)
                    {
                        switch (command)
                        {
                            case BotControlCommand.Start:
                                if (!bot.IsRunning) bot.Start();
                                break;
                            case BotControlCommand.Stop:
                                if (bot.IsRunning) bot.Stop();
                                break;
                            case BotControlCommand.Idle:
                                bot.Pause();
                                break;
                            case BotControlCommand.Resume:
                                bot.Start();
                                break;
                        }
                    }
                }
            }));

            return $"OK: {command} command sent to bot {botId} ({targetBot.State.Connection.IP})";
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to execute {command} on bot {botId} - {ex.Message}";
        }
    }

    private static string GetBotsList()
    {
        try
        {
            var botList = new List<object>();
            var config = GetConfig();
            var controllers = GetBotControllers();

            if (controllers.Count == 0)
            {
                var botsProperty = _main!.GetType().GetProperty("Bots",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (botsProperty?.GetValue(_main) is List<PokeBotState> bots)
                {
                    foreach (var bot in bots)
                    {
                        botList.Add(new
                        {
                            Id = $"{bot.Connection.IP}:{bot.Connection.Port}",
                            Name = bot.Connection.IP,
                            RoutineType = bot.InitialRoutine.ToString(),
                            Status = "Unknown",
                            ConnectionType = bot.Connection.Protocol.ToString(),
                            bot.Connection.IP,
                            bot.Connection.Port
                        });
                    }

                    return System.Text.Json.JsonSerializer.Serialize(new { Bots = botList });
                }
            }

            foreach (var controller in controllers)
            {
                var state = controller.State;
                var botName = GetBotName(state, config);
                var status = controller.ReadBotState();

                botList.Add(new
                {
                    Id = $"{state.Connection.IP}:{state.Connection.Port}",
                    Name = botName,
                    RoutineType = state.InitialRoutine.ToString(),
                    Status = status,
                    ConnectionType = state.Connection.Protocol.ToString(),
                    state.Connection.IP,
                    state.Connection.Port
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new { Bots = botList });
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"GetBotsList error: {ex.Message}", "WebAPI");
            return $"ERROR: Failed to get bots list - {ex.Message}";
        }
    }

    private static string GetBotStatuses(string? botId)
    {
        try
        {
            var config = GetConfig();
            var controllers = GetBotControllers();

            if (string.IsNullOrEmpty(botId))
            {
                var statuses = controllers.Select(c => new
                {
                    Id = $"{c.State.Connection.IP}:{c.State.Connection.Port}",
                    Name = GetBotName(c.State, config),
                    Status = c.ReadBotState()
                }).ToList();

                return System.Text.Json.JsonSerializer.Serialize(statuses);
            }

            var botController = controllers.FirstOrDefault(c =>
                $"{c.State.Connection.IP}:{c.State.Connection.Port}" == botId);

            return botController?.ReadBotState() ?? "ERROR: Bot not found";
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to get status - {ex.Message}";
        }
    }

    private static string CheckReady()
    {
        try
        {
            var controllers = GetBotControllers();
            var hasRunningBots = controllers.Any(c => c.GetBot()?.IsRunning ?? false);
            return hasRunningBots ? "READY" : "NOT_READY";
        }
        catch
        {
            return "NOT_READY";
        }
    }

    private static string GetInstanceInfo()
    {
        try
        {
            var config = GetConfig();
            var version = GetVersion();
            var mode = config?.Mode.ToString() ?? "Unknown";
            var name = GetInstanceName(config, mode);

            var info = new
            {
                Version = version,
                Mode = mode,
                Name = name,
                Environment.ProcessId,
                Port = _tcpPort
            };

            return System.Text.Json.JsonSerializer.Serialize(info);
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to get instance info - {ex.Message}";
        }
    }

    private static List<BotController> GetBotControllers()
    {
        var flpBotsField = _main!.GetType().GetField("FLP_Bots",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (flpBotsField?.GetValue(_main) is FlowLayoutPanel flpBots)
        {
            return [.. flpBots.Controls.OfType<BotController>()];
        }

        return new List<BotController>();
    }

    private static ProgramConfig? GetConfig()
    {
        var configProp = _main?.GetType().GetProperty("Config",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return configProp?.GetValue(_main) as ProgramConfig;
    }

    private static string GetBotName(PokeBotState state, ProgramConfig? config)
    {
        return state.Connection.IP;
    }

    private static string GetVersion()
    {
        return PokeBot.Version;
    }

    private static string GetInstanceName(ProgramConfig? config, string mode)
    {
        if (!string.IsNullOrEmpty(config?.Hub?.BotName))
            return config.Hub.BotName;

        return mode switch
        {
            "LGPE" => "LGPE",
            "BDSP" => "BDSP",
            "SWSH" => "SWSH",
            "SV" => "SV",
            "LA" => "LA",
            _ => "PokeBot"
        };
    }

    private static void CreatePortFile()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
            var exeDir = Path.GetDirectoryName(exePath) ?? Program.WorkingDirectory;
            var portFile = Path.Combine(exeDir, $"PokeBot_{Environment.ProcessId}.port");
            File.WriteAllText(portFile, _tcpPort.ToString());
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to create port file: {ex.Message}", "WebServer");
        }
    }

    private static void CleanupPortFile()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
            var exeDir = Path.GetDirectoryName(exePath) ?? Program.WorkingDirectory;
            var portFile = Path.Combine(exeDir, $"PokeBot_{Environment.ProcessId}.port");

            if (File.Exists(portFile))
                File.Delete(portFile);
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Failed to cleanup port file: {ex.Message}", "WebServer");
        }
    }

    private static int FindAvailablePort(int startPort)
    {
        for (int port = startPort; port < startPort + 100; port++)
        {
            if (!IsPortInUse(port))
                return port;
        }
        throw new InvalidOperationException("No available ports found");
    }

    private static bool IsPortInUse(int port)
    {
        try
        {
            using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromMilliseconds(200) };
            var response = client.GetAsync($"http://localhost:{port}/api/bot/instances").Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            try
            {
                using var tcpClient = new TcpClient();
                var result = tcpClient.BeginConnect("127.0.0.1", port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(200));
                if (success)
                {
                    tcpClient.EndConnect(result);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public static void StopWebServer(this Main mainForm)
    {
        try
        {
            _monitorCts?.Cancel();
            _cts?.Cancel();
            _tcp?.Stop();
            _server?.Dispose();
            CleanupPortFile();
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Error stopping web server: {ex.Message}", "WebServer");
        }
    }
}
