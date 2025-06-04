using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysBot.Base;

namespace SysBot.Pokemon.WinForms.WebApi;

public class BotServer : IDisposable
{
    private HttpListener? _listener;
    private Thread? _listenerThread;
    private readonly int _port;
    private readonly int _tcpPort;
    private readonly CancellationTokenSource _cts = new();
    private readonly Main _mainForm;
    private volatile bool _running;

    private const string HtmlTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>PokeBot Control Center</title>
    <style>
        :root {
            --bg-primary: #0a0e27;
            --bg-secondary: #151932;
            --bg-card: #1e2139;
            --bg-hover: #252846;
            --text-primary: #ffffff;
            --text-secondary: #a8aec0;
            --accent: #7c3aed;
            --accent-hover: #6d28d9;
            --success: #10b981;
            --warning: #f59e0b;
            --danger: #ef4444;
            --border: #2d3054;
            --online: #10b981;
            --offline: #6b7280;
            --idle: #f59e0b;
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: var(--bg-primary); color: var(--text-primary); line-height: 1.6; overflow-x: hidden; }
        .app { min-height: 100vh; display: flex; flex-direction: column; }
        .header { background: rgba(21, 25, 50, 0.95); padding: 1rem 2rem; border-bottom: 1px solid var(--border); position: sticky; top: 0; z-index: 100; backdrop-filter: blur(10px); }
        .header-content { max-width: 1400px; margin: 0 auto; display: flex; justify-content: space-between; align-items: center; }
        .logo { display: flex; align-items: center; gap: 1rem; }
        .logo h1 { font-size: 1.5rem; font-weight: 700; background: linear-gradient(135deg, #7c3aed 0%, #10b981 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
        .status-indicator { width: 12px; height: 12px; border-radius: 50%; background: var(--success); animation: pulse 2s infinite; }
        @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); } 70% { box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); } 100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); } }
        .main { flex: 1; padding: 2rem; max-width: 1400px; margin: 0 auto; width: 100%; }
        .global-controls { background: var(--bg-card); border-radius: 12px; padding: 1.5rem; margin-bottom: 2rem; border: 1px solid var(--border); }
        .global-controls h2 { font-size: 1.2rem; margin-bottom: 1rem; color: var(--text-secondary); }
        .control-buttons { display: flex; flex-wrap: wrap; gap: 0.75rem; }
        .btn { padding: 0.75rem 1.5rem; border: none; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; transition: all 0.2s; display: flex; align-items: center; gap: 0.5rem; background: var(--bg-hover); color: var(--text-primary); border: 1px solid var(--border); position: relative; overflow: hidden; }
        .btn:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3); }
        .btn-primary { background: var(--accent); border-color: var(--accent); }
        .btn-success { background: var(--success); border-color: var(--success); }
        .btn-warning { background: var(--warning); border-color: var(--warning); }
        .btn-danger { background: var(--danger); border-color: var(--danger); }
        .instances-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 1.5rem; }
        .instance-card { background: var(--bg-card); border-radius: 12px; border: 1px solid var(--border); overflow: hidden; transition: all 0.3s; position: relative; }
        .instance-card.online { border-color: var(--online); }
        .instance-card.offline { opacity: 0.7; border-color: var(--offline); }
        .instance-card:hover { transform: translateY(-4px); box-shadow: 0 8px 24px rgba(0, 0, 0, 0.4); }
        .instance-header { background: var(--bg-secondary); padding: 1rem 1.5rem; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid var(--border); }
        .instance-title { font-size: 1.1rem; font-weight: 600; display: flex; align-items: center; gap: 0.5rem; }
        .instance-badge { padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.75rem; font-weight: 600; background: var(--bg-hover); color: var(--text-secondary); }
        .instance-body { padding: 1.5rem; }
        .instance-info { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
        .info-item { display: flex; flex-direction: column; gap: 0.25rem; }
        .info-label { font-size: 0.75rem; color: var(--text-secondary); text-transform: uppercase; letter-spacing: 0.5px; }
        .info-value { font-size: 0.95rem; font-weight: 600; }
        .bot-status { display: grid; grid-template-columns: 1fr; gap: 0.5rem; margin-bottom: 1rem; padding: 0.75rem; background: var(--bg-hover); border-radius: 8px; }
        .bot-status-item { display: flex; align-items: center; justify-content: space-between; font-size: 0.875rem; padding: 0.25rem 0; }
        .bot-status-item .bot-name { display: flex; align-items: center; gap: 0.5rem; }
        .bot-status-item .bot-state { font-weight: 600; padding: 0.125rem 0.5rem; border-radius: 4px; font-size: 0.75rem; }
        .bot-state.running { background: rgba(16, 185, 129, 0.2); color: var(--success); }
        .bot-state.stopped { background: rgba(239, 68, 68, 0.2); color: var(--danger); }
        .bot-state.idle { background: rgba(245, 158, 11, 0.2); color: var(--idle); }
        .bot-state.error { background: rgba(239, 68, 68, 0.2); color: var(--danger); }
        .loading { display: flex; align-items: center; justify-content: center; min-height: 200px; }
        .spinner { width: 40px; height: 40px; border: 3px solid var(--border); border-top-color: var(--accent); border-radius: 50%; animation: spin 1s linear infinite; }
        @keyframes spin { to { transform: rotate(360deg); } }
        .error-message { background: rgba(239, 68, 68, 0.1); border: 1px solid var(--danger); border-radius: 8px; padding: 1rem; margin: 1rem 0; color: var(--danger); display: flex; align-items: center; gap: 0.75rem; }
        .toast { position: fixed; bottom: 2rem; right: 2rem; background: var(--bg-card); border: 1px solid var(--border); border-radius: 8px; padding: 1rem 1.5rem; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4); transform: translateX(400px); transition: transform 0.3s ease; z-index: 1000; display: flex; align-items: center; gap: 0.75rem; max-width: 400px; }
        .toast.show { transform: translateX(0); }
        .toast.success { border-color: var(--success); background: rgba(16, 185, 129, 0.1); }
        .toast.error { border-color: var(--danger); background: rgba(239, 68, 68, 0.1); }
        .online-indicator { width: 8px; height: 8px; border-radius: 50%; background: var(--online); display: inline-block; margin-right: 0.5rem; }
        .offline-indicator { width: 8px; height: 8px; border-radius: 50%; background: var(--offline); display: inline-block; margin-right: 0.5rem; }
        .instance-status-badge { padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
        .instance-status-badge.running { background: rgba(16, 185, 129, 0.2); color: var(--success); border: 1px solid var(--success); }
        .instance-status-badge.stopped { background: rgba(239, 68, 68, 0.2); color: var(--danger); border: 1px solid var(--danger); }
        .instance-status-badge.idle { background: rgba(245, 158, 11, 0.2); color: var(--idle); border: 1px solid var(--idle); }
        .instance-status-badge.mixed { background: rgba(168, 174, 192, 0.2); color: var(--text-secondary); border: 1px solid var(--text-secondary); }
        @media (max-width: 768px) { .header { padding: 1rem; } .main { padding: 1rem; } .instances-grid { grid-template-columns: 1fr; } .control-buttons { justify-content: stretch; } .btn { flex: 1; justify-content: center; } }
    </style>
</head>
<body>
    <div class=""app"">
        <header class=""header"">
            <div class=""header-content"">
                <div class=""logo"">
                    <h1>PokeBot Control Center</h1>
                    <div class=""status-indicator""></div>
                </div>
                <button class=""btn"" onclick=""refreshInstances()"">🔄 Refresh</button>
            </div>
        </header>
        <main class=""main"">
            <div class=""global-controls"">
                <h2>Global Controls - All Instances</h2>
                <div class=""control-buttons"">
                    <button class=""btn btn-success"" onclick=""sendGlobalCommand('start')"">▶️ Start All</button>
                    <button class=""btn btn-danger"" onclick=""sendGlobalCommand('stop')"">⏹️ Stop All</button>
                    <button class=""btn btn-warning"" onclick=""sendGlobalCommand('idle')"">⏸️ Idle All</button>
                    <button class=""btn"" onclick=""sendGlobalCommand('resume')"">⏯️ Resume All</button>
                    <button class=""btn"" onclick=""sendGlobalCommand('restart')"">🔄 Restart All</button>
                    <button class=""btn"" onclick=""sendGlobalCommand('reboot')"">🔌 Reboot All</button>
                    <button class=""btn"" onclick=""sendGlobalCommand('screenon')"">💡 Screen On</button>
                    <button class=""btn"" onclick=""sendGlobalCommand('screenoff')"">🌙 Screen Off</button>
                </div>
            </div>
            <div id=""instances-container"" class=""instances-grid"">
                <div class=""loading""><div class=""spinner""></div></div>
            </div>
        </main>
    </div>
    <div id=""toast"" class=""toast"">
        <span class=""toast-icon""></span>
        <div class=""toast-content"">
            <div class=""toast-title""></div>
            <div class=""toast-message""></div>
        </div>
    </div>
    <script>
        const API_BASE = '/api/bot';
        let instances = [];
        let refreshInterval;

        document.addEventListener('DOMContentLoaded', () => {
            refreshInstances();
            refreshInterval = setInterval(refreshInstances, 5000);
        });

        window.addEventListener('beforeunload', () => {
            if (refreshInterval) clearInterval(refreshInterval);
        });

        async function refreshInstances() {
            try {
                const response = await fetch(`${API_BASE}/instances`);
                if (!response.ok) throw new Error('Failed to fetch instances');
                
                const data = await response.json();
                instances = data.Instances;
                renderInstances();
            } catch (error) {
                console.error('Error fetching instances:', error);
                showError('Failed to load bot instances. Make sure the bot is running.');
            }
        }

        function renderInstances() {
            const container = document.getElementById('instances-container');
            
            if (instances.length === 0) {
                container.innerHTML = '<div class=""error-message"">⚠️ No bot instances found. Make sure at least one PokeBot is running.</div>';
                return;
            }

            container.innerHTML = instances.map(instance => {
                const isOnline = instance.IsOnline || false;
                const statusClass = isOnline ? 'online' : 'offline';
                const statusIndicator = isOnline ? 
                    '<span class=""online-indicator""></span>Connected' : 
                    '<span class=""offline-indicator""></span>Disconnected';
                
                let instanceStatus = 'stopped';
                let instanceStatusText = 'Stopped';
                if (instance.BotStatuses && instance.BotStatuses.length > 0) {
                    const runningCount = instance.BotStatuses.filter(b => 
                        b.Status.toUpperCase().includes('RUNNING') || 
                        b.Status.toUpperCase().includes('ACTIVE') ||
                        (!b.Status.toUpperCase().includes('IDLE') && 
                         !b.Status.toUpperCase().includes('STOPPED') && 
                         !b.Status.toUpperCase().includes('ERROR'))
                    ).length;
                    const idleCount = instance.BotStatuses.filter(b => 
                        b.Status.toUpperCase().includes('IDLE')
                    ).length;
                    
                    if (runningCount === instance.BotStatuses.length) {
                        instanceStatus = 'running';
                        instanceStatusText = 'All Running';
                    } else if (idleCount === instance.BotStatuses.length) {
                        instanceStatus = 'idle';
                        instanceStatusText = 'All Idle';
                    } else if (runningCount > 0) {
                        instanceStatus = 'mixed';
                        instanceStatusText = `${runningCount}/${instance.BotStatuses.length} Running`;
                    } else if (idleCount > 0) {
                        instanceStatus = 'idle';
                        instanceStatusText = 'Idle';
                    }
                }
                
                return `
                <div class=""instance-card ${statusClass}"" data-port=""${instance.Port}"">
                    <div class=""instance-header"">
                        <h3 class=""instance-title"">
                            ${instance.Name}
                            <span class=""instance-status-badge ${instanceStatus}"">${instanceStatusText}</span>
                        </h3>
                        <span class=""instance-badge"">Port ${instance.Port}</span>
                    </div>
                    <div class=""instance-body"">
                        <div class=""instance-info"">
                            <div class=""info-item"">
                                <span class=""info-label"">Version</span>
                                <span class=""info-value"">${instance.Version}</span>
                            </div>
                            <div class=""info-item"">
                                <span class=""info-label"">Mode</span>
                                <span class=""info-value"">${instance.Mode}</span>
                            </div>
                            <div class=""info-item"">
                                <span class=""info-label"">Process ID</span>
                                <span class=""info-value"">${instance.ProcessId}</span>
                            </div>
                            <div class=""info-item"">
                                <span class=""info-label"">Connection</span>
                                <span class=""info-value"">${statusIndicator}</span>
                            </div>
                        </div>
                        
                        ${instance.BotStatuses && instance.BotStatuses.length > 0 ? `
                        <div class=""bot-status"">
                            <div class=""info-label"" style=""margin-bottom: 0.5rem;"">BOTS (${instance.BotStatuses.length})</div>
                            ${instance.BotStatuses.map((bot, index) => `
                                <div class=""bot-status-item"">
                                    <span class=""bot-name"">
                                        <span style=""color: ${getStatusColor(bot.Status)};"">●</span>
                                        ${bot.Name || `Bot ${index + 1}`}
                                    </span>
                                    <span class=""bot-state ${getStatusClass(bot.Status)}"">${bot.Status}</span>
                                </div>
                            `).join('')}
                        </div>
                        ` : instance.BotCount > 0 ? `
                        <div class=""bot-status"">
                            <div class=""info-label"">BOTS</div>
                            <div class=""bot-status-item"">
                                <span class=""bot-name"">Bot Count: ${instance.BotCount}</span>
                                <span class=""bot-state"">Status Unknown</span>
                            </div>
                        </div>
                        ` : ''}
                        
                        <div class=""control-buttons"">
                            <button class=""btn btn-sm"" onclick=""sendInstanceCommand(${instance.Port}, 'start')"" ${!isOnline ? 'disabled' : ''}>Start</button>
                            <button class=""btn btn-sm"" onclick=""sendInstanceCommand(${instance.Port}, 'stop')"" ${!isOnline ? 'disabled' : ''}>Stop</button>
                            <button class=""btn btn-sm"" onclick=""sendInstanceCommand(${instance.Port}, 'idle')"" ${!isOnline ? 'disabled' : ''}>Idle</button>
                            <button class=""btn btn-sm"" onclick=""sendInstanceCommand(${instance.Port}, 'resume')"" ${!isOnline ? 'disabled' : ''}>Resume</button>
                        </div>
                    </div>
                </div>
            `;
            }).join('');
        }

        function getStatusColor(status) {
            const upperStatus = status?.toUpperCase() || '';
            if (upperStatus.includes('RUNNING') || upperStatus.includes('ACTIVE') || upperStatus === 'ONLINE' ||
                (!upperStatus.includes('IDLE') && !upperStatus.includes('STOPPED') && !upperStatus.includes('ERROR') && !upperStatus.includes('UNKNOWN'))) {
                return '#10b981';
            } else if (upperStatus.includes('IDLE') || upperStatus.includes('PAUSED')) {
                return '#f59e0b';
            } else if (upperStatus.includes('STOPPED') || upperStatus.includes('OFFLINE') || upperStatus.includes('DISCONNECTED')) {
                return '#ef4444';
            } else if (upperStatus.includes('ERROR')) {
                return '#ef4444';
            } else {
                return '#6b7280';
            }
        }

        function getStatusClass(status) {
            const upperStatus = status?.toUpperCase() || '';
            if (upperStatus.includes('RUNNING') || upperStatus.includes('ACTIVE') || 
                (!upperStatus.includes('IDLE') && !upperStatus.includes('STOPPED') && !upperStatus.includes('ERROR') && !upperStatus.includes('UNKNOWN'))) {
                return 'running';
            } else if (upperStatus.includes('IDLE')) {
                return 'idle';
            } else if (upperStatus.includes('STOPPED') || upperStatus.includes('ERROR')) {
                return 'stopped';
            } else {
                return 'error';
            }
        }

        async function sendGlobalCommand(command) {
            showToast('info', 'Sending Command', `Sending ${command} to all instances...`);
            
            try {
                const response = await fetch(`${API_BASE}/command/all`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ Command: command })
                });

                if (!response.ok) throw new Error('Command failed');
                
                const result = await response.json();
                const successCount = result.SuccessfulCommands || 0;
                const totalCount = result.TotalInstances || 0;
                
                if (successCount === totalCount && totalCount > 0) {
                    showToast('success', 'Command Sent', `Successfully sent ${command} to all ${totalCount} instances`);
                } else if (successCount > 0) {
                    showToast('warning', 'Partial Success', `Command sent to ${successCount} of ${totalCount} instances`);
                } else {
                    showToast('error', 'Command Failed', `Failed to send command to any instances`);
                }
                
                setTimeout(refreshInstances, 1000);
            } catch (error) {
                console.error('Error sending global command:', error);
                showToast('error', 'Error', `Failed to send command: ${command}`);
            }
        }

        async function sendInstanceCommand(port, command) {
            showToast('info', 'Sending Command', `Sending ${command} to instance on port ${port}...`);
            
            try {
                const response = await fetch(`${API_BASE}/instances/${port}/command`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ Command: command })
                });

                if (!response.ok) throw new Error('Command failed');
                
                const result = await response.json();
                if (result.Success) {
                    showToast('success', 'Command Sent', `Successfully sent ${command} to instance on port ${port}`);
                } else {
                    showToast('error', 'Command Failed', result.Message || 'Unknown error');
                }
                
                setTimeout(refreshInstances, 1000);
            } catch (error) {
                console.error(`Error sending command to port ${port}:`, error);
                showToast('error', 'Error', `Failed to send command to instance on port ${port}`);
            }
        }

        function showError(message) {
            console.error(message);
            showToast('error', 'Error', message);
        }

        function showToast(type, title, message) {
            const toast = document.getElementById('toast');
            const icon = toast.querySelector('.toast-icon');
            const titleEl = toast.querySelector('.toast-title');
            const messageEl = toast.querySelector('.toast-message');
            
            titleEl.textContent = title;
            messageEl.textContent = message;
            
            toast.className = 'toast';
            switch(type) {
                case 'success':
                    icon.textContent = '✅';
                    toast.classList.add('success');
                    break;
                case 'error':
                    icon.textContent = '❌';
                    toast.classList.add('error');
                    break;
                case 'warning':
                    icon.textContent = '⚠️';
                    break;
                case 'info':
                default:
                    icon.textContent = 'ℹ️';
                    break;
            }
            
            toast.classList.add('show');
            
            setTimeout(() => {
                toast.classList.remove('show');
            }, 4000);
        }
    </script>
