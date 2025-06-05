using SysBot.Pokemon.WinForms.Properties;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System;
using SysBot.Base;
using System.Runtime.InteropServices;

namespace SysBot.Pokemon.WinForms
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing && trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));

            animationTimer = new System.Windows.Forms.Timer(this.components);
            animationTimer.Interval = 32;
            animationTimer.Tick += AnimationTimer_Tick;

            trayIcon = new NotifyIcon(this.components);
            trayContextMenu = new ContextMenuStrip(this.components);
            trayMenuShow = new ToolStripMenuItem();
            trayMenuExit = new ToolStripMenuItem();

            mainLayoutPanel = new TableLayoutPanel();
            sidebarPanel = new Panel();
            contentPanel = new Panel();
            headerPanel = new Panel();

            logoPanel = new Panel();
            navButtonsPanel = new FlowLayoutPanel();
            btnNavBots = new Button();
            btnNavHub = new Button();
            btnNavLogs = new Button();
            sidebarBottomPanel = new Panel();
            btnUpdate = new Button();
            statusIndicator = new Panel();

            titleLabel = new Label();
            controlButtonsPanel = new FlowLayoutPanel();
            btnStart = new Button();
            btnStop = new Button();
            btnReboot = new Button();

            botsPanel = new Panel();
            hubPanel = new Panel();
            logsPanel = new Panel();

            botHeaderPanel = new Panel();
            addBotPanel = new Panel();
            TB_IP = new TextBox();
            NUD_Port = new NumericUpDown();
            CB_Protocol = new ComboBox();
            CB_Routine = new ComboBox();
            B_New = new Button();
            FLP_Bots = new FlowLayoutPanel();

            PG_Hub = new PropertyGrid();

            RTB_Logs = new RichTextBox();
            logsHeaderPanel = new Panel();
            searchPanel = new Panel();
            logSearchBox = new TextBox();
            searchOptionsPanel = new FlowLayoutPanel();
            btnCaseSensitive = new CheckBox();
            btnRegex = new CheckBox();
            btnWholeWord = new CheckBox();
            btnClearLogs = new Button();
            searchStatusLabel = new Label();

            comboBox1 = new ComboBox();

            mainLayoutPanel.SuspendLayout();
            sidebarPanel.SuspendLayout();
            navButtonsPanel.SuspendLayout();
            sidebarBottomPanel.SuspendLayout();
            headerPanel.SuspendLayout();
            controlButtonsPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            botsPanel.SuspendLayout();
            botHeaderPanel.SuspendLayout();
            addBotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).BeginInit();
            hubPanel.SuspendLayout();
            logsPanel.SuspendLayout();
            logsHeaderPanel.SuspendLayout();
            searchPanel.SuspendLayout();
            searchOptionsPanel.SuspendLayout();
            SuspendLayout();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(1100, 600);
            BackColor = Color.FromArgb(23, 26, 33);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Icon = Resources.icon;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PokéBot Control Center";
            FormClosing += Main_FormClosing;
            DoubleBuffered = true;
            Resize += Main_Resize;

            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(sidebarPanel, 0, 0);
            mainLayoutPanel.Controls.Add(contentPanel, 1, 0);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Margin = new Padding(0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 1;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.TabIndex = 0;
            mainLayoutPanel.BackColor = Color.Transparent;
            EnableDoubleBuffering(mainLayoutPanel);

            sidebarPanel.BackColor = Color.FromArgb(18, 18, 18);
            sidebarPanel.Controls.Add(navButtonsPanel);
            sidebarPanel.Controls.Add(sidebarBottomPanel);
            sidebarPanel.Controls.Add(logoPanel);
            sidebarPanel.Dock = DockStyle.Fill;
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Margin = new Padding(0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(260, 720);
            sidebarPanel.TabIndex = 0;
            EnableDoubleBuffering(sidebarPanel);

            logoPanel.BackColor = Color.FromArgb(15, 15, 15);
            logoPanel.Dock = DockStyle.Top;
            logoPanel.Height = 100;
            logoPanel.Location = new Point(0, 0);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new Size(260, 100);
            logoPanel.TabIndex = 2;
            logoPanel.Paint += LogoPanel_Paint;
            EnableDoubleBuffering(logoPanel);

            navButtonsPanel.AutoSize = false;
            navButtonsPanel.Controls.Add(btnNavBots);
            navButtonsPanel.Controls.Add(btnNavHub);
            navButtonsPanel.Controls.Add(btnNavLogs);
            navButtonsPanel.Dock = DockStyle.Fill;
            navButtonsPanel.FlowDirection = FlowDirection.TopDown;
            navButtonsPanel.Location = new Point(0, 100);
            navButtonsPanel.Margin = new Padding(0);
            navButtonsPanel.Name = "navButtonsPanel";
            navButtonsPanel.Padding = new Padding(0, 40, 0, 0);
            navButtonsPanel.Size = new Size(260, 540);
            navButtonsPanel.TabIndex = 1;
            navButtonsPanel.BackColor = Color.Transparent;
            EnableDoubleBuffering(navButtonsPanel);

            ConfigureNavButton(btnNavBots, "BOTS", 0, "Manage bot connections");
            ConfigureNavButton(btnNavHub, "CONFIGURATION", 1, "System settings");
            ConfigureNavButton(btnNavLogs, "SYSTEM LOGS", 2, "View activity logs");

            var separator = new Panel();
            separator.BackColor = Color.FromArgb(50, 50, 50);
            separator.Size = new Size(220, 1);
            separator.Margin = new Padding(20, 20, 20, 20);
            navButtonsPanel.Controls.Add(separator);

            var btnExit = new Button();
            ConfigureNavButton(btnExit, "EXIT", 3, "Exit application");
            btnExit.ForeColor = Color.FromArgb(237, 66, 69);
            btnExit.Click += (s, e) => {
                var result = MessageBox.Show(
                    "Are you sure you want to exit PokéBot?",
                    "Exit Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _isReallyClosing = true;
                    Close();
                }
            };
            navButtonsPanel.Controls.Add(btnExit);

            sidebarBottomPanel.Controls.Add(btnUpdate);
            sidebarBottomPanel.Controls.Add(comboBox1);
            sidebarBottomPanel.Dock = DockStyle.Bottom;
            sidebarBottomPanel.Height = 100;
            sidebarBottomPanel.Location = new Point(0, 620);
            sidebarBottomPanel.Name = "sidebarBottomPanel";
            sidebarBottomPanel.Padding = new Padding(20, 10, 20, 20);
            sidebarBottomPanel.TabIndex = 0;
            sidebarBottomPanel.BackColor = Color.FromArgb(15, 15, 15);
            EnableDoubleBuffering(sidebarBottomPanel);

            comboBox1.BackColor = Color.FromArgb(30, 30, 30);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.Font = new Font("Segoe UI", 9F);
            comboBox1.ForeColor = Color.FromArgb(224, 224, 224);
            comboBox1.Location = new Point(20, 10);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(220, 23);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            btnUpdate.BackColor = Color.FromArgb(30, 30, 30);
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnUpdate.ForeColor = Color.FromArgb(176, 176, 176);
            btnUpdate.Location = new Point(20, 40);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(220, 40);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "CHECK FOR UPDATES";
            btnUpdate.UseVisualStyleBackColor = false;
            btnUpdate.Click += Updater_Click;
            btnUpdate.Cursor = Cursors.Hand;
            btnUpdate.Tag = new ButtonAnimationState();
            btnUpdate.Text = ""; // Clear text since we're drawing it manually
            ConfigureHoverAnimation(btnUpdate);

            // Configure status indicator after button is set up
            statusIndicator.BackColor = Color.FromArgb(100, 100, 100); // Default gray
            statusIndicator.Size = new Size(12, 12);
            statusIndicator.Location = new Point(190, 15); // Fixed position from left
            statusIndicator.Name = "statusIndicator";
            statusIndicator.Enabled = false; // Prevent mouse interaction
            statusIndicator.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to top-right
            CreateCircularRegion(statusIndicator);
            btnUpdate.Controls.Add(statusIndicator);
            statusIndicator.BringToFront();

            // Add paint handler for status indicator
            statusIndicator.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = statusIndicator.ClientRectangle;
                rect.Inflate(-1, -1);

                using var brush = new SolidBrush(statusIndicator.BackColor);
                e.Graphics.FillEllipse(brush, rect);

                // Add inner highlight if update available
                var mainForm = (Main)statusIndicator.FindForm();
                if (mainForm != null && mainForm.hasUpdate)
                {
                    var highlightRect = new Rectangle(2, 2, 4, 4);
                    using var highlightBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
                    e.Graphics.FillEllipse(highlightBrush, highlightRect);
                }
            };

            // Add tooltip
            var updateTooltip = new ToolTip();
            updateTooltip.SetToolTip(btnUpdate, "Check for updates");
            btnUpdate.MouseEnter += (s, e) => {
                // Access hasUpdate from main form context
                var mainForm = (Main)btnUpdate.FindForm();
                if (mainForm != null && mainForm.hasUpdate)
                {
                    updateTooltip.SetToolTip(btnUpdate, "Update available! Click to download.");
                }
                else
                {
                    updateTooltip.SetToolTip(btnUpdate, "No updates available");
                }
            };

            // Reposition status indicator on resize
            btnUpdate.Resize += (s, e) => {
                if (statusIndicator != null && btnUpdate.Controls.Contains(statusIndicator))
                {
                    statusIndicator.Location = new Point(btnUpdate.ClientSize.Width - 25, 15);
                }
            };
            btnUpdate.Layout += (s, e) => {
                if (statusIndicator != null && btnUpdate.Controls.Contains(statusIndicator))
                {
                    statusIndicator.Location = new Point(btnUpdate.ClientSize.Width - 25, 15);
                }
            };

            // Set initial position
            statusIndicator.Location = new Point(btnUpdate.ClientSize.Width - 25, 15);
            btnUpdate.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                var animState = btnUpdate.Tag as ButtonAnimationState;

                // Draw hover glow background - only if hovering
                if (animState != null && animState.HoverProgress > 0 && animState.IsHovering)
                {
                    using var glowBrush = new SolidBrush(Color.FromArgb((int)(20 * animState.HoverProgress), 88, 101, 242));
                    e.Graphics.FillRectangle(glowBrush, btnUpdate.ClientRectangle);
                }

                // Determine icon color with hover effect
                var iconColor = btnUpdate.ForeColor;
                if (animState != null && animState.HoverProgress > 0)
                {
                    iconColor = Color.FromArgb(
                        (int)(176 + (224 - 176) * animState.HoverProgress),
                        (int)(176 + (224 - 176) * animState.HoverProgress),
                        (int)(176 + (224 - 176) * animState.HoverProgress)
                    );
                }

                // Draw icon
                using var iconFont = new Font("Segoe MDL2 Assets", 14F);
                var iconText = "\uE895"; // Download/Update icon

                using var iconBrush = new SolidBrush(iconColor);
                var iconSize = e.Graphics.MeasureString(iconText, iconFont);

                // Position icon on the left
                var iconX = 20;
                var iconY = (btnUpdate.Height - iconSize.Height) / 2;
                e.Graphics.DrawString(iconText, iconFont, iconBrush, iconX, iconY);

                // Draw text after icon
                using var textFont = new Font("Segoe UI", 9F, FontStyle.Regular);
                var text = "CHECK FOR UPDATES";
                var textSize = e.Graphics.MeasureString(text, textFont);
                var textX = iconX + iconSize.Width + 10;
                var textY = (btnUpdate.Height - textSize.Height) / 2;
                e.Graphics.DrawString(text, textFont, iconBrush, textX, textY);

                // Draw glow around status indicator if update available
                var mainForm = (Main)btnUpdate.FindForm();
                if (mainForm != null && mainForm.hasUpdate && statusIndicator != null)
                {
                    var indicatorBounds = new Rectangle(
                        statusIndicator.Left - 3,
                        statusIndicator.Top - 3,
                        statusIndicator.Width + 6,
                        statusIndicator.Height + 6
                    );

                    // Multi-layer glow
                    for (int i = 3; i > 0; i--)
                    {
                        var glowAlpha = (int)(20 / i * (0.5 + 0.5 * Math.Sin(mainForm.pulsePhase)));
                        using var glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, 87, 242, 135));
                        var glowRect = new Rectangle(
                            indicatorBounds.X - i * 2,
                            indicatorBounds.Y - i * 2,
                            indicatorBounds.Width + i * 4,
                            indicatorBounds.Height + i * 4
                        );
                        e.Graphics.FillEllipse(glowBrush, glowRect);
                    }
                }
            };

            contentPanel.BackColor = Color.FromArgb(28, 28, 28);
            contentPanel.Controls.Add(botsPanel);
            contentPanel.Controls.Add(hubPanel);
            contentPanel.Controls.Add(logsPanel);
            contentPanel.Controls.Add(headerPanel);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(260, 0);
            contentPanel.Margin = new Padding(0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1020, 720);
            contentPanel.TabIndex = 1;
            EnableDoubleBuffering(contentPanel);

            headerPanel.BackColor = Color.FromArgb(28, 28, 28);
            headerPanel.Controls.Add(controlButtonsPanel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1020, 100);
            headerPanel.TabIndex = 3;
            headerPanel.Paint += HeaderPanel_Paint;
            headerPanel.Resize += HeaderPanel_Resize;
            EnableDoubleBuffering(headerPanel);

            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(224, 224, 224);
            titleLabel.Location = new Point(40, 25);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(250, 45);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Bot Management";

            controlButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlButtonsPanel.AutoSize = true;
            controlButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlButtonsPanel.Controls.Add(btnStart);
            controlButtonsPanel.Controls.Add(btnStop);
            controlButtonsPanel.Controls.Add(btnReboot);
            controlButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            controlButtonsPanel.Location = new Point(contentPanel.Width - 520, 30);
            controlButtonsPanel.Name = "controlButtonsPanel";
            controlButtonsPanel.Size = new Size(480, 40);
            controlButtonsPanel.TabIndex = 1;
            controlButtonsPanel.BackColor = Color.Transparent;
            controlButtonsPanel.WrapContents = false;

            ConfigureControlButton(btnStart, "START ALL", Color.FromArgb(87, 242, 135));
            ConfigureControlButton(btnStop, "STOP ALL", Color.FromArgb(237, 66, 69));
            ConfigureControlButton(btnReboot, "REBOOT", Color.FromArgb(88, 101, 242));

            btnStart.Click += B_Start_Click;
            btnStop.Click += B_Stop_Click;
            btnReboot.Click += B_RebootStop_Click;

            botsPanel.BackColor = Color.Transparent;
            botsPanel.Controls.Add(FLP_Bots);
            botsPanel.Controls.Add(botHeaderPanel);
            botsPanel.Dock = DockStyle.Fill;
            botsPanel.Location = new Point(0, 100);
            botsPanel.Name = "botsPanel";
            botsPanel.Padding = new Padding(40);
            botsPanel.Size = new Size(1020, 620);
            botsPanel.TabIndex = 0;
            botsPanel.Visible = true;
            EnableDoubleBuffering(botsPanel);

            botHeaderPanel.BackColor = Color.FromArgb(35, 35, 35);
            botHeaderPanel.Controls.Add(addBotPanel);
            botHeaderPanel.Dock = DockStyle.Top;
            botHeaderPanel.Height = 100;
            botHeaderPanel.Location = new Point(40, 40);
            botHeaderPanel.Name = "botHeaderPanel";
            botHeaderPanel.Size = new Size(940, 100);
            botHeaderPanel.TabIndex = 1;
            CreateRoundedPanel(botHeaderPanel);
            EnableDoubleBuffering(botHeaderPanel);

            addBotPanel.Controls.Add(B_New);
            addBotPanel.Controls.Add(CB_Routine);
            addBotPanel.Controls.Add(CB_Protocol);
            addBotPanel.Controls.Add(NUD_Port);
            addBotPanel.Controls.Add(TB_IP);
            addBotPanel.Dock = DockStyle.Fill;
            addBotPanel.Location = new Point(0, 0);
            addBotPanel.Name = "addBotPanel";
            addBotPanel.Size = new Size(940, 100);
            addBotPanel.TabIndex = 0;
            addBotPanel.BackColor = Color.Transparent;

            TB_IP.BackColor = Color.FromArgb(28, 28, 28);
            TB_IP.BorderStyle = BorderStyle.FixedSingle;
            TB_IP.Font = new Font("Segoe UI", 11F);
            TB_IP.ForeColor = Color.FromArgb(224, 224, 224);
            TB_IP.Location = new Point(30, 35);
            TB_IP.Name = "TB_IP";
            TB_IP.PlaceholderText = "IP Address";
            TB_IP.Size = new Size(150, 25);
            TB_IP.TabIndex = 0;
            TB_IP.Text = "192.168.0.1";

            ConfigureNumericUpDown(NUD_Port, 190, 35, 80);
            NUD_Port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Port.Value = new decimal(new int[] { 6000, 0, 0, 0 });

            CB_Protocol.SuspendLayout();
            ConfigureComboBox(CB_Protocol, 280, 35, 120);
            CB_Protocol.SelectedIndexChanged += CB_Protocol_SelectedIndexChanged;
            CB_Protocol.ResumeLayout();

            ConfigureComboBox(CB_Routine, 410, 35, 200);

            B_New.BackColor = Color.FromArgb(87, 242, 135);
            B_New.FlatAppearance.BorderSize = 0;
            B_New.FlatStyle = FlatStyle.Flat;
            B_New.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            B_New.ForeColor = Color.FromArgb(28, 28, 28);
            B_New.Location = new Point(620, 30);
            B_New.Name = "B_New";
            B_New.Size = new Size(120, 40);
            B_New.TabIndex = 4;
            B_New.Text = "ADD BOT";
            B_New.UseVisualStyleBackColor = false;
            B_New.Click += B_New_Click;
            B_New.Cursor = Cursors.Hand;
            ConfigureGlowButton(B_New);

            FLP_Bots.AutoScroll = true;
            FLP_Bots.BackColor = Color.Transparent;
            FLP_Bots.Dock = DockStyle.Fill;
            FLP_Bots.FlowDirection = FlowDirection.TopDown;
            FLP_Bots.Location = new Point(40, 140);
            FLP_Bots.Margin = new Padding(0, 20, 0, 0);
            FLP_Bots.Name = "FLP_Bots";
            FLP_Bots.Padding = new Padding(0);
            FLP_Bots.Size = new Size(940, 440);
            FLP_Bots.TabIndex = 0;
            FLP_Bots.WrapContents = false;
            FLP_Bots.Resize += FLP_Bots_Resize;
            FLP_Bots.Paint += FLP_Bots_Paint;
            EnableDoubleBuffering(FLP_Bots);

            hubPanel.BackColor = Color.Transparent;
            hubPanel.Controls.Add(PG_Hub);
            hubPanel.Dock = DockStyle.Fill;
            hubPanel.Location = new Point(0, 100);
            hubPanel.Name = "hubPanel";
            hubPanel.Padding = new Padding(40);
            hubPanel.Size = new Size(1020, 620);
            hubPanel.TabIndex = 1;
            hubPanel.Visible = false;
            EnableDoubleBuffering(hubPanel);

            var pgContainer = new Panel();
            pgContainer.BackColor = Color.FromArgb(35, 35, 35);
            pgContainer.Dock = DockStyle.Fill;
            pgContainer.Location = new Point(40, 40);
            pgContainer.Name = "pgContainer";
            pgContainer.Padding = new Padding(2);
            pgContainer.Size = new Size(940, 540);
            CreateRoundedPanel(pgContainer);
            EnableDoubleBuffering(pgContainer);
            hubPanel.Controls.Add(pgContainer);

            PG_Hub.BackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.CategoryForeColor = Color.FromArgb(224, 224, 224);
            PG_Hub.CategorySplitterColor = Color.FromArgb(50, 50, 50);
            PG_Hub.CommandsBackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.CommandsForeColor = Color.FromArgb(224, 224, 224);
            PG_Hub.Dock = DockStyle.Fill;
            PG_Hub.Font = new Font("Segoe UI", 9F);
            PG_Hub.HelpBackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.HelpForeColor = Color.FromArgb(176, 176, 176);
            PG_Hub.LineColor = Color.FromArgb(50, 50, 50);
            PG_Hub.Location = new Point(2, 2);
            PG_Hub.Name = "PG_Hub";
            PG_Hub.PropertySort = PropertySort.Categorized;
            PG_Hub.Size = new Size(936, 536);
            PG_Hub.TabIndex = 0;
            PG_Hub.ToolbarVisible = false;
            PG_Hub.ViewBackColor = Color.FromArgb(28, 28, 28);
            PG_Hub.ViewForeColor = Color.FromArgb(224, 224, 224);
            pgContainer.Controls.Add(PG_Hub);
            PG_Hub.CreateControl();

            logsPanel.BackColor = Color.Transparent;
            logsPanel.Controls.Add(RTB_Logs);
            logsPanel.Dock = DockStyle.Fill;
            logsPanel.Location = new Point(0, 100);
            logsPanel.Name = "logsPanel";
            logsPanel.Padding = new Padding(40);
            logsPanel.Size = new Size(1020, 620);
            logsPanel.TabIndex = 2;
            logsPanel.Visible = false;
            EnableDoubleBuffering(logsPanel);

            var logsContainer = new Panel();
            logsContainer.BackColor = Color.FromArgb(35, 35, 35);
            logsContainer.Dock = DockStyle.Fill;
            logsContainer.Location = new Point(40, 120);
            logsContainer.Margin = new Padding(0, 20, 0, 0);
            logsContainer.Name = "logsContainer";
            logsContainer.Padding = new Padding(2);
            logsContainer.Size = new Size(940, 460);
            CreateRoundedPanel(logsContainer);
            EnableDoubleBuffering(logsContainer);
            logsPanel.Controls.Add(logsContainer);
            logsPanel.Controls.Add(logsHeaderPanel);

            logsHeaderPanel.BackColor = Color.FromArgb(35, 35, 35);
            logsHeaderPanel.Controls.Add(searchPanel);
            logsHeaderPanel.Controls.Add(searchOptionsPanel);
            logsHeaderPanel.Controls.Add(searchStatusLabel);
            logsHeaderPanel.Controls.Add(btnClearLogs);
            logsHeaderPanel.Dock = DockStyle.Top;
            logsHeaderPanel.Height = 70;
            logsHeaderPanel.Location = new Point(40, 40);
            logsHeaderPanel.Name = "logsHeaderPanel";
            logsHeaderPanel.Padding = new Padding(20, 10, 20, 10);
            logsHeaderPanel.Size = new Size(940, 70);
            logsHeaderPanel.TabIndex = 1;
            CreateRoundedPanel(logsHeaderPanel);
            EnableDoubleBuffering(logsHeaderPanel);

            searchPanel.Controls.Add(logSearchBox);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 30;
            searchPanel.Location = new Point(20, 10);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new Size(550, 30);
            searchPanel.TabIndex = 0;
            searchPanel.BackColor = Color.Transparent;

            logSearchBox.BackColor = Color.FromArgb(28, 28, 28);
            logSearchBox.BorderStyle = BorderStyle.FixedSingle;
            logSearchBox.Dock = DockStyle.Fill;
            logSearchBox.Font = new Font("Segoe UI", 10F);
            logSearchBox.ForeColor = Color.FromArgb(224, 224, 224);
            logSearchBox.Location = new Point(0, 0);
            logSearchBox.Name = "logSearchBox";
            logSearchBox.PlaceholderText = "Search logs (Enter = next, Shift+Enter = previous, Esc = clear)...";
            logSearchBox.Size = new Size(550, 30);
            logSearchBox.TabIndex = 0;
            logSearchBox.TextChanged += LogSearchBox_TextChanged;
            logSearchBox.KeyDown += LogSearchBox_KeyDown;

            searchOptionsPanel.AutoSize = true;
            searchOptionsPanel.Controls.Add(btnCaseSensitive);
            searchOptionsPanel.Controls.Add(btnRegex);
            searchOptionsPanel.Controls.Add(btnWholeWord);
            searchOptionsPanel.Dock = DockStyle.Bottom;
            searchOptionsPanel.FlowDirection = FlowDirection.LeftToRight;
            searchOptionsPanel.Height = 25;
            searchOptionsPanel.Location = new Point(20, 45);
            searchOptionsPanel.Name = "searchOptionsPanel";
            searchOptionsPanel.Size = new Size(550, 25);
            searchOptionsPanel.TabIndex = 1;
            searchOptionsPanel.BackColor = Color.Transparent;
            searchOptionsPanel.WrapContents = false;

            ConfigureSearchOption(btnCaseSensitive, "Aa", "Case sensitive search");
            ConfigureSearchOption(btnRegex, ".*", "Regular expression search");
            ConfigureSearchOption(btnWholeWord, "Ab", "Whole word search");

            searchStatusLabel.AutoSize = true;
            searchStatusLabel.Dock = DockStyle.Right;
            searchStatusLabel.Font = new Font("Segoe UI", 9F);
            searchStatusLabel.ForeColor = Color.FromArgb(176, 176, 176);
            searchStatusLabel.Location = new Point(650, 10);
            searchStatusLabel.Padding = new Padding(10, 18, 10, 0);
            searchStatusLabel.Name = "searchStatusLabel";
            searchStatusLabel.Size = new Size(150, 50);
            searchStatusLabel.TabIndex = 2;
            searchStatusLabel.Text = "";
            searchStatusLabel.TextAlign = ContentAlignment.MiddleRight;

            btnClearLogs.BackColor = Color.FromArgb(237, 66, 69);
            btnClearLogs.Dock = DockStyle.Right;
            btnClearLogs.FlatAppearance.BorderSize = 0;
            btnClearLogs.FlatStyle = FlatStyle.Flat;
            btnClearLogs.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClearLogs.ForeColor = Color.White;
            btnClearLogs.Location = new Point(800, 10);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(120, 50);
            btnClearLogs.TabIndex = 3;
            btnClearLogs.Text = "CLEAR LOGS";
            btnClearLogs.UseVisualStyleBackColor = false;
            btnClearLogs.Cursor = Cursors.Hand;
            btnClearLogs.Click += (s, e) => {
                RTB_Logs.Clear();
                _searchManager.ClearSearch();
            };
            ConfigureGlowButton(btnClearLogs);

            RTB_Logs.BackColor = Color.FromArgb(28, 28, 28);
            RTB_Logs.BorderStyle = BorderStyle.None;
            RTB_Logs.Dock = DockStyle.Fill;
            RTB_Logs.Font = new Font("Consolas", 10F);
            RTB_Logs.ForeColor = Color.FromArgb(224, 224, 224);
            RTB_Logs.Location = new Point(2, 2);
            RTB_Logs.Name = "RTB_Logs";
            RTB_Logs.ReadOnly = true;
            RTB_Logs.Size = new Size(936, 456);
            RTB_Logs.TabIndex = 0;
            RTB_Logs.Text = "";
            RTB_Logs.HideSelection = false;
            RTB_Logs.KeyDown += RTB_Logs_KeyDown;
            logsContainer.Controls.Add(RTB_Logs);

            TC_Main = new TabControl { Visible = false };
            Tab_Bots = new TabPage();
            Tab_Hub = new TabPage();
            Tab_Logs = new TabPage();
            TC_Main.TabPages.AddRange(new[] { Tab_Bots, Tab_Hub, Tab_Logs });
            TC_Main.SendToBack();

            Controls.Add(mainLayoutPanel);

            mainLayoutPanel.ResumeLayout(false);
            sidebarPanel.ResumeLayout(false);
            navButtonsPanel.ResumeLayout(false);
            sidebarBottomPanel.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            controlButtonsPanel.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            botsPanel.ResumeLayout(false);
            botHeaderPanel.ResumeLayout(false);
            addBotPanel.ResumeLayout(false);
            addBotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).EndInit();
            hubPanel.ResumeLayout(false);
            logsPanel.ResumeLayout(false);
            logsHeaderPanel.ResumeLayout(false);
            logsHeaderPanel.PerformLayout();
            searchPanel.ResumeLayout(false);
            searchOptionsPanel.ResumeLayout(false);
            ResumeLayout(false);

            ConfigureSystemTray();
            animationTimer.Start();
        }

        private void EnableDoubleBuffering(Control control)
        {
            if (control == null) return;

            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private void HeaderPanel_Resize(object sender, EventArgs e)
        {
            if (controlButtonsPanel != null && headerPanel != null)
            {
                int rightMargin = 40;
                int minLeftPosition = titleLabel.Right + 50;
                int desiredX = headerPanel.Width - controlButtonsPanel.Width - rightMargin;
                controlButtonsPanel.Location = new Point(Math.Max(minLeftPosition, desiredX), 30);
            }
        }

        private LinearGradientBrush _logoBrush;

        private void ConfigureSearchOption(CheckBox checkBox, string text, string tooltip)
        {
            checkBox.Appearance = Appearance.Button;
            checkBox.BackColor = Color.FromArgb(40, 40, 40);
            checkBox.FlatAppearance.BorderSize = 1;
            checkBox.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            checkBox.FlatAppearance.CheckedBackColor = Color.FromArgb(88, 101, 242);
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            checkBox.ForeColor = Color.FromArgb(200, 200, 200);
            checkBox.Margin = new Padding(0, 0, 10, 0);
            checkBox.Size = new Size(30, 20);
            checkBox.Text = text;
            checkBox.TextAlign = ContentAlignment.MiddleCenter;
            checkBox.UseVisualStyleBackColor = false;
            checkBox.Cursor = Cursors.Hand;

            var toolTip = new ToolTip();
            toolTip.SetToolTip(checkBox, tooltip);
        }

        private void ConfigureNavButton(Button btn, string text, int index, string tooltip)
        {
            btn.BackColor = Color.FromArgb(18, 18, 18);
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 30, 30);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            btn.ForeColor = Color.FromArgb(176, 176, 176);
            btn.Location = new Point(0, 40 + (index * 60));
            btn.Margin = new Padding(0, 0, 0, 10);
            btn.Name = $"btnNav{text.Replace(" ", "")}";
            btn.Padding = new Padding(60, 0, 0, 0);
            btn.Size = new Size(260, 50);
            btn.TabIndex = index;
            btn.Text = text;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.UseVisualStyleBackColor = false;
            btn.Tag = new ButtonAnimationState();

            btn.Paint += (s, e) => {
                var animState = btn.Tag as ButtonAnimationState;

                if (btn.BackColor == Color.FromArgb(30, 30, 30))
                {
                    using var brush = new LinearGradientBrush(
                        new RectangleF(0, 0, 4, btn.Height),
                        Color.FromArgb(87, 242, 135),
                        Color.FromArgb(88, 101, 242),
                        LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(brush, 0, 0, 4, btn.Height);
                }

                var iconRect = new Rectangle(20, (btn.Height - 24) / 2, 24, 24);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                using var iconFont = new Font("Segoe MDL2 Assets", 16F);
                string iconText = index switch
                {
                    0 => "\uE77B",
                    1 => "\uE713",
                    2 => "\uE7C3",
                    3 => "\uE7E8",
                    _ => "\uE700"
                };

                using var iconBrush = new SolidBrush(
                    btn.BackColor == Color.FromArgb(30, 30, 30)
                        ? Color.FromArgb(224, 224, 224)
                        : Color.FromArgb(150, 150, 150)
                );

                var textSize = e.Graphics.MeasureString(iconText, iconFont);
                var textX = iconRect.X + (iconRect.Width - textSize.Width) / 2;
                var textY = iconRect.Y + (iconRect.Height - textSize.Height) / 2;

                e.Graphics.DrawString(iconText, iconFont, iconBrush, textX, textY);
            };

            btn.Click += (s, e) => {
                if (index >= 3) return;

                foreach (Button navBtn in navButtonsPanel.Controls.OfType<Button>())
                {
                    if (navBtn.TabIndex < 3)
                    {
                        navBtn.BackColor = Color.FromArgb(18, 18, 18);
                        navBtn.ForeColor = Color.FromArgb(176, 176, 176);
                    }
                }

                btn.BackColor = Color.FromArgb(30, 30, 30);
                btn.ForeColor = Color.FromArgb(224, 224, 224);

                TransitionPanels(index);

                titleLabel.Text = index switch
                {
                    0 => "Bot Management",
                    1 => "Configuration",
                    2 => "System Logs",
                    _ => "PokéBot"
                };
            };

            ConfigureHoverAnimation(btn);

            if (index == 0)
            {
                btn.BackColor = Color.FromArgb(30, 30, 30);
                btn.ForeColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void ConfigureControlButton(Button btn, string text, Color baseColor)
        {
            btn.BackColor = baseColor;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatStyle = FlatStyle.Flat;

            // Adaptive font sizing based on DPI
            float fontSize = 10F;
            using (Graphics g = btn.CreateGraphics())
            {
                float dpiScale = g.DpiX / 96f; // 96 DPI is standard
                fontSize = Math.Max(9F, 10F * dpiScale); // Scale font but keep minimum readable size
            }

            btn.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            btn.ForeColor = Color.FromArgb(28, 28, 28);
            btn.Margin = new Padding(5, 0, 5, 0);
            btn.Name = $"btn{text.Replace(" ", "")}";
            btn.Padding = new Padding(15, 8, 15, 8); // Increased padding for better text fit
            btn.TabIndex = 0;
            btn.Text = text;
            btn.UseVisualStyleBackColor = false;
            btn.Tag = new ButtonAnimationState { BaseColor = baseColor };

            // Auto-size based on content with minimum width
            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.MinimumSize = new Size(100, 40); // Minimum size to ensure visibility
            btn.MaximumSize = new Size(200, 50); // Maximum size to prevent over-growth

            CreateRoundedButton(btn);
            ConfigureGlowButton(btn);

            // Handle resize to maintain rounded corners
            btn.Resize += (s, e) => btn.Invalidate();
        }

        private void ConfigureNumericUpDown(NumericUpDown nud, int x, int y, int width)
        {
            nud.BackColor = Color.FromArgb(28, 28, 28);
            nud.BorderStyle = BorderStyle.None;
            nud.Font = new Font("Segoe UI", 11F);
            nud.ForeColor = Color.FromArgb(224, 224, 224);
            nud.Location = new Point(x, y);
            nud.Name = nud.Name;
            nud.Size = new Size(width, 25);
            nud.TabIndex = 1;
        }

        private void ConfigureComboBox(ComboBox cb, int x, int y, int width)
        {
            cb.BackColor = Color.FromArgb(28, 28, 28);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.FlatStyle = FlatStyle.Flat;
            cb.Font = new Font("Segoe UI", 11F);
            cb.ForeColor = Color.FromArgb(224, 224, 224);
            cb.Location = new Point(x, y);
            cb.Name = cb.Name;
            cb.Size = new Size(width, 28);
            cb.TabIndex = 2;
        }

        private void ConfigureHoverAnimation(Control control)
        {
            var animState = control.Tag as ButtonAnimationState ?? new ButtonAnimationState();
            control.Tag = animState;

            control.MouseEnter += (s, e) => {
                animState.IsHovering = true;
                animState.AnimationStart = DateTime.Now;
            };

            control.MouseLeave += (s, e) => {
                animState.IsHovering = false;
                animState.AnimationStart = DateTime.Now;
            };
        }

        private void ConfigureGlowButton(Button btn)
        {
            ConfigureHoverAnimation(btn);

            btn.Paint += (s, e) => {
                var animState = btn.Tag as ButtonAnimationState;
                if (animState != null && animState.HoverProgress > 0)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    var glowAlpha = (int)(60 * animState.HoverProgress);
                    using (var glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, btn.BackColor)))
                    {
                        for (int i = 1; i <= 1; i++)
                        {
                            var rect = new Rectangle(-i * 2, -i * 2, btn.Width + i * 4, btn.Height + i * 4);
                            e.Graphics.FillRectangle(glowBrush, rect);
                        }
                    }
                }
            };
        }

        private void CreateRoundedPanel(Panel panel)
        {
            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = new GraphicsPath();
                var rect = panel.ClientRectangle;
                rect.Inflate(-1, -1);
                GraphicsExtensions.AddRoundedRectangle(path, rect, 8);
                panel.Region = new Region(path);
            };
        }

        private void CreateRoundedButton(Button btn)
        {
            // Handle the paint event to create rounded corners that adapt to button size
            btn.Paint += (s, e) => {
                if (btn.Region != null) btn.Region.Dispose();

                using var path = new GraphicsPath();
                var rect = btn.ClientRectangle;
                int radius = Math.Min(6, Math.Min(rect.Width, rect.Height) / 4); // Adaptive radius
                GraphicsExtensions.AddRoundedRectangle(path, rect, radius);
                btn.Region = new Region(path);
            };

            // Trigger initial paint
            btn.Invalidate();
        }

        private void CreateCircularRegion(Control control)
        {
            using var path = new GraphicsPath();
            path.AddEllipse(0, 0, control.Width, control.Height);
            control.Region = new Region(path);
        }

        private void LogoPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (_logoBrush == null)
            {
                _logoBrush = new LinearGradientBrush(
                    logoPanel.ClientRectangle,
                    Color.FromArgb(88, 101, 242),
                    Color.FromArgb(87, 242, 135),
                    LinearGradientMode.ForwardDiagonal);
            }

            e.Graphics.FillRectangle(_logoBrush, logoPanel.ClientRectangle);

            using var font = new Font("Segoe UI", 22F, FontStyle.Bold);
            var text = "POKÉBOT";
            var textSize = e.Graphics.MeasureString(text, font);
            var x = (logoPanel.Width - textSize.Width) / 2;
            var y = (logoPanel.Height - textSize.Height) / 2;

            using var glowBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255));
            e.Graphics.DrawString(text, font, glowBrush, x - 1, y - 1);

            using var textBrush = new SolidBrush(Color.FromArgb(18, 18, 18));
            e.Graphics.DrawString(text, font, textBrush, x, y);
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(
                new RectangleF(0, headerPanel.Height - 2, headerPanel.Width, 2),
                Color.FromArgb(50, 88, 101, 242),
                Color.FromArgb(50, 87, 242, 135),
                LinearGradientMode.Horizontal);

            e.Graphics.FillRectangle(brush, 0, headerPanel.Height - 2, headerPanel.Width, 2);
        }

        private void FLP_Bots_Paint(object sender, PaintEventArgs e)
        {
            if (FLP_Bots.Controls.Count == 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                using var font = new Font("Segoe UI", 14F, FontStyle.Regular);
                using var brush = new SolidBrush(Color.FromArgb(100, 176, 176, 176));
                var text = "No bots configured. Add a bot using the form above.";
                var size = e.Graphics.MeasureString(text, font);
                e.Graphics.DrawString(text, font, brush,
                    (FLP_Bots.Width - size.Width) / 2,
                    100);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control control in GetAllControls(this))
            {
                if (control.Tag is ButtonAnimationState animState)
                {
                    var oldProgress = animState.HoverProgress;
                    var elapsed = (DateTime.Now - animState.AnimationStart).TotalMilliseconds;
                    var duration = 150.0;

                    if (animState.IsHovering)
                    {
                        animState.HoverProgress = Math.Min(1.0, elapsed / duration);
                    }
                    else
                    {
                        animState.HoverProgress = Math.Max(0.0, 1.0 - (elapsed / duration));
                    }

                    if (Math.Abs(animState.HoverProgress - oldProgress) > 0.01)
                    {
                        control.Invalidate();
                    }
                }
            }

            // Update status indicator pulse with throttling
            UpdateStatusIndicatorPulse();
        }

        private void TransitionPanels(int index)
        {
            // Hide all panels
            botsPanel.Visible = false;
            hubPanel.Visible = false;
            logsPanel.Visible = false;

            // Force refresh of content panel
            contentPanel.Refresh();

            // Show the selected panel
            switch (index)
            {
                case 0:
                    botsPanel.Visible = true;
                    break;
                case 1:
                    hubPanel.Visible = true;
                    break;
                case 2:
                    logsPanel.Visible = true;
                    logsPanel.Refresh(); // Extra refresh for logs panel
                    break;
            }
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            var controls = container.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl)).Concat(controls);
        }

        private void ConfigureSystemTray()
        {
            trayIcon.Icon = Icon;
            trayIcon.Text = "PokéBot Control Center";
            trayIcon.Visible = false;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            trayContextMenu.BackColor = Color.FromArgb(35, 35, 35);
            trayContextMenu.Font = new Font("Segoe UI", 10F);
            trayContextMenu.Renderer = new ModernMenuRenderer();

            trayMenuShow.Text = "Show Window";
            trayMenuShow.ForeColor = Color.FromArgb(224, 224, 224);
            trayMenuShow.Click += TrayMenuShow_Click;

            var separator = new ToolStripSeparator();

            var trayMenuStart = new ToolStripMenuItem("Start All Bots");
            trayMenuStart.ForeColor = Color.FromArgb(87, 242, 135);
            trayMenuStart.Click += (s, e) => {
                RunningEnvironment.InitializeStart();
                foreach (var c in FLP_Bots.Controls.OfType<BotController>())
                    c.SendCommand(BotControlCommand.Start, false);
                LogUtil.LogInfo("All bots started from tray", "Tray");
            };

            var trayMenuStop = new ToolStripMenuItem("Stop All Bots");
            trayMenuStop.ForeColor = Color.FromArgb(237, 66, 69);
            trayMenuStop.Click += (s, e) => {
                RunningEnvironment.StopAll();
                LogUtil.LogInfo("All bots stopped from tray", "Tray");
            };

            var separator2 = new ToolStripSeparator();

            trayMenuExit.Text = "Exit";
            trayMenuExit.ForeColor = Color.FromArgb(237, 66, 69);
            trayMenuExit.Click += TrayMenuExit_Click;

            trayContextMenu.Items.AddRange(new ToolStripItem[] {
                trayMenuShow,
                separator,
                trayMenuStart,
                trayMenuStop,
                separator2,
                trayMenuExit
            });
            trayIcon.ContextMenuStrip = trayContextMenu;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void TrayMenuShow_Click(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void TrayMenuExit_Click(object sender, EventArgs e)
        {
            _isReallyClosing = true;
            Close();
        }

        private void ShowFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            trayIcon.Visible = false;
            BringToFront();
            Activate();

            int headerHeight = headerPanel.Height + 10;

            if (hubPanel.Padding.Top <= 40)
                hubPanel.Padding = new Padding(40, headerHeight, 40, 40);

            if (logsPanel.Padding.Top <= 40)
                logsPanel.Padding = new Padding(40, headerHeight, 40, 40);

            hubPanel.PerformLayout();
            PG_Hub.Refresh();

            logsPanel.PerformLayout();
            RTB_Logs.Refresh();
        }

        private void MinimizeToTray()
        {
            Hide();
            ShowInTaskbar = false;
            trayIcon.Visible = true;

            var runningBots = FLP_Bots.Controls.OfType<BotController>().Count(c => c.GetBot()?.IsRunning ?? false);
            var totalBots = FLP_Bots.Controls.OfType<BotController>().Count();

            string message = totalBots == 0
                ? "No bots configured"
                : $"{runningBots} of {totalBots} bots running";

            trayIcon.ShowBalloonTip(2000, "PokéBot Minimized", message, ToolTipIcon.Info);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && !_isReallyClosing)
            {
                MinimizeToTray();
            }
        }

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
                e.TextColor = e.Item.Enabled ? e.Item.ForeColor : Color.FromArgb(100, 100, 100);
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

        private class ButtonAnimationState
        {
            public bool IsHovering { get; set; }
            public DateTime AnimationStart { get; set; }
            public double HoverProgress { get; set; }
            public Color BaseColor { get; set; }
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Panel headerPanel;
        private Panel logoPanel;
        private FlowLayoutPanel navButtonsPanel;
        private Button btnNavBots;
        private Button btnNavHub;
        private Button btnNavLogs;
        private Panel sidebarBottomPanel;
        private Button btnUpdate;
        private Label titleLabel;
        private FlowLayoutPanel controlButtonsPanel;
        private Button btnStart;
        private Button btnStop;
        private Button btnReboot;
        private Panel botsPanel;
        private Panel hubPanel;
        private Panel logsPanel;
        private Panel botHeaderPanel;
        private Panel addBotPanel;
        private TextBox TB_IP;
        private NumericUpDown NUD_Port;
        private ComboBox CB_Protocol;
        private ComboBox CB_Routine;
        private Button B_New;
        private FlowLayoutPanel FLP_Bots;
        private PropertyGrid PG_Hub;
        private RichTextBox RTB_Logs;
        private Panel logsHeaderPanel;
        private Panel searchPanel;
        private TextBox logSearchBox;
        private FlowLayoutPanel searchOptionsPanel;
        private CheckBox btnCaseSensitive;
        private CheckBox btnRegex;
        private CheckBox btnWholeWord;
        private Button btnClearLogs;
        private Label searchStatusLabel;
        private Panel statusIndicator;
        private System.Windows.Forms.Timer animationTimer;
        private ComboBox comboBox1;

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayContextMenu;
        private ToolStripMenuItem trayMenuShow;
        private ToolStripMenuItem trayMenuExit;
        private bool _isReallyClosing = false;

        private Button updater => btnUpdate;
        private Button B_Start => btnStart;
        private Button B_Stop => btnStop;
        private Button B_RebootStop => btnReboot;
        private TabControl TC_Main;
        private TabPage Tab_Bots;
        private TabPage Tab_Hub;
        private TabPage Tab_Logs;
        private Panel ButtonPanel => controlButtonsPanel;
    }

    public static class GraphicsExtensions
    {
        public static void AddRoundedRectangle(this GraphicsPath path, Rectangle rect, int radius)
        {
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
        }
    }
}
