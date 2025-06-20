using SysBot.Pokemon.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using SysBot.Base;

#pragma warning disable CS8618
#pragma warning disable CS8625
#pragma warning disable CS8669

namespace SysBot.Pokemon.WinForms
{
    partial class Main
    {
        private System.ComponentModel.IContainer? components = null;

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
            MinimumSize = new Size(800, 500);
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
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
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
            sidebarPanel.Size = new Size(240, 720);
            sidebarPanel.TabIndex = 0;
            EnableDoubleBuffering(sidebarPanel);

            logoPanel.BackColor = Color.FromArgb(15, 15, 15);
            logoPanel.Dock = DockStyle.Top;
            logoPanel.Height = 100;
            logoPanel.Location = new Point(0, 0);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new Size(240, 100);
            logoPanel.TabIndex = 2;
            logoPanel.Paint += LogoPanel_Paint;
            EnableDoubleBuffering(logoPanel);

            Resize += (s, e) => {
                if (Width < 900)
                {
                    logoPanel.Height = 70;
                    navButtonsPanel.Padding = new Padding(0, 20, 0, 0);
                }
                else if (Width < 1100)
                {
                    logoPanel.Height = 85;
                    navButtonsPanel.Padding = new Padding(0, 30, 0, 0);
                }
                else
                {
                    logoPanel.Height = 100;
                    navButtonsPanel.Padding = new Padding(0, 40, 0, 0);
                }
                logoPanel.Invalidate();
            };

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
            navButtonsPanel.Size = new Size(240, 540);
            navButtonsPanel.TabIndex = 1;
            navButtonsPanel.BackColor = Color.Transparent;
            EnableDoubleBuffering(navButtonsPanel);

            ConfigureNavButton(btnNavBots, "BOTS", 0, "Manage bot connections");
            ConfigureNavButton(btnNavHub, "CONFIGURATION", 1, "System settings");
            ConfigureNavButton(btnNavLogs, "SYSTEM LOGS", 2, "View activity logs");

            var separator = new Panel();
            separator.BackColor = Color.FromArgb(50, 50, 50);
            separator.Size = new Size(200, 1);
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

            Resize += (s, e) => {
                foreach (Button navBtn in navButtonsPanel.Controls.OfType<Button>())
                {
                    if (Width < 900)
                    {
                        navBtn.Height = 40;
                        navBtn.Font = ScaleFont(new Font("Segoe UI", 9F, FontStyle.Regular));
                        navBtn.Margin = new Padding(0, 0, 0, 5);
                    }
                    else
                    {
                        navBtn.Height = 50;
                        navBtn.Font = ScaleFont(new Font("Segoe UI", 10F, FontStyle.Regular));
                        navBtn.Margin = new Padding(0, 0, 0, 10);
                    }

                    var idx = navButtonsPanel.Controls.GetChildIndex(navBtn);
                    if (idx <= 3)
                    {
                        navBtn.Location = new Point(0, 40 + (idx * (navBtn.Height + navBtn.Margin.Bottom)));
                    }
                }
            };

            sidebarBottomPanel.Controls.Add(btnUpdate);
            sidebarBottomPanel.Controls.Add(comboBox1);
            sidebarBottomPanel.Dock = DockStyle.Bottom;
            sidebarBottomPanel.Height = 100;
            sidebarBottomPanel.Location = new Point(0, 620);
            sidebarBottomPanel.Name = "sidebarBottomPanel";
            sidebarBottomPanel.Padding = new Padding(15, 10, 15, 20);
            sidebarBottomPanel.TabIndex = 0;
            sidebarBottomPanel.BackColor = Color.FromArgb(15, 15, 15);
            EnableDoubleBuffering(sidebarBottomPanel);

            Resize += (s, e) => {
                if (Width < 900)
                {
                    sidebarBottomPanel.Height = 80;
                    btnUpdate.Size = new Size(210, 35);
                }
                else
                {
                    sidebarBottomPanel.Height = 100;
                    btnUpdate.Size = new Size(210, 40);
                }
            };

            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.BackColor = Color.FromArgb(30, 30, 30);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.Font = new Font("Segoe UI", 9F);
            comboBox1.ForeColor = Color.FromArgb(224, 224, 224);
            comboBox1.Location = new Point(15, 10);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(210, 23);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnUpdate.BackColor = Color.FromArgb(30, 30, 30);
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnUpdate.ForeColor = Color.FromArgb(176, 176, 176);
            btnUpdate.Location = new Point(15, 40);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(210, 40);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "";
            btnUpdate.UseVisualStyleBackColor = false;
            btnUpdate.Click += Updater_Click;
            btnUpdate.Cursor = Cursors.Hand;
            btnUpdate.Tag = new ButtonAnimationState();
            ConfigureHoverAnimation(btnUpdate);
            ConfigureUpdateButton();

