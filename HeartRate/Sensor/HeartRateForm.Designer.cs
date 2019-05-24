namespace HeartRateSensor
{
    partial class HeartRateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.uxBpmNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.uxNotifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectIconFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFontColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editIconFontWarningColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectWindowFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWindowFontColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWindowFontWarningColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.uxEditSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uxExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uxBpmLabel = new System.Windows.Forms.Label();
            this.avatarTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.authorTextBox = new System.Windows.Forms.TextBox();
            this.sendToExobrainCheckBox = new System.Windows.Forms.CheckBox();
            this.uxNotifyIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxBpmNotifyIcon
            // 
            this.uxBpmNotifyIcon.ContextMenuStrip = this.uxNotifyIconContextMenu;
            this.uxBpmNotifyIcon.Text = "notifyIcon1";
            this.uxBpmNotifyIcon.Visible = true;
            this.uxBpmNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.uxBpmNotifyIcon_MouseClick);
            // 
            // uxNotifyIconContextMenu
            // 
            this.uxNotifyIconContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uxNotifyIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectIconFontToolStripMenuItem,
            this.editFontColorToolStripMenuItem,
            this.editIconFontWarningColorToolStripMenuItem,
            this.toolStripMenuItem2,
            this.selectWindowFontToolStripMenuItem,
            this.editWindowFontColorToolStripMenuItem,
            this.editWindowFontWarningColorToolStripMenuItem,
            this.toolStripMenuItem1,
            this.uxEditSettingsMenuItem,
            this.uxExitMenuItem});
            this.uxNotifyIconContextMenu.Name = "uxNotifyIconContextMenu";
            this.uxNotifyIconContextMenu.Size = new System.Drawing.Size(250, 192);
            // 
            // selectIconFontToolStripMenuItem
            // 
            this.selectIconFontToolStripMenuItem.Name = "selectIconFontToolStripMenuItem";
            this.selectIconFontToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.selectIconFontToolStripMenuItem.Text = "Select icon font...";
            this.selectIconFontToolStripMenuItem.Click += new System.EventHandler(this.selectIconFontToolStripMenuItem_Click);
            // 
            // editFontColorToolStripMenuItem
            // 
            this.editFontColorToolStripMenuItem.Name = "editFontColorToolStripMenuItem";
            this.editFontColorToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.editFontColorToolStripMenuItem.Text = "Edit icon font color...";
            this.editFontColorToolStripMenuItem.Click += new System.EventHandler(this.editFontColorToolStripMenuItem_Click);
            // 
            // editIconFontWarningColorToolStripMenuItem
            // 
            this.editIconFontWarningColorToolStripMenuItem.Name = "editIconFontWarningColorToolStripMenuItem";
            this.editIconFontWarningColorToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.editIconFontWarningColorToolStripMenuItem.Text = "Edit icon font warning color...";
            this.editIconFontWarningColorToolStripMenuItem.Click += new System.EventHandler(this.editIconFontWarningColorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(246, 6);
            // 
            // selectWindowFontToolStripMenuItem
            // 
            this.selectWindowFontToolStripMenuItem.Name = "selectWindowFontToolStripMenuItem";
            this.selectWindowFontToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.selectWindowFontToolStripMenuItem.Text = "Select window font...";
            this.selectWindowFontToolStripMenuItem.Click += new System.EventHandler(this.selectWindowFontToolStripMenuItem_Click);
            // 
            // editWindowFontColorToolStripMenuItem
            // 
            this.editWindowFontColorToolStripMenuItem.Name = "editWindowFontColorToolStripMenuItem";
            this.editWindowFontColorToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.editWindowFontColorToolStripMenuItem.Text = "Edit window font color...";
            this.editWindowFontColorToolStripMenuItem.Click += new System.EventHandler(this.editWindowFontColorToolStripMenuItem_Click);
            // 
            // editWindowFontWarningColorToolStripMenuItem
            // 
            this.editWindowFontWarningColorToolStripMenuItem.Name = "editWindowFontWarningColorToolStripMenuItem";
            this.editWindowFontWarningColorToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.editWindowFontWarningColorToolStripMenuItem.Text = "Edit window font warning color...";
            this.editWindowFontWarningColorToolStripMenuItem.Click += new System.EventHandler(this.editWindowFontWarningColorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(246, 6);
            // 
            // uxEditSettingsMenuItem
            // 
            this.uxEditSettingsMenuItem.Name = "uxEditSettingsMenuItem";
            this.uxEditSettingsMenuItem.Size = new System.Drawing.Size(249, 22);
            this.uxEditSettingsMenuItem.Text = "Edit settings XML...";
            this.uxEditSettingsMenuItem.Click += new System.EventHandler(this.uxMenuEditSettings_Click);
            // 
            // uxExitMenuItem
            // 
            this.uxExitMenuItem.Name = "uxExitMenuItem";
            this.uxExitMenuItem.Size = new System.Drawing.Size(249, 22);
            this.uxExitMenuItem.Text = "Exit";
            this.uxExitMenuItem.Click += new System.EventHandler(this.uxExitMenuItem_Click);
            // 
            // uxBpmLabel
            // 
            this.uxBpmLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxBpmLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uxBpmLabel.Location = new System.Drawing.Point(0, 0);
            this.uxBpmLabel.Name = "uxBpmLabel";
            this.uxBpmLabel.Size = new System.Drawing.Size(307, 106);
            this.uxBpmLabel.TabIndex = 0;
            this.uxBpmLabel.Text = "Starting...";
            // 
            // avatarTextBox
            // 
            this.avatarTextBox.Location = new System.Drawing.Point(78, 111);
            this.avatarTextBox.Name = "avatarTextBox";
            this.avatarTextBox.ReadOnly = true;
            this.avatarTextBox.Size = new System.Drawing.Size(218, 20);
            this.avatarTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Avatar";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Author";
            // 
            // authorTextBox
            // 
            this.authorTextBox.Location = new System.Drawing.Point(78, 137);
            this.authorTextBox.Name = "authorTextBox";
            this.authorTextBox.ReadOnly = true;
            this.authorTextBox.Size = new System.Drawing.Size(218, 20);
            this.authorTextBox.TabIndex = 3;
            // 
            // sendToExobrainCheckBox
            // 
            this.sendToExobrainCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.sendToExobrainCheckBox.AutoSize = true;
            this.sendToExobrainCheckBox.Location = new System.Drawing.Point(198, 163);
            this.sendToExobrainCheckBox.Name = "sendToExobrainCheckBox";
            this.sendToExobrainCheckBox.Size = new System.Drawing.Size(98, 23);
            this.sendToExobrainCheckBox.TabIndex = 6;
            this.sendToExobrainCheckBox.Text = "Send to Exobrain";
            this.sendToExobrainCheckBox.UseVisualStyleBackColor = true;
            // 
            // HeartRateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 198);
            this.Controls.Add(this.sendToExobrainCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.authorTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.avatarTextBox);
            this.Controls.Add(this.uxBpmLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "HeartRateForm";
            this.ShowInTaskbar = false;
            this.Text = "Heart Rate Sensor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HeartRateForm_FormClosing);
            this.Load += new System.EventHandler(this.HeartRateForm_Load);
            this.ResizeEnd += new System.EventHandler(this.HeartRateForm_ResizeEnd);
            this.uxNotifyIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon uxBpmNotifyIcon;
        private System.Windows.Forms.Label uxBpmLabel;
        private System.Windows.Forms.ContextMenuStrip uxNotifyIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem uxEditSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uxExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFontColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editIconFontWarningColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWindowFontColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWindowFontWarningColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem selectIconFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem selectWindowFontToolStripMenuItem;
        private System.Windows.Forms.TextBox avatarTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox authorTextBox;
        private System.Windows.Forms.CheckBox sendToExobrainCheckBox;
    }
}