</body>
</html>";

    public BotServer(Main mainForm, int port = 8080, int tcpPort = 8081)
    {
        _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        _port = port;
        _tcpPort = tcpPort;
    }

    public void Start()
    {
        if (_running) return;

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
            _listener.Start();
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

        // Get mode from config, not window title
        var mode = config?.Mode.ToString() ?? "Unknown";
        var name = "PokeBot";

        // Get version from TradeBot.Version
        var version = "Unknown";
        try
        {
            var tradeBotType = Type.GetType("SysBot.Pokemon.Helpers.TradeBot, SysBot.Pokemon");
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
            BotStatuses = botStatuses
        };
    }

    private static List<BotInstance> ScanRemoteInstances()
    {
        var instances = new List<BotInstance>();
        var currentPid = Environment.ProcessId;

        try
        {
            var processes = Process.GetProcessesByName("PokeBot")
                .Where(p => p.Id != currentPid);

            foreach (var process in processes)
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

            var portFile = Path.Combine(Path.GetDirectoryName(exePath)!, $"PokeBot_{process.Id}.port");
            if (!File.Exists(portFile))
                return null;

            var portText = File.ReadAllText(portFile).Trim();
            if (portText.StartsWith("ERROR:") || !int.TryParse(portText, out var port))
                return null;

            var isOnline = IsPortOpen(port);
            var instance = new BotInstance
            {
                ProcessId = process.Id,
                Name = "PokeBot",
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

            // Run on local instance
            var localResult = JsonSerializer.Deserialize<CommandResponse>(RunLocalCommand(commandRequest.Command));
            if (localResult != null)
            {
                localResult.InstanceName = _mainForm.Text;
                results.Add(localResult);
            }

            // Run on remote instances
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

            _mainForm.BeginInvoke((MethodInvoker)(() =>
            {
                var sendAllMethod = _mainForm.GetType().GetMethod("SendAll",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                sendAllMethod?.Invoke(_mainForm, [cmd]);
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

    private static string QueryRemote(int port, string command)
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

        return [];
    }

    private ProgramConfig? GetConfig()
    {
        var configProp = _mainForm.GetType().GetProperty("Config",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return configProp?.GetValue(_mainForm) as ProgramConfig;
    }

    private static string GetBotName(PokeBotState state, ProgramConfig? config)
    {
        // Always return IP address as the bot name
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