            contentPanel.BackColor = Color.FromArgb(28, 28, 28);
            contentPanel.Controls.Add(botsPanel);
            contentPanel.Controls.Add(hubPanel);
            contentPanel.Controls.Add(logsPanel);
            contentPanel.Controls.Add(headerPanel);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(240, 0);
            contentPanel.Margin = new Padding(0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1040, 720);
            contentPanel.TabIndex = 1;
            EnableDoubleBuffering(contentPanel);

            headerPanel.BackColor = Color.FromArgb(28, 28, 28);
            headerPanel.Controls.Add(controlButtonsPanel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1040, 100);
            headerPanel.TabIndex = 3;
            headerPanel.Paint += HeaderPanel_Paint;
            headerPanel.Resize += HeaderPanel_Resize;
            EnableDoubleBuffering(headerPanel);

            Resize += (s, e) => {
                if (Width < 900)
                {
                    headerPanel.Height = 70;
                    controlButtonsPanel.Location = new Point(controlButtonsPanel.Location.X, 20);
                    titleLabel.Location = new Point(titleLabel.Location.X, 15);
                }
                else if (Width < 1100)
                {
                    headerPanel.Height = 85;
                    controlButtonsPanel.Location = new Point(controlButtonsPanel.Location.X, 25);
                    titleLabel.Location = new Point(titleLabel.Location.X, 20);
                }
                else
                {
                    headerPanel.Height = 100;
                    controlButtonsPanel.Location = new Point(controlButtonsPanel.Location.X, 30);
                    titleLabel.Location = new Point(titleLabel.Location.X, 25);
                }
            };

            titleLabel.AutoSize = true;
            titleLabel.Font = ScaleFont(new Font("Segoe UI", 24F, FontStyle.Bold));
            titleLabel.ForeColor = Color.FromArgb(224, 224, 224);
            titleLabel.Location = new Point(40, 25);
            titleLabel.Name = "titleLabel";
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Bot Management";
            titleLabel.MaximumSize = new Size(400, 50);
            titleLabel.AutoEllipsis = true;

            Resize += (s, e) => {
                if (Width < 900)
                {
                    titleLabel.Font = ScaleFont(new Font("Segoe UI", 16F, FontStyle.Bold));
                    titleLabel.MaximumSize = new Size(250, 40);
                }
                else if (Width < 1100)
                {
                    titleLabel.Font = ScaleFont(new Font("Segoe UI", 18F, FontStyle.Bold));
                    titleLabel.MaximumSize = new Size(300, 45);
                }
                else
                {
                    titleLabel.Font = ScaleFont(new Font("Segoe UI", 24F, FontStyle.Bold));
                    titleLabel.MaximumSize = new Size(400, 50);
                }
            };

            controlButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlButtonsPanel.AutoSize = true;
            controlButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlButtonsPanel.Controls.Add(btnStart);
            controlButtonsPanel.Controls.Add(btnStop);
            controlButtonsPanel.Controls.Add(btnReboot);
            controlButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            controlButtonsPanel.Location = new Point(contentPanel.Width - 400, 30);
            controlButtonsPanel.Name = "controlButtonsPanel";
            controlButtonsPanel.TabIndex = 1;
            controlButtonsPanel.BackColor = Color.Transparent;
            controlButtonsPanel.WrapContents = true;
            controlButtonsPanel.MaximumSize = new Size(500, 100);

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

            botHeaderPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            botHeaderPanel.BackColor = Color.FromArgb(35, 35, 35);
            botHeaderPanel.Controls.Add(addBotPanel);
            botHeaderPanel.Height = 100;
            botHeaderPanel.Location = new Point(40, 40);
            botHeaderPanel.Name = "botHeaderPanel";
            botHeaderPanel.Size = new Size(960, 100);
            botHeaderPanel.TabIndex = 1;
            CreateRoundedPanel(botHeaderPanel);
            EnableDoubleBuffering(botHeaderPanel);

            botsPanel.Resize += (s, e) => {
                if (botsPanel.Width < 700)
                {
                    botHeaderPanel.Height = 70;
                    FLP_Bots.Location = new Point(40, 120);
                }
                else if (botsPanel.Width < 900)
                {
                    botHeaderPanel.Height = 85;
                    FLP_Bots.Location = new Point(40, 140);
                }
                else
                {
                    botHeaderPanel.Height = 100;
                    FLP_Bots.Location = new Point(40, 160);
                }
            };

            addBotPanel.Controls.Add(B_New);
            addBotPanel.Controls.Add(CB_Routine);
            addBotPanel.Controls.Add(CB_Protocol);
            addBotPanel.Controls.Add(NUD_Port);
            addBotPanel.Controls.Add(TB_IP);
            addBotPanel.Dock = DockStyle.Fill;
            addBotPanel.Location = new Point(0, 0);
            addBotPanel.Name = "addBotPanel";
            addBotPanel.Size = new Size(960, 100);
            addBotPanel.TabIndex = 0;
            addBotPanel.BackColor = Color.Transparent;

