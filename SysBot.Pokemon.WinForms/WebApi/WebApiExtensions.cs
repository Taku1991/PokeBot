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

    // Bot type detection
    public enum BotType
    {
        PokeBot,
        RaidBot,
        Unknown
    }

    public static void InitWebServer(this Main mainForm)
    {
        _main = mainForm;

        try
        {
            CleanupStalePortFiles();

            if (IsPortInUse(WebPort))
            {
                LogUtil.LogInfo($"Web port {WebPort} is in use by another bot instance. Starting as slave...", "WebServer");
                _tcpPort = FindAvailablePort(8081);
                StartTcpOnly();
                LogUtil.LogInfo($"Slave instance started with TCP port {_tcpPort}. Monitoring master...", "WebServer");

                StartMasterMonitor();
                return;
            }

            TryAddUrlReservation(WebPort);

            _tcpPort = FindAvailablePort(8081);
            LogUtil.LogInfo($"Starting as master web server on port {WebPort} with TCP port {_tcpPort}", "WebServer");
            StartFullServer();
            LogUtil.LogInfo($"Web interface is available at http://localhost:{WebPort}", "WebServer");
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

            // Clean up both PokeBot and RaidBot port files
            var pokeBotPortFiles = Directory.GetFiles(exeDir, "PokeBot_*.port");
            var raidBotPortFiles = Directory.GetFiles(exeDir, "SVRaidBot_*.port");
            var allPortFiles = pokeBotPortFiles.Concat(raidBotPortFiles);

            foreach (var portFile in allPortFiles)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(portFile);
                    var parts = fileName.Split('_');
                    if (parts.Length < 2) continue;

                    var pidStr = parts[1];
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

    private static BotType DetectBotType()
    {
        try
        {
            // Try to detect PokeBot first
            var pokeBotType = Type.GetType("SysBot.Pokemon.Helpers.PokeBot, SysBot.Pokemon");
            if (pokeBotType != null)
                return BotType.PokeBot;

            // Try to detect RaidBot
            var raidBotType = Type.GetType("SysBot.Pokemon.SV.BotRaid.Helpers.SVRaidBot, SysBot.Pokemon");
            if (raidBotType != null)
                return BotType.RaidBot;

            return BotType.Unknown;
        }
        catch
        {
            return BotType.Unknown;
        }
    }

    private static string GetVersionForBotType(BotType botType)
    {
        try
        {
            return botType switch
            {
                BotType.PokeBot => GetPokeBotVersion(),
                BotType.RaidBot => GetRaidBotVersion(),
                _ => "Unknown"
            };
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string GetPokeBotVersion()
    {
        try
        {
            var pokeBotType = Type.GetType("SysBot.Pokemon.Helpers.PokeBot, SysBot.Pokemon");
            if (pokeBotType != null)
            {
                var versionField = pokeBotType.GetField("Version",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (versionField != null)
                {
                    return versionField.GetValue(null)?.ToString() ?? "Unknown";
                }
            }
            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string GetRaidBotVersion()
    {
        try
        {
            var raidBotType = Type.GetType("SysBot.Pokemon.SV.BotRaid.Helpers.SVRaidBot, SysBot.Pokemon");
            if (raidBotType != null)
            {
                var versionField = raidBotType.GetField("Version",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (versionField != null)
                {
                    return versionField.GetValue(null)?.ToString() ?? "Unknown";
                }
            }
            return "Unknown";
        }
        catch
        {
            return "Unknown";
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
                            "REFRESHMAPALL" => HandleRefreshMapAllCommand(),
            "LISTBOTS" => GetBotsList(),
            "STATUS" => GetBotStatuses(botId),
            "ISREADY" => CheckReady(),
            "INFO" => GetInstanceInfo(),
            "VERSION" => GetVersionForBotType(DetectBotType()),
            "UPDATE" => TriggerUpdate(),
            _ => $"ERROR: Unknown command '{cmd}'"
        };
    }

    private static string TryExecuteRaidBotCommand(BotControlCommand command)
    {
        try
        {
            var botType = DetectBotType();
            if (botType == BotType.RaidBot)
            {
                return ExecuteGlobalCommand(command);
            }
            else
            {
                return "OK: Command not applicable to this bot type";
            }
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to execute {command} - {ex.Message}";
        }
    }

    private static string TriggerUpdate()
    {
        try
        {
            if (_main == null)
                return "ERROR: Main form not initialized";

            var botType = DetectBotType();

            _main.BeginInvoke((MethodInvoker)(async () =>
            {
                try
                {
                    var (updateAvailable, _, newVersion) = await CheckForUpdatesForBotType(botType);
                    if (updateAvailable)
                    {
                        var updateForm = await CreateUpdateFormForBotType(botType, newVersion);
                        if (updateForm != null)
                        {
                            updateForm.PerformUpdate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogError($"Error during update trigger: {ex.Message}", "WebServer");
                }
            }));

            return "OK: Update triggered";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    private static async Task<(bool, string, string)> CheckForUpdatesForBotType(BotType botType)
    {
        try
        {
            return botType switch
            {
                BotType.PokeBot => await CheckPokeBotUpdates(),
                BotType.RaidBot => await CheckRaidBotUpdates(),
                _ => (false, "", "Unknown")
            };
        }
        catch
        {
            return (false, "", "Unknown");
        }
    }

    private static async Task<(bool, string, string)> CheckPokeBotUpdates()
    {
        try
        {
            var result = await UpdateChecker.CheckForUpdatesAsync(false);
            return (result.UpdateAvailable, "", result.NewVersion);
        }
        catch
        {
            return (false, "", "Unknown");
        }
    }

    private static async Task<(bool, string, string)> CheckRaidBotUpdates()
    {
        try
        {
            // Use RaidBot's UpdateChecker if available
            var raidUpdateCheckerType = Type.GetType("SysBot.Pokemon.SV.BotRaid.Helpers.UpdateChecker, SysBot.Pokemon");
            if (raidUpdateCheckerType != null)
            {
                var checkMethod = raidUpdateCheckerType.GetMethod("CheckForUpdatesAsync");
                if (checkMethod != null)
                {
                    var task = (Task<(bool, string, string)>)checkMethod.Invoke(null, new object[] { false });
                    return await task;
                }
            }

            return (false, "", "Unknown");
        }
        catch
        {
            return (false, "", "Unknown");
        }
    }

    private static async Task<dynamic?> CreateUpdateFormForBotType(BotType botType, string latestVersion)
    {
        try
        {
            return botType switch
            {
                BotType.PokeBot => new UpdateForm(false, latestVersion, true),
                BotType.RaidBot => await CreateRaidBotUpdateForm(latestVersion),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static async Task<dynamic?> CreateRaidBotUpdateForm(string latestVersion)
    {
        try
        {
            var updateFormType = Type.GetType("SysBot.Pokemon.WinForms.UpdateForm, SysBot.Pokemon.WinForms");
            if (updateFormType != null)
            {
                return Activator.CreateInstance(updateFormType, false, latestVersion, true);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string GetInstanceInfo()
    {
        try
        {
            var config = GetConfig();
            var botType = DetectBotType();
            var version = GetVersionForBotType(botType);
            var mode = config?.Mode.ToString() ?? "Unknown";
            var name = GetInstanceName(config, mode, botType);

            var info = new
            {
                Version = version,
                Mode = mode,
                Name = name,
                Environment.ProcessId,
                Port = _tcpPort,
                BotType = botType.ToString()
            };

            return System.Text.Json.JsonSerializer.Serialize(info);
        }
        catch (Exception ex)
        {
            return $"ERROR: Failed to get instance info - {ex.Message}";
        }
    }

    private static string GetInstanceName(ProgramConfig? config, string mode, BotType botType)
    {
        if (!string.IsNullOrEmpty(config?.Hub?.BotName))
            return config.Hub.BotName;

        return botType switch
        {
            BotType.PokeBot => mode switch
            {
                "LGPE" => "LGPE Bot",
                "BDSP" => "BDSP Bot",
                "SWSH" => "SWSH Bot",
                "SV" => "SV PokeBot",
                "LA" => "LA Bot",
                _ => "PokeBot"
            },
            BotType.RaidBot => "SV RaidBot",
            _ => "Universal Bot"
        };
    }

    private static void CreatePortFile()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
            var exeDir = Path.GetDirectoryName(exePath) ?? Program.WorkingDirectory;

            var botType = DetectBotType();
            var portFileName = botType switch
            {
                BotType.PokeBot => $"PokeBot_{Environment.ProcessId}.port",
                BotType.RaidBot => $"SVRaidBot_{Environment.ProcessId}.port",
                _ => $"UniversalBot_{Environment.ProcessId}.port"
            };

            var portFile = Path.Combine(exeDir, portFileName);
            File.WriteAllText(portFile, _tcpPort.ToString());
            LogUtil.LogInfo($"Created port file: {portFileName} with port {_tcpPort}", "WebServer");
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

            var botType = DetectBotType();
            var portFileName = botType switch
            {
                BotType.PokeBot => $"PokeBot_{Environment.ProcessId}.port",
                BotType.RaidBot => $"SVRaidBot_{Environment.ProcessId}.port",
                _ => $"UniversalBot_{Environment.ProcessId}.port"
            };

            var portFile = Path.Combine(exeDir, portFileName);

            if (File.Exists(portFile))
            {
                File.Delete(portFile);
                LogUtil.LogInfo($"Cleaned up port file: {portFileName}", "WebServer");
            }
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

    private static string HandleRefreshMapAllCommand()
    {
        // RefreshMap is only available for RaidBot, not PokeBot
        return "ERROR: REFRESHMAPALL command is only available for RaidBot instances";
    }
}
