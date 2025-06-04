using SysBot.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysBot.Pokemon.WinForms
{
    public partial class BotController : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PokeBotState State { get; private set; } = new();
        private IPokeBotRunner? Runner;
        public EventHandler? Remove;

        // Animation state
        private float hoverProgress = 0f;
        private float progressValue = 0f;
        private bool isHovering = false;
        private DateTime animationStart = DateTime.Now;
        private Color currentStatusColor = Color.FromArgb(87, 242, 135); // Green
        private Color targetStatusColor = Color.FromArgb(87, 242, 135);
        private DateTime LastUpdateStatus = DateTime.Now;

        public BotController()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            ConfigureContextMenu();
            ConfigureChildControls();
        }

        private void ConfigureContextMenu()
        {
            var opt = (BotControlCommand[])Enum.GetValues(typeof(BotControlCommand));

            // Create modern styled menu renderer
            RCMenu.Renderer = new ModernMenuRenderer();

            for (int i = 1; i < opt.Length; i++)
            {
                var cmd = opt[i];
                var item = new ToolStripMenuItem(cmd.ToString())
                {
                    ForeColor = Color.FromArgb(224, 224, 224),
                    BackColor = Color.FromArgb(35, 35, 35)
                };
                item.Click += (_, __) => SendCommand(cmd);

                // Add icon based on command
                switch (cmd)
                {
                    case BotControlCommand.Start:
                        item.Text = "▶ " + item.Text;
                        break;
                    case BotControlCommand.Stop:
                        item.Text = "■ " + item.Text;
                        break;
                    case BotControlCommand.Idle:
                        item.Text = "⏸ " + item.Text;
                        break;
                    case BotControlCommand.Resume:
                        item.Text = "⏵ " + item.Text;
                        break;
                    case BotControlCommand.Restart:
                        item.Text = "↻ " + item.Text;
                        break;
                    case BotControlCommand.RebootAndStop:
                        item.Text = "⚡ " + item.Text;
                        break;
                    case BotControlCommand.ScreenOnAll:
                        item.Text = "💡 " + item.Text;
                        break;
                    case BotControlCommand.ScreenOffAll:
                        item.Text = "🌙 " + item.Text;
                        break;
                }

                RCMenu.Items.Add(item);
            }

            // Add separator
            RCMenu.Items.Add(new ToolStripSeparator());

            var remove = new ToolStripMenuItem("✕ Remove")
            {
                ForeColor = Color.FromArgb(237, 66, 69),
                BackColor = Color.FromArgb(35, 35, 35)
            };
            remove.Click += (_, __) => TryRemove();
            RCMenu.Items.Add(remove);
            RCMenu.Opening += RcMenuOnOpening;
        }

        private void ConfigureChildControls()
        {
            var controls = Controls;
            foreach (var c in controls.OfType<Control>())
            {
                c.MouseEnter += BotController_MouseEnter;
                c.MouseLeave += BotController_MouseLeave;
            }
        }

        private void RcMenuOnOpening(object? sender, CancelEventArgs? e)
        {
            if (Runner == null)
                return;

            var bot = Runner.GetBot(State);
            if (bot is null)
                return;

            foreach (var tsi in RCMenu.Items.OfType<ToolStripMenuItem>())
            {
                var text = tsi.Text.Replace("▶ ", "").Replace("■ ", "").Replace("⏸ ", "")
                    .Replace("⏵ ", "").Replace("↻ ", "").Replace("⚡ ", "").Replace("✕ ", "")
                    .Replace("💡 ", "").Replace("🌙 ", "");
                tsi.Enabled = Enum.TryParse(text, out BotControlCommand cmd)
                    ? cmd.IsUsable(bot.IsRunning, bot.IsPaused)
                    : !bot.IsRunning;
            }
        }

        public void Initialize(IPokeBotRunner runner, PokeBotState cfg)
        {
            Runner = runner;
            State = cfg;
            ReloadStatus();
            L_Description.Text = "Initializing...";
        }

        public void ReloadStatus()
        {
            var bot = GetBot().Bot;
            L_Left.Text = $"{bot.Connection.Name}\n{State.InitialRoutine}";
        }

        public void ReloadStatus(BotSource<PokeBotState> b)
        {
            ReloadStatus();
            var bot = b.Bot;
            L_Description.Text = $"[{bot.LastTime:HH:mm:ss}] {bot.Connection.Label}: {bot.LastLogged}";
            L_Left.Text = $"{bot.Connection.Name}\n{State.InitialRoutine}";

            var lastTime = bot.LastTime;
            if (!b.IsRunning)
            {
                targetStatusColor = Color.FromArgb(100, 100, 100); // Gray
                progressValue = 0;
                return;
            }

            if (!b.Bot.Connection.Connected)
            {
                targetStatusColor = Color.FromArgb(135, 206, 250); // Aqua
                progressValue = 0.3f;
                return;
            }

            var cfg = bot.Config;
            if (cfg.CurrentRoutineType == PokeRoutineType.Idle && cfg.NextRoutineType == PokeRoutineType.Idle)
            {
                targetStatusColor = Color.FromArgb(254, 231, 92); // Yellow
                progressValue = 0.5f;
                return;
            }

            if (LastUpdateStatus == lastTime)
                return;

            // Color decay from Green based on time
            const int threshold = 100;
            Color good = Color.FromArgb(87, 242, 135); // Green
            if (cfg.Connection.Protocol == SwitchProtocol.USB)
                good = Color.FromArgb(88, 101, 242); // Blue for USB
            Color bad = Color.FromArgb(237, 66, 69); // Red

            var delta = DateTime.Now - lastTime;
            var seconds = delta.Seconds;

            LastUpdateStatus = lastTime;
            if (seconds > 2 * threshold)
                return;

            if (seconds > threshold)
            {
                targetStatusColor = bad;
                progressValue = 0.1f;
            }
            else
            {
                var factor = seconds / (double)threshold;
                targetStatusColor = Blend(bad, good, factor * factor);
                progressValue = (float)(1.0 - factor);
            }
        }

        private static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
            byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
            byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
            return Color.FromArgb(r, g, b);
        }

        public void TryRemove()
        {
            var bot = GetBot();
            if (!Runner!.Config.SkipConsoleBotCreation)
                bot.Stop();

            Remove?.Invoke(this, EventArgs.Empty);
        }

        public void SendCommand(BotControlCommand cmd, bool echo = true)
        {
            if (Runner?.Config.SkipConsoleBotCreation != false)
            {
                LogUtil.LogError("No bots were created because SkipConsoleBotCreation is on!", "Hub");
                return;
            }
            var bot = GetBot();
            if (bot == null)
            {
                LogUtil.LogError("Bot is null!", "BotController");
                return;
            }

            switch (cmd)
            {
                case BotControlCommand.Idle:
                    bot.Pause();
                    break;
                case BotControlCommand.Start:
                    Runner.InitializeStart();
                    bot.Start();
                    break;
                case BotControlCommand.Stop:
                    bot.Stop();
                    break;
                case BotControlCommand.Resume:
                    bot.Resume();
                    break;
                case BotControlCommand.RebootAndStop:
                    bot.RebootAndStop();
                    break;
                case BotControlCommand.Restart:
                    {
                        var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Are you sure you want to restart the connection?");
                        if (prompt != DialogResult.Yes)
                            return;

                        Runner.InitializeStart();
                        bot.Bot.Connection.Reset();
                        bot.Start();
                        break;
                    }
                case BotControlCommand.ScreenOnAll:
                    ExecuteScreenCommand(true);
                    break;
                case BotControlCommand.ScreenOffAll:
                    ExecuteScreenCommand(false);
                    break;
                default:
                    WinFormsUtil.Alert($"{cmd} is not a command that can be sent to the Bot.");
                    return;
            }
        }

        private void ExecuteScreenCommand(bool screenOn)
        {
            if (Runner == null)
            {
                LogUtil.LogError("Runner is null - cannot execute screen command", "BotController");
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    var bots = Runner.Bots;
                    if (bots == null || bots.Count == 0)
                    {
                        LogUtil.LogError("No bots available to execute screen command", "BotController");
                        return;
                    }

                    int successCount = 0;
                    int totalCount = bots.Count;

                    foreach (var botSource in bots)
                    {
                        try
                        {
                            var bot = botSource.Bot;
                            if (bot?.Connection != null && bot.Connection.Connected)
                            {
                                var crlf = bot is SwitchRoutineExecutor<PokeBotState> { UseCRLF: true };
                                await bot.Connection.SendAsync(SwitchCommand.SetScreen(screenOn ? ScreenState.On : ScreenState.Off, crlf), CancellationToken.None);
                                successCount++;
                                LogUtil.LogInfo($"Screen turned {(screenOn ? "ON" : "OFF")} for {bot.Connection.Name}", "BotController");
                            }
                            else
                            {
                                LogUtil.LogError($"Cannot send screen command - bot {bot?.Connection?.Name ?? "unknown"} is not connected", "BotController");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtil.LogError($"Failed to send screen command to bot: {ex.Message}", "BotController");
                        }
                    }

                    LogUtil.LogInfo($"Screen command sent to {successCount} of {totalCount} bots", "BotController");
                }
                catch (Exception ex)
                {
                    LogUtil.LogError($"Failed to execute screen command for all bots: {ex.Message}", "BotController");
                }
            });
        }

        public string ReadBotState()
        {
            try
            {
                var botSource = GetBot();
                if (botSource == null)
                    return "ERROR";

                var bot = botSource.Bot;
                if (bot == null)
                    return "ERROR";

                // Check if bot is stopped first - this is the key fix
                if (!botSource.IsRunning)
                    return "STOPPED";

                if (botSource.IsStopping)
                    return "STOPPING";

                if (botSource.IsPaused)
                {
                    if (bot.Config?.CurrentRoutineType != PokeRoutineType.Idle)
                        return "IDLING";
                    else
                        return "IDLE";
                }

                if (botSource.IsRunning && !bot.Connection.Connected)
                    return "REBOOTING";

                var cfg = bot.Config;
                if (cfg == null)
                    return "UNKNOWN";

                if (cfg.CurrentRoutineType == PokeRoutineType.Idle)
                    return "IDLE";

                // Only return the routine type if the bot is actually running
                if (botSource.IsRunning && bot.Connection.Connected)
                    return cfg.CurrentRoutineType.ToString();

                return "UNKNOWN";
            }
            catch (Exception ex)
            {
                LogUtil.LogError($"Error reading bot state: {ex.Message}", "BotController");
                return "ERROR";
            }
        }

        public BotSource<PokeBotState> GetBot()
        {
            try
            {
                if (Runner == null)
                    return null;

                var bot = Runner.GetBot(State);
                if (bot == null)
                    return null;

                return bot;
            }
            catch (Exception ex)
            {
                LogUtil.LogError($"Error getting bot: {ex.Message}", "BotController");
                return null;
            }
        }

        private void BotController_MouseEnter(object? sender, EventArgs e)
        {
            isHovering = true;
            animationStart = DateTime.Now;
        }

        private void BotController_MouseLeave(object? sender, EventArgs e)
        {
            isHovering = false;
            animationStart = DateTime.Now;
        }

        public void ReadState()
        {
            var bot = GetBot();

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => ReloadStatus(bot)));
            }
            else
            {
                ReloadStatus(bot);
            }
        }

        // Paint event handlers
        private void BotController_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            // Background with gradient
            using (var path = new GraphicsPath())
            {
                GraphicsExtensions.AddRoundedRectangle(path, rect, 8);

                // Base gradient
                using (var brush = new LinearGradientBrush(rect,
                    Color.FromArgb(35, 35, 35),
                    Color.FromArgb(28, 28, 28),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Hover glow effect
                if (hoverProgress > 0)
                {
                    using (var glowBrush = new SolidBrush(Color.FromArgb((int)(20 * hoverProgress), 88, 101, 242)))
                    {
                        g.FillPath(glowBrush, path);
                    }

                    // Border glow
                    using (var pen = new Pen(Color.FromArgb((int)(100 * hoverProgress), 88, 101, 242), 2))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Normal border
                using (var pen = new Pen(Color.FromArgb(50, 50, 50), 1))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void StatusPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Simple static glow around the status lamp
            var glowRect = new Rectangle(5, 5, 40, 40);
            using (var glowBrush = new SolidBrush(Color.FromArgb(20, currentStatusColor)))
            {
                g.FillEllipse(glowBrush, glowRect);
            }
        }

        private void PB_Lamp_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = PB_Lamp.ClientRectangle;

            // Main status circle with gradient
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (var brush = new LinearGradientBrush(rect,
                    ControlPaint.Light(currentStatusColor, 0.3f),
                    currentStatusColor,
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillPath(brush, path);
                }

                // Inner highlight
                var highlightRect = new Rectangle(2, 2, 8, 8);
                using (var highlightBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                {
                    g.FillEllipse(highlightBrush, highlightRect);
                }
            }
        }

        private void ProgressBar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = progressBar.ClientRectangle;

            // Background track
            using (var brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                g.FillRectangle(brush, rect);
            }

            // Progress fill
            if (progressValue > 0)
            {
                var fillWidth = (int)(rect.Width * progressValue);
                var fillRect = new Rectangle(0, 0, fillWidth, rect.Height);

                using (var brush = new LinearGradientBrush(fillRect,
                    Color.FromArgb(88, 101, 242),
                    Color.FromArgb(87, 242, 135),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(brush, fillRect);
                }

                // Glow effect
                using (var glowBrush = new SolidBrush(Color.FromArgb(50, 87, 242, 135)))
                {
                    g.FillRectangle(glowBrush, fillRect.X, fillRect.Y - 2, fillRect.Width, fillRect.Height + 4);
                }
            }
        }

        private void ActionButton_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var btn = sender as System.Windows.Forms.Button;
            var rect = btn.ClientRectangle;

            // Draw rounded button with glow
            using (var path = new GraphicsPath())
            {
                GraphicsExtensions.AddRoundedRectangle(path, rect, 4);

                // Glow effect
                if (btn.ClientRectangle.Contains(btn.PointToClient(MousePosition)))
                {
                    for (int i = 3; i > 0; i--)
                    {
                        var glowRect = new Rectangle(-i, -i, rect.Width + i * 2, rect.Height + i * 2);
                        using (var glowPath = new GraphicsPath())
                        {
                            GraphicsExtensions.AddRoundedRectangle(glowPath, glowRect, 4);
                            using (var glowBrush = new SolidBrush(Color.FromArgb(20 / i, btn.BackColor)))
                            {
                                g.FillPath(glowBrush, glowPath);
                            }
                        }
                    }
                }
            }
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            RCMenu.Show(actionButton, new Point(0, actionButton.Height));
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Update hover animation
            var elapsed = (DateTime.Now - animationStart).TotalMilliseconds;
            var duration = 150.0;

            if (isHovering)
            {
                hoverProgress = Math.Min(1.0f, (float)(elapsed / duration));
            }
            else
            {
                hoverProgress = Math.Max(0.0f, 1.0f - (float)(elapsed / duration));
            }

            // Smoothly transition status color
            if (currentStatusColor != targetStatusColor)
            {
                currentStatusColor = Blend(targetStatusColor, currentStatusColor, 0.1);
                PB_Lamp.BackColor = currentStatusColor;
            }

            // Redraw if animating
            if (hoverProgress > 0 && hoverProgress < 1)
            {
                Invalidate();
            }

            statusPanel.Invalidate();
            PB_Lamp.Invalidate();
            progressBar.Invalidate();
        }

        // Custom menu renderer for dark theme
        private class ModernMenuRenderer : ToolStripProfessionalRenderer
        {
            public ModernMenuRenderer() : base(new ModernColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rc = new Rectangle(Point.Empty, e.Item.Size);
                var c = e.Item.Selected ? Color.FromArgb(50, 50, 50) : Color.FromArgb(35, 35, 35);
                using (var brush = new SolidBrush(c))
                    e.Graphics.FillRectangle(brush, rc);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? Color.FromArgb(224, 224, 224) : Color.FromArgb(100, 100, 100);
                base.OnRenderItemText(e);
            }
        }

        private class ModernColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(50, 50, 50);
            public override Color MenuItemBorder => Color.FromArgb(88, 101, 242);
            public override Color MenuBorder => Color.FromArgb(50, 50, 50);
            public override Color ToolStripDropDownBackground => Color.FromArgb(35, 35, 35);
            public override Color ImageMarginGradientBegin => Color.FromArgb(35, 35, 35);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(35, 35, 35);
            public override Color ImageMarginGradientEnd => Color.FromArgb(35, 35, 35);
            public override Color SeparatorDark => Color.FromArgb(50, 50, 50);
            public override Color SeparatorLight => Color.FromArgb(60, 60, 60);
        }
    }

    public enum BotControlCommand
    {
        None,
        Start,
        Stop,
        Idle,
        Resume,
        Restart,
        RebootAndStop,
        ScreenOnAll,
        ScreenOffAll,
    }

    public static class BotControlCommandExtensions
    {
        public static bool IsUsable(this BotControlCommand cmd, bool running, bool paused)
        {
            return cmd switch
            {
                BotControlCommand.Start => !running,
                BotControlCommand.Stop => running,
                BotControlCommand.Idle => running && !paused,
                BotControlCommand.Resume => paused,
                BotControlCommand.Restart => true,
                BotControlCommand.ScreenOnAll => running, // Only when running
                BotControlCommand.ScreenOffAll => running, // Only when running
                _ => false,
            };
        }
    }
}