            TB_IP.BackColor = Color.FromArgb(28, 28, 28);
            TB_IP.BorderStyle = BorderStyle.FixedSingle;
            TB_IP.Font = ScaleFont(new Font("Segoe UI", 10F));
            TB_IP.ForeColor = Color.FromArgb(224, 224, 224);
            TB_IP.Location = new Point(20, 35);
            TB_IP.Name = "TB_IP";
            TB_IP.PlaceholderText = "IP Address";
            TB_IP.Size = new Size(130, 25);
            TB_IP.TabIndex = 0;
            TB_IP.Text = "192.168.0.1";

            ConfigureNumericUpDown(NUD_Port, 160, 35, 70);
            NUD_Port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Port.Value = new decimal(new int[] { 6000, 0, 0, 0 });

            CB_Protocol.SuspendLayout();
            ConfigureComboBox(CB_Protocol, 240, 35, 100);
            CB_Protocol.SelectedIndexChanged += CB_Protocol_SelectedIndexChanged;
            CB_Protocol.ResumeLayout();

            ConfigureComboBox(CB_Routine, 350, 35, 170);

            B_New.BackColor = Color.FromArgb(87, 242, 135);
            B_New.FlatAppearance.BorderSize = 0;
            B_New.FlatStyle = FlatStyle.Flat;
            B_New.Font = ScaleFont(new Font("Segoe UI", 10F, FontStyle.Bold));
            B_New.ForeColor = Color.FromArgb(28, 28, 28);
            B_New.Location = new Point(820, 30);
            B_New.Name = "B_New";
            B_New.Size = new Size(100, 40);
            B_New.TabIndex = 4;
            B_New.Text = "ADD BOT";
            B_New.UseVisualStyleBackColor = false;
            B_New.Click += B_New_Click;
            B_New.Cursor = Cursors.Hand;
            ConfigureGlowButton(B_New);
            CreateRoundedButton(B_New);

            Action updateLayout = () => {
                if (addBotPanel.Width < 700)
                {
                    TB_IP.Width = 110;
                    NUD_Port.Width = 60;
                    CB_Protocol.Width = 80;
                    CB_Routine.Width = 120;

                    TB_IP.Location = new Point(10, 35);
                    NUD_Port.Location = new Point(125, 35);
                    CB_Protocol.Location = new Point(190, 35);
                    CB_Routine.Location = new Point(275, 35);

                    B_New.Size = new Size(80, 35);
                    B_New.Font = ScaleFont(new Font("Segoe UI", 9F, FontStyle.Bold));
                }
                else if (addBotPanel.Width < 850)
                {
                    TB_IP.Width = 120;
                    NUD_Port.Width = 65;
                    CB_Protocol.Width = 90;
                    CB_Routine.Width = 150;

                    TB_IP.Location = new Point(15, 35);
                    NUD_Port.Location = new Point(140, 35);
                    CB_Protocol.Location = new Point(210, 35);
                    CB_Routine.Location = new Point(305, 35);

                    B_New.Size = new Size(90, 38);
                    B_New.Font = ScaleFont(new Font("Segoe UI", 9.5F, FontStyle.Bold));
                }
                else
                {
                    TB_IP.Width = 130;
                    NUD_Port.Width = 70;
                    CB_Protocol.Width = 100;
                    CB_Routine.Width = 170;

                    TB_IP.Location = new Point(20, 35);
                    NUD_Port.Location = new Point(160, 35);
                    CB_Protocol.Location = new Point(240, 35);
                    CB_Routine.Location = new Point(350, 35);

                    B_New.Size = new Size(100, 40);
                    B_New.Font = ScaleFont(new Font("Segoe UI", 10F, FontStyle.Bold));
                }

                int buttonSpacing = addBotPanel.Width < 700 ? 10 : 15;
                int totalWidth = CB_Routine.Right + buttonSpacing + B_New.Width + 20;

                if (totalWidth > addBotPanel.Width && addBotPanel.Width < 600)
                {
                    B_New.Location = new Point(
                        TB_IP.Left,
                        CB_Routine.Bottom + 10
                    );

                    if (botHeaderPanel.Height < 120)
                        botHeaderPanel.Height = 120;
                }
                else
                {
                    B_New.Location = new Point(
                        CB_Routine.Right + buttonSpacing,
                        (addBotPanel.Height - B_New.Height) / 2
                    );
                }

                if (B_New.Width < 90)
                    B_New.Text = "ADD";
                else
                    B_New.Text = "ADD BOT";
            };

            addBotPanel.Resize += (s, e) => updateLayout();
            addBotPanel.Layout += (s, e) => updateLayout();

