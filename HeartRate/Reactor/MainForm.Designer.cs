namespace Reactor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.spikeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.reactiveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.enableLogsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.spikeCountToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDendritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPropertiesWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewResultMarkersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSpikeTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.viewFiredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTriggeredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.loadedInfoToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.graphCountToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.refractoryPeriodToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.refractoryPeriodToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.spacerToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spikeToolStripButton,
            this.reactiveToolStripButton,
            this.enableLogsToolStripButton,
            this.toolStripLabel1,
            this.spikeCountToolStripTextBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(949, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // spikeToolStripButton
            // 
            this.spikeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.spikeToolStripButton.Enabled = false;
            this.spikeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("spikeToolStripButton.Image")));
            this.spikeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.spikeToolStripButton.Name = "spikeToolStripButton";
            this.spikeToolStripButton.Size = new System.Drawing.Size(39, 22);
            this.spikeToolStripButton.Text = "Spike";
            this.spikeToolStripButton.Click += new System.EventHandler(this.spikeButton_Click);
            // 
            // reactiveToolStripButton
            // 
            this.reactiveToolStripButton.CheckOnClick = true;
            this.reactiveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.reactiveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("reactiveToolStripButton.Image")));
            this.reactiveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reactiveToolStripButton.Name = "reactiveToolStripButton";
            this.reactiveToolStripButton.Size = new System.Drawing.Size(55, 22);
            this.reactiveToolStripButton.Text = "Reactive";
            // 
            // enableLogsToolStripButton
            // 
            this.enableLogsToolStripButton.Checked = true;
            this.enableLogsToolStripButton.CheckOnClick = true;
            this.enableLogsToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableLogsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.enableLogsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("enableLogsToolStripButton.Image")));
            this.enableLogsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.enableLogsToolStripButton.Name = "enableLogsToolStripButton";
            this.enableLogsToolStripButton.Size = new System.Drawing.Size(80, 22);
            this.enableLogsToolStripButton.Text = "Debug Mode";
            this.enableLogsToolStripButton.Click += new System.EventHandler(this.enableLogsToolStripButton_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(71, 22);
            this.toolStripLabel1.Text = "Spike Count";
            // 
            // spikeCountToolStripTextBox
            // 
            this.spikeCountToolStripTextBox.Name = "spikeCountToolStripTextBox";
            this.spikeCountToolStripTextBox.Size = new System.Drawing.Size(76, 25);
            this.spikeCountToolStripTextBox.Text = "1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(949, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewGraphToolStripMenuItem,
            this.toolStripSeparator3,
            this.viewServersToolStripMenuItem,
            this.viewDendritesToolStripMenuItem,
            this.viewPropertiesWindowToolStripMenuItem,
            this.viewResultMarkersToolStripMenuItem,
            this.viewSpikeTargetsToolStripMenuItem,
            this.toolStripSeparator4,
            this.viewFiredToolStripMenuItem,
            this.viewLogsToolStripMenuItem,
            this.viewTriggeredToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // viewGraphToolStripMenuItem
            // 
            this.viewGraphToolStripMenuItem.Name = "viewGraphToolStripMenuItem";
            this.viewGraphToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewGraphToolStripMenuItem.Text = "&Graph";
            this.viewGraphToolStripMenuItem.Click += new System.EventHandler(this.viewGraphToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(171, 6);
            // 
            // viewServersToolStripMenuItem
            // 
            this.viewServersToolStripMenuItem.Name = "viewServersToolStripMenuItem";
            this.viewServersToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewServersToolStripMenuItem.Text = "&Servers";
            this.viewServersToolStripMenuItem.Click += new System.EventHandler(this.viewServersToolStripMenuItem_Click);
            // 
            // viewDendritesToolStripMenuItem
            // 
            this.viewDendritesToolStripMenuItem.Name = "viewDendritesToolStripMenuItem";
            this.viewDendritesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewDendritesToolStripMenuItem.Text = "&Dendrites";
            this.viewDendritesToolStripMenuItem.Click += new System.EventHandler(this.viewDendritesToolStripMenuItem_Click);
            // 
            // viewPropertiesWindowToolStripMenuItem
            // 
            this.viewPropertiesWindowToolStripMenuItem.Name = "viewPropertiesWindowToolStripMenuItem";
            this.viewPropertiesWindowToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewPropertiesWindowToolStripMenuItem.Text = "Pr&operties Window";
            this.viewPropertiesWindowToolStripMenuItem.Click += new System.EventHandler(this.viewPropertiesWindowToolStripMenuItem_Click);
            // 
            // viewResultMarkersToolStripMenuItem
            // 
            this.viewResultMarkersToolStripMenuItem.Name = "viewResultMarkersToolStripMenuItem";
            this.viewResultMarkersToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewResultMarkersToolStripMenuItem.Text = "&Result Markers";
            this.viewResultMarkersToolStripMenuItem.Click += new System.EventHandler(this.viewResultMarkersToolStripMenuItem_Click);
            // 
            // viewSpikeTargetsToolStripMenuItem
            // 
            this.viewSpikeTargetsToolStripMenuItem.Name = "viewSpikeTargetsToolStripMenuItem";
            this.viewSpikeTargetsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewSpikeTargetsToolStripMenuItem.Text = "S&pike Targets";
            this.viewSpikeTargetsToolStripMenuItem.Click += new System.EventHandler(this.viewSpikeTargetsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(171, 6);
            // 
            // viewFiredToolStripMenuItem
            // 
            this.viewFiredToolStripMenuItem.Name = "viewFiredToolStripMenuItem";
            this.viewFiredToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewFiredToolStripMenuItem.Text = "&Fired";
            this.viewFiredToolStripMenuItem.Click += new System.EventHandler(this.viewFiredToolStripMenuItem_Click);
            // 
            // viewLogsToolStripMenuItem
            // 
            this.viewLogsToolStripMenuItem.Name = "viewLogsToolStripMenuItem";
            this.viewLogsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewLogsToolStripMenuItem.Text = "&Logs";
            this.viewLogsToolStripMenuItem.Click += new System.EventHandler(this.viewLogsToolStripMenuItem_Click);
            // 
            // viewTriggeredToolStripMenuItem
            // 
            this.viewTriggeredToolStripMenuItem.Name = "viewTriggeredToolStripMenuItem";
            this.viewTriggeredToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewTriggeredToolStripMenuItem.Text = "&Triggered";
            this.viewTriggeredToolStripMenuItem.Click += new System.EventHandler(this.viewTriggeredToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(47, 20);
            this.toolStripMenuItem1.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.aboutToolStripMenuItem.Text = "&About Reactor";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadedInfoToolStripStatusLabel,
            this.graphCountToolStripStatusLabel,
            this.refractoryPeriodToolStripStatusLabel,
            this.refractoryPeriodToolStripProgressBar,
            this.spacerToolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 565);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(949, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // loadedInfoToolStripStatusLabel
            // 
            this.loadedInfoToolStripStatusLabel.Name = "loadedInfoToolStripStatusLabel";
            this.loadedInfoToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // graphCountToolStripStatusLabel
            // 
            this.graphCountToolStripStatusLabel.Name = "graphCountToolStripStatusLabel";
            this.graphCountToolStripStatusLabel.Size = new System.Drawing.Size(750, 17);
            this.graphCountToolStripStatusLabel.Spring = true;
            this.graphCountToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // refractoryPeriodToolStripStatusLabel
            // 
            this.refractoryPeriodToolStripStatusLabel.Name = "refractoryPeriodToolStripStatusLabel";
            this.refractoryPeriodToolStripStatusLabel.Size = new System.Drawing.Size(101, 17);
            this.refractoryPeriodToolStripStatusLabel.Text = "Refractory Period:";
            // 
            // refractoryPeriodToolStripProgressBar
            // 
            this.refractoryPeriodToolStripProgressBar.Name = "refractoryPeriodToolStripProgressBar";
            this.refractoryPeriodToolStripProgressBar.Size = new System.Drawing.Size(75, 16);
            // 
            // spacerToolStripStatusLabel
            // 
            this.spacerToolStripStatusLabel.Name = "spacerToolStripStatusLabel";
            this.spacerToolStripStatusLabel.Size = new System.Drawing.Size(10, 17);
            this.spacerToolStripStatusLabel.Text = " ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 587);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reactor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton spikeToolStripButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox spikeCountToolStripTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel loadedInfoToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewTriggeredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewFiredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem viewDendritesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem viewResultMarkersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewServersToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton enableLogsToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem viewSpikeTargetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPropertiesWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel graphCountToolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar refractoryPeriodToolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel spacerToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel refractoryPeriodToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton reactiveToolStripButton;
    }
}

