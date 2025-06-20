using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SysBot.Pokemon.WinForms
{
    partial class BotController
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.L_Description = new System.Windows.Forms.Label();
            this.L_Left = new System.Windows.Forms.Label();
            this.PB_Lamp = new System.Windows.Forms.PictureBox();
            this.RCMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusPanel = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.Panel();
            this.actionButton = new System.Windows.Forms.Button();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Lamp)).BeginInit();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();

            // BotController
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            this.ContextMenuStrip = this.RCMenu;
            this.Controls.Add(this.actionButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.L_Description);
            this.Controls.Add(this.L_Left);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.Name = "BotController";
            this.Size = new System.Drawing.Size(900, 80);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BotController_Paint);
            this.MouseEnter += new System.EventHandler(this.BotController_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.BotController_MouseLeave);

            // Status Panel
            this.statusPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusPanel.Controls.Add(this.PB_Lamp);
            this.statusPanel.Location = new System.Drawing.Point(15, 15);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(50, 50);
            this.statusPanel.TabIndex = 5;
            this.statusPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.StatusPanel_Paint);

            // Status Lamp
            this.PB_Lamp.BackColor = System.Drawing.Color.FromArgb(87, 242, 135);
            this.PB_Lamp.Location = new System.Drawing.Point(15, 15);
            this.PB_Lamp.Name = "PB_Lamp";
            this.PB_Lamp.Size = new System.Drawing.Size(20, 20);
            this.PB_Lamp.TabIndex = 4;
            this.PB_Lamp.TabStop = false;
            this.PB_Lamp.Paint += new System.Windows.Forms.PaintEventHandler(this.PB_Lamp_Paint);

            // Left Label (IP/Routine)
            this.L_Left.AutoSize = false;
            this.L_Left.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.L_Left.ForeColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.L_Left.Location = new System.Drawing.Point(80, 15);
            this.L_Left.Name = "L_Left";
            this.L_Left.Size = new System.Drawing.Size(200, 50);
            this.L_Left.TabIndex = 3;
            this.L_Left.Text = "192.168.123.123\r\nFlexTrade";
            this.L_Left.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Description Label
            this.L_Description.Anchor = System.Windows.Forms.AnchorStyles.Top |
                                        System.Windows.Forms.AnchorStyles.Left |
                                        System.Windows.Forms.AnchorStyles.Right;
            this.L_Description.AutoEllipsis = true;
            this.L_Description.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.L_Description.ForeColor = System.Drawing.Color.FromArgb(176, 176, 176);
            this.L_Description.Location = new System.Drawing.Point(290, 25);
            this.L_Description.Name = "L_Description";
            this.L_Description.Size = new System.Drawing.Size(450, 30);
            this.L_Description.TabIndex = 2;
            this.L_Description.Text = "Waiting for command...";
            this.L_Description.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Progress Bar
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                                     System.Windows.Forms.AnchorStyles.Left |
                                     System.Windows.Forms.AnchorStyles.Right;
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.progressBar.Location = new System.Drawing.Point(80, 65);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(660, 4);
            this.progressBar.TabIndex = 6;
            this.progressBar.Paint += new System.Windows.Forms.PaintEventHandler(this.ProgressBar_Paint);

            // Action Button
            this.actionButton.Anchor = System.Windows.Forms.AnchorStyles.Top |
                                      System.Windows.Forms.AnchorStyles.Right;
            this.actionButton.BackColor = System.Drawing.Color.FromArgb(88, 101, 242);
            this.actionButton.FlatAppearance.BorderSize = 0;
            this.actionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.actionButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.actionButton.ForeColor = System.Drawing.Color.White;
            this.actionButton.Location = new System.Drawing.Point(760, 25);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(100, 30);
            this.actionButton.TabIndex = 7;
            this.actionButton.Text = "ACTIONS";
            this.actionButton.UseVisualStyleBackColor = false;
            this.actionButton.Click += new System.EventHandler(this.ActionButton_Click);
            this.actionButton.Paint += new System.Windows.Forms.PaintEventHandler(this.ActionButton_Paint);

            // Context Menu
            this.RCMenu.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            this.RCMenu.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.RCMenu.Name = "RCMenu";
            this.RCMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.RCMenu.ShowImageMargin = false;
            this.RCMenu.ShowItemToolTips = false;
            this.RCMenu.Size = new System.Drawing.Size(150, 4);

            // Animation Timer
            this.animationTimer.Interval = 32;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            this.animationTimer.Enabled = true;

            ((System.ComponentModel.ISupportInitialize)(this.PB_Lamp)).EndInit();
            this.statusPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label L_Description;
        private System.Windows.Forms.Label L_Left;
        private System.Windows.Forms.PictureBox PB_Lamp;
        private System.Windows.Forms.ContextMenuStrip RCMenu;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Panel progressBar;
        private System.Windows.Forms.Button actionButton;
        private System.Windows.Forms.Timer animationTimer;
    }
}