            botsPanel.Resize += (s, e) => {
                updateLayout();

                if (botsPanel.Width < 600 && B_New.Top > CB_Routine.Top)
                {
                    botHeaderPanel.Height = 120;
                    FLP_Bots.Location = new Point(40, 140);
                }
                else if (botsPanel.Width < 700)
                {
                    botHeaderPanel.Height = 70;
                    FLP_Bots.Location = new Point(40, 120);
                }
                else if (botsPanel.Width < 900)
                {
                    botHeaderPanel.Height = 85;
                    FLP_Bots.Location = new Point(40, 140);
                }
                else
                {
                    botHeaderPanel.Height = 100;
                    FLP_Bots.Location = new Point(40, 160);
                }
            };

            updateLayout();

            FLP_Bots.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FLP_Bots.AutoScroll = true;
            FLP_Bots.BackColor = Color.Transparent;
            FLP_Bots.FlowDirection = FlowDirection.TopDown;
            FLP_Bots.Location = new Point(40, 160);
            FLP_Bots.Margin = new Padding(0, 20, 0, 0);
            FLP_Bots.Name = "FLP_Bots";
            FLP_Bots.Padding = new Padding(0);
            FLP_Bots.Size = new Size(960, 420);
            FLP_Bots.TabIndex = 0;
            FLP_Bots.WrapContents = false;
            FLP_Bots.Resize += FLP_Bots_Resize;
            FLP_Bots.Paint += FLP_Bots_Paint;
            FLP_Bots.Scroll += (s, e) => FLP_Bots.Invalidate();
            FLP_Bots.ControlAdded += (s, e) => FLP_Bots.Invalidate();
            FLP_Bots.ControlRemoved += (s, e) => FLP_Bots.Invalidate();
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
            pgContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pgContainer.BackColor = Color.FromArgb(35, 35, 35);
            pgContainer.Location = new Point(40, 40);
            pgContainer.Name = "pgContainer";
            pgContainer.Padding = new Padding(2);
            pgContainer.Size = new Size(960, 540);
            CreateRoundedPanel(pgContainer);
            EnableDoubleBuffering(pgContainer);
            hubPanel.Controls.Add(pgContainer);

            PG_Hub.BackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.CategoryForeColor = Color.FromArgb(224, 224, 224);
            PG_Hub.CategorySplitterColor = Color.FromArgb(50, 50, 50);
            PG_Hub.CommandsBackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.CommandsForeColor = Color.FromArgb(224, 224, 224);
            PG_Hub.Dock = DockStyle.Fill;
            PG_Hub.Font = ScaleFont(new Font("Segoe UI", 9F));
            PG_Hub.HelpBackColor = Color.FromArgb(35, 35, 35);
            PG_Hub.HelpForeColor = Color.FromArgb(176, 176, 176);
            PG_Hub.LineColor = Color.FromArgb(50, 50, 50);
            PG_Hub.Location = new Point(2, 2);
            PG_Hub.Name = "PG_Hub";
            PG_Hub.PropertySort = PropertySort.Categorized;
            PG_Hub.Size = new Size(956, 536);
            PG_Hub.TabIndex = 0;
            PG_Hub.ToolbarVisible = false;
            PG_Hub.ViewBackColor = Color.FromArgb(28, 28, 28);
            PG_Hub.ViewForeColor = Color.FromArgb(224, 224, 224);
            pgContainer.Controls.Add(PG_Hub);
            PG_Hub.CreateControl();

            logsPanel.BackColor = Color.Transparent;
            logsPanel.Dock = DockStyle.Fill;
            logsPanel.Location = new Point(0, 100);
            logsPanel.Name = "logsPanel";
            logsPanel.Padding = new Padding(40);
            logsPanel.Size = new Size(1020, 620);
            logsPanel.TabIndex = 2;
            logsPanel.Visible = false;
            EnableDoubleBuffering(logsPanel);

            var logsContainer = new Panel();
            logsContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logsContainer.BackColor = Color.FromArgb(35, 35, 35);
            logsContainer.Location = new Point(40, 120);
            logsContainer.Margin = new Padding(0, 20, 0, 0);
            logsContainer.Name = "logsContainer";
            logsContainer.Padding = new Padding(2);
            logsContainer.Size = new Size(960, 460);
            CreateRoundedPanel(logsContainer);
            EnableDoubleBuffering(logsContainer);
            logsPanel.Controls.Add(logsContainer);
            logsPanel.Controls.Add(logsHeaderPanel);

            logsHeaderPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            logsHeaderPanel.BackColor = Color.FromArgb(35, 35, 35);
            logsHeaderPanel.Height = 70;
            logsHeaderPanel.Location = new Point(40, 40);
            logsHeaderPanel.Name = "logsHeaderPanel";
            logsHeaderPanel.Padding = new Padding(20, 10, 20, 10);
            logsHeaderPanel.Size = new Size(960, 70);
            logsHeaderPanel.TabIndex = 1;
            CreateRoundedPanel(logsHeaderPanel);
            EnableDoubleBuffering(logsHeaderPanel);

            logsHeaderPanel.Resize += (s, e) => {
                if (btnClearLogs != null && logsHeaderPanel.Width > 0)
                {
                    int rightMargin = 50; // Account for padding and rounded corners
                    btnClearLogs.Location = new Point(Math.Max(700, logsHeaderPanel.Width - btnClearLogs.Width - rightMargin), 15);
                }
                if (searchStatusLabel != null)
                {
                    searchStatusLabel.Location = new Point(Math.Max(550, logsHeaderPanel.Width - 320), 25);
                }
            };

            searchPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            searchPanel.Controls.Add(logSearchBox);
            searchPanel.Height = 30;
            searchPanel.Location = new Point(20, 10);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new Size(500, 30);
            searchPanel.TabIndex = 0;
            searchPanel.BackColor = Color.FromArgb(35, 35, 35);

            logSearchBox.BackColor = Color.FromArgb(28, 28, 28);
            logSearchBox.BorderStyle = BorderStyle.FixedSingle;
            logSearchBox.Dock = DockStyle.Fill;
            logSearchBox.Font = ScaleFont(new Font("Segoe UI", 10F));
            logSearchBox.ForeColor = Color.FromArgb(224, 224, 224);
            logSearchBox.Location = new Point(0, 0);
            logSearchBox.Name = "logSearchBox";
            logSearchBox.PlaceholderText = "Search logs (Enter = next, Shift+Enter = previous, Esc = clear)...";
            logSearchBox.Size = new Size(500, 30);
            logSearchBox.TabIndex = 0;
            logSearchBox.TextChanged += LogSearchBox_TextChanged;
            logSearchBox.KeyDown += LogSearchBox_KeyDown;

            searchOptionsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            searchOptionsPanel.AutoSize = true;
            searchOptionsPanel.Controls.Add(btnCaseSensitive);
            searchOptionsPanel.Controls.Add(btnRegex);
            searchOptionsPanel.Controls.Add(btnWholeWord);
            searchOptionsPanel.FlowDirection = FlowDirection.LeftToRight;
            searchOptionsPanel.Height = 25;
            searchOptionsPanel.Location = new Point(20, 45);
            searchOptionsPanel.Name = "searchOptionsPanel";
            searchOptionsPanel.Size = new Size(500, 25);
            searchOptionsPanel.TabIndex = 1;
            searchOptionsPanel.BackColor = Color.FromArgb(35, 35, 35);
            searchOptionsPanel.WrapContents = false;

            ConfigureSearchOption(btnCaseSensitive, "Aa", "Case sensitive search");
            ConfigureSearchOption(btnRegex, ".*", "Regular expression search");
            ConfigureSearchOption(btnWholeWord, "Ab", "Whole word search");

            searchStatusLabel.AutoSize = true;
            searchStatusLabel.Anchor = AnchorStyles.Top;
            searchStatusLabel.Font = ScaleFont(new Font("Segoe UI", 9F));
            searchStatusLabel.ForeColor = Color.FromArgb(176, 176, 176);
            searchStatusLabel.Location = new Point(650, 25);
            searchStatusLabel.Name = "searchStatusLabel";
            searchStatusLabel.Size = new Size(120, 20);
            searchStatusLabel.TabIndex = 2;
            searchStatusLabel.Text = "";
            searchStatusLabel.TextAlign = ContentAlignment.MiddleRight;

            btnClearLogs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearLogs.BackColor = Color.FromArgb(237, 66, 69);
            btnClearLogs.FlatAppearance.BorderSize = 0;
            btnClearLogs.FlatStyle = FlatStyle.Flat;
            btnClearLogs.Font = ScaleFont(new Font("Segoe UI", 9F, FontStyle.Bold));
            btnClearLogs.ForeColor = Color.White;
            btnClearLogs.Location = new Point(800, 15);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(110, 40);
            btnClearLogs.TabIndex = 3;
            btnClearLogs.Text = "CLEAR LOGS";
            btnClearLogs.UseVisualStyleBackColor = false;
            btnClearLogs.Cursor = Cursors.Hand;
            btnClearLogs.Click += (s, e) => {
                RTB_Logs.Clear();
                _searchManager.ClearSearch();
            };
            ConfigureGlowButton(btnClearLogs);
            CreateRoundedButton(btnClearLogs);

            RTB_Logs.BackColor = Color.FromArgb(28, 28, 28);
            RTB_Logs.BorderStyle = BorderStyle.None;
            RTB_Logs.Dock = DockStyle.Fill;
            RTB_Logs.Font = ScaleFont(new Font("Consolas", 10F));
            RTB_Logs.ForeColor = Color.FromArgb(224, 224, 224);
            RTB_Logs.Location = new Point(2, 2);
            RTB_Logs.Name = "RTB_Logs";
            RTB_Logs.ReadOnly = true;
            RTB_Logs.Size = new Size(956, 456);
            RTB_Logs.TabIndex = 0;
            RTB_Logs.Text = "";
            RTB_Logs.HideSelection = false;
            RTB_Logs.KeyDown += RTB_Logs_KeyDown;
            logsContainer.Controls.Add(RTB_Logs);

            // Add controls to logsHeaderPanel in correct order
            logsHeaderPanel.Controls.Add(searchPanel);
            logsHeaderPanel.Controls.Add(searchOptionsPanel);
            logsHeaderPanel.Controls.Add(searchStatusLabel);
            logsHeaderPanel.Controls.Add(btnClearLogs);

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

            this.Shown += (s, e) => {
                if (addBotPanel != null)
                {
                    var temp = addBotPanel.Width;
                    addBotPanel.Width = temp + 1;
                    addBotPanel.Width = temp;
                }

                if (logsHeaderPanel != null)
                {
                    logsHeaderPanel.PerformLayout();
                    logsHeaderPanel.Invalidate();

                    if (searchPanel != null)
                    {
                        searchPanel.Invalidate();
                        logSearchBox.Refresh();
                    }

                    if (btnClearLogs != null)
                    {
                        btnClearLogs.Invalidate();
                    }
                }
            };
        }

        #endregion

        #region Font Scaling

        private Font ScaleFont(Font baseFont)
        {
            using (Graphics g = CreateGraphics())
            {
                float dpiScale = g.DpiX / 96f;
                float scaledSize = baseFont.Size * dpiScale;

                if (ClientSize.Width < 900)
                {
                    scaledSize *= 0.85f;
                }
                else if (ClientSize.Width < 1100)
                {
                    scaledSize *= 0.92f;
                }

                scaledSize = Math.Max(7f, scaledSize);

                if (ClientSize.Width < 800)
                {
                    if (baseFont.Size >= 24)
                        scaledSize = Math.Min(scaledSize, 16f);
                    else if (baseFont.Size >= 11)
                        scaledSize = Math.Min(scaledSize, 9f);
                    else
                        scaledSize = Math.Min(scaledSize, 8f);
                }

                return new Font(baseFont.FontFamily, scaledSize, baseFont.Style);
            }
        }

        #endregion

        #region UI Helper Methods

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
                int rightMargin = 20;
                int minLeftPosition = titleLabel.Right + 20;

                int availableWidth = headerPanel.Width - minLeftPosition - rightMargin;

                if (availableWidth < 250)
                {
                    controlButtonsPanel.MaximumSize = new Size(250, 100);
                    controlButtonsPanel.WrapContents = true;
                }
                else
                {
                    controlButtonsPanel.MaximumSize = new Size(500, 50);
                    controlButtonsPanel.WrapContents = false;
                }

                int desiredX = headerPanel.Width - controlButtonsPanel.Width - rightMargin;
                controlButtonsPanel.Location = new Point(Math.Max(minLeftPosition, desiredX),
                    controlButtonsPanel.Height > 50 ? 15 : 30);
            }
        }

        private void ConfigureSearchOption(CheckBox checkBox, string text, string tooltip)
        {
            checkBox.Appearance = Appearance.Button;
            checkBox.BackColor = Color.FromArgb(40, 40, 40);
            checkBox.FlatAppearance.BorderSize = 1;
            checkBox.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            checkBox.FlatAppearance.CheckedBackColor = Color.FromArgb(88, 101, 242);
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.Font = ScaleFont(new Font("Segoe UI", 8F, FontStyle.Bold));
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
            btn.Font = ScaleFont(new Font("Segoe UI", 10F, FontStyle.Regular));
            btn.ForeColor = Color.FromArgb(176, 176, 176);
            btn.Location = new Point(0, 40 + (index * 60));
            btn.Margin = new Padding(0, 0, 0, 10);
            btn.Name = $"btnNav{text.Replace(" ", "")}";
            btn.Padding = new Padding(45, 0, 0, 0);
            btn.Size = new Size(240, 50);
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

                int iconSize = btn.Height < 45 ? 20 : 24;
                var iconRect = new Rectangle(12, (btn.Height - iconSize) / 2, iconSize, iconSize);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                float iconFontSize = btn.Height < 45 ? 14F : 16F;
                using var iconFont = new Font("Segoe MDL2 Assets", iconFontSize);
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

            float fontSize = 10F;
            using (Graphics g = btn.CreateGraphics())
            {
                float dpiScale = g.DpiX / 96f;
                fontSize = Math.Max(7F, 10F * dpiScale);
            }

            if (Width < 900)
                fontSize = Math.Max(7F, fontSize * 0.8f);
            else if (Width < 1100)
                fontSize = Math.Max(8F, fontSize * 0.9f);

            btn.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            btn.ForeColor = Color.FromArgb(28, 28, 28);
            btn.Margin = new Padding(3, 0, 3, 0);
            btn.Name = $"btn{text.Replace(" ", "")}";
            btn.Padding = new Padding(10, 5, 10, 5);
            btn.TabIndex = 0;
            btn.Text = text;
            btn.UseVisualStyleBackColor = false;
            btn.Tag = new ButtonAnimationState { BaseColor = baseColor };

            btn.AutoSize = true;
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn.MinimumSize = new Size(70, 32);
            btn.MaximumSize = new Size(160, 45);

            CreateRoundedButton(btn);
            ConfigureGlowButton(btn);

            btn.Resize += (s, e) => btn.Invalidate();
        }

        private void ConfigureNumericUpDown(NumericUpDown nud, int x, int y, int width)
        {
            nud.BackColor = Color.FromArgb(28, 28, 28);
            nud.BorderStyle = BorderStyle.None;
            nud.Font = ScaleFont(new Font("Segoe UI", 10F));
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
            cb.Font = ScaleFont(new Font("Segoe UI", 10F));
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
            btn.Paint += (s, e) => {
                if (btn.Region != null) btn.Region.Dispose();

                using var path = new GraphicsPath();
                var rect = btn.ClientRectangle;
                int radius = Math.Min(6, Math.Min(rect.Width, rect.Height) / 4);
                GraphicsExtensions.AddRoundedRectangle(path, rect, radius);
                btn.Region = new Region(path);
            };

            btn.Invalidate();
        }

        private void CreateCircularRegion(Control control)
        {
            using var path = new GraphicsPath();
            path.AddEllipse(0, 0, control.Width, control.Height);
            control.Region = new Region(path);
        }

        private void ConfigureUpdateButton()
        {
            statusIndicator.BackColor = Color.FromArgb(100, 100, 100);
            statusIndicator.Size = new Size(12, 12);
            statusIndicator.Location = new Point(btnUpdate.ClientSize.Width - 25, 15);
            statusIndicator.Name = "statusIndicator";
            statusIndicator.Enabled = false;
            statusIndicator.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CreateCircularRegion(statusIndicator);
            btnUpdate.Controls.Add(statusIndicator);
            statusIndicator.BringToFront();

            statusIndicator.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = statusIndicator.ClientRectangle;
                rect.Inflate(-1, -1);

                using var brush = new SolidBrush(statusIndicator.BackColor);
                e.Graphics.FillEllipse(brush, rect);

                var mainForm = (Main)statusIndicator.FindForm();
                if (mainForm != null && mainForm.hasUpdate)
                {
                    var highlightRect = new Rectangle(2, 2, 4, 4);
                    using var highlightBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
                    e.Graphics.FillEllipse(highlightBrush, highlightRect);
                }
            };

            var updateTooltip = new ToolTip();
            updateTooltip.SetToolTip(btnUpdate, "Check for updates");
            btnUpdate.MouseEnter += (s, e) => {
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

            statusIndicator.Location = new Point(btnUpdate.ClientSize.Width - 25, 15);

            btnUpdate.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                var animState = btnUpdate.Tag as ButtonAnimationState;

                if (animState != null && animState.HoverProgress > 0 && animState.IsHovering)
                {
                    using var glowBrush = new SolidBrush(Color.FromArgb((int)(20 * animState.HoverProgress), 88, 101, 242));
                    e.Graphics.FillRectangle(glowBrush, btnUpdate.ClientRectangle);
                }

                var iconColor = btnUpdate.ForeColor;
                if (animState != null && animState.HoverProgress > 0)
                {
                    iconColor = Color.FromArgb(
                        (int)(176 + (224 - 176) * animState.HoverProgress),
                        (int)(176 + (224 - 176) * animState.HoverProgress),
                        (int)(176 + (224 - 176) * animState.HoverProgress)
                    );
                }

                float iconFontSize = 14F;
                float textFontSize = 8.5F;
                int iconX = 15;

                if (btnUpdate.Width < 180)
                {
                    iconFontSize = 12F;
                    textFontSize = 7.5F;
                    iconX = 10;
                }

                using var iconFont = new Font("Segoe MDL2 Assets", iconFontSize);
                var iconText = "\uE895";

                using var iconBrush = new SolidBrush(iconColor);
                var iconSize = e.Graphics.MeasureString(iconText, iconFont);

                var iconY = (btnUpdate.Height - iconSize.Height) / 2;
                e.Graphics.DrawString(iconText, iconFont, iconBrush, iconX, iconY);

                using var textFont = ScaleFont(new Font("Segoe UI", textFontSize, FontStyle.Regular));
                var text = "CHECK FOR UPDATES";

                if (btnUpdate.Width < 170)
                    text = "UPDATES";

                var textSize = e.Graphics.MeasureString(text, textFont);
                var textX = iconX + iconSize.Width + 6;
                var textY = (btnUpdate.Height - textSize.Height) / 2;
                e.Graphics.DrawString(text, textFont, iconBrush, textX, textY);

                var mainForm = (Main)btnUpdate.FindForm();
                if (mainForm != null && mainForm.hasUpdate && statusIndicator != null)
                {
                    var indicatorBounds = new Rectangle(
                        statusIndicator.Left - 3,
                        statusIndicator.Top - 3,
                        statusIndicator.Width + 6,
                        statusIndicator.Height + 6
                    );

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
        }

        #endregion

        #region Paint Event Handlers

        private void LogoPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (_logoBrush == null || _logoBrush.Rectangle != logoPanel.ClientRectangle)
            {
                _logoBrush?.Dispose();
                _logoBrush = new LinearGradientBrush(
                    logoPanel.ClientRectangle,
                    Color.FromArgb(88, 101, 242),
                    Color.FromArgb(87, 242, 135),
                    LinearGradientMode.ForwardDiagonal);
            }

            e.Graphics.FillRectangle(_logoBrush, logoPanel.ClientRectangle);

            float fontSize = 22F;
            if (Width < 900)
                fontSize = 16F;
            else if (Width < 1100)
                fontSize = 19F;

            using var font = ScaleFont(new Font("Segoe UI", fontSize, FontStyle.Bold));
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
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (_currentModeImage != null)
            {
                var image = _currentModeImage;
                var panelWidth = FLP_Bots.ClientSize.Width;
                var panelHeight = FLP_Bots.ClientSize.Height;

                float scale = 1.0f;
                if (panelWidth < 600)
                    scale = 0.6f;
                else if (panelWidth < 800)
                    scale = 0.75f;
                else if (panelWidth < 1000)
                    scale = 0.85f;

                int imageWidth = (int)(image.Width * scale);
                int imageHeight = (int)(image.Height * scale);

                int x = (panelWidth - imageWidth) / 2;

                int y;

                if (FLP_Bots.Controls.Count > 0)
                {
                    int lastControlBottom = 0;
                    foreach (Control ctrl in FLP_Bots.Controls)
                    {
                        if (ctrl.Visible)
                        {
                            int ctrlBottom = ctrl.Bottom - FLP_Bots.VerticalScroll.Value;
                            if (ctrlBottom > lastControlBottom && ctrlBottom < panelHeight)
                                lastControlBottom = ctrlBottom;
                        }
                    }

                    y = lastControlBottom + 20;

                    if (y + imageHeight > panelHeight - 20)
                    {
                        y = panelHeight - imageHeight - 20;
                    }

                    var originalComposite = g.CompositingMode;
                    g.CompositingMode = CompositingMode.SourceOver;

                    using (var attributes = new ImageAttributes())
                    {
                        float[][] matrixItems = {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, 0.25f, 0},
                            new float[] {0, 0, 0, 0, 1}
                        };
                        var colorMatrix = new ColorMatrix(matrixItems);
                        attributes.SetColorMatrix(colorMatrix);

                        g.DrawImage(image,
                            new Rectangle(x, y, imageWidth, imageHeight),
                            0, 0, image.Width, image.Height,
                            GraphicsUnit.Pixel, attributes);
                    }

                    g.CompositingMode = originalComposite;
                }
                else
                {
                    y = panelHeight > 500 ? 80 : 40;

                    g.DrawImage(image, new Rectangle(x, y, imageWidth, imageHeight));

                    using var font = ScaleFont(new Font("Segoe UI", 14F, FontStyle.Regular));
                    using var brush = new SolidBrush(Color.FromArgb(100, 176, 176, 176));
                    var text = "No bots configured. Add a bot using the form above.";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, brush,
                        (panelWidth - size.Width) / 2,
                        y + imageHeight + 20);
                }
            }
            else if (FLP_Bots.Controls.Count == 0)
            {
                using var font = ScaleFont(new Font("Segoe UI", 14F, FontStyle.Regular));
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

            UpdateStatusIndicatorPulse();
        }

        private void TransitionPanels(int index)
        {
            botsPanel.Visible = false;
            hubPanel.Visible = false;
            logsPanel.Visible = false;

            contentPanel.Refresh();

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
                    logsPanel.Refresh();
                    break;
            }
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            var controls = container.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl)).Concat(controls);
        }

        #endregion

        #region System Tray

        private void ConfigureSystemTray()
        {
            trayIcon.Icon = Icon;
            trayIcon.Text = "PokéBot Control Center";
            trayIcon.Visible = false;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            trayContextMenu.BackColor = Color.FromArgb(35, 35, 35);
            trayContextMenu.Font = ScaleFont(new Font("Segoe UI", 10F));
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

        #endregion

        #region Custom Classes

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

        #region Controls Declaration

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

        private Button updater => btnUpdate;
        private Button B_Start => btnStart;
        private Button B_Stop => btnStop;
        private Button B_RebootStop => btnReboot;
        private TabControl TC_Main;
        private TabPage Tab_Bots;
        private TabPage Tab_Hub;
        private TabPage Tab_Logs;
        private Panel ButtonPanel => controlButtonsPanel;

        #endregion
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

#pragma warning restore CS8618
#pragma warning restore CS8625
#pragma warning restore CS8669
