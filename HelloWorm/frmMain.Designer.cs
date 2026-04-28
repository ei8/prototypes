namespace ei8.Prototypes.HelloWorm
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            statusStrip1 = new StatusStrip();
            tslblStatus = new ToolStripStatusLabel();
            tslblCount = new ToolStripStatusLabel();
            toolStripContainer1 = new ToolStripContainer();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem3 = new ToolStripMenuItem();
            mnuFileNewProject = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            mnuView = new ToolStripMenuItem();
            mnuViewResetPeripherals = new ToolStripMenuItem();
            mnuProject = new ToolStripMenuItem();
            mnuProjectAddDish = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            mnuProjectSettings = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            mnuToolsInitializeAvatar = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutHelloWormToolStripMenuItem = new ToolStripMenuItem();
            tsTemporal = new ToolStrip();
            tsbTemporalPlay = new ToolStripButton();
            tsbTemporalPause = new ToolStripButton();
            statusStrip1.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            menuStrip1.SuspendLayout();
            tsTemporal.SuspendLayout();
            SuspendLayout();
            // 
            // dockPanel1
            // 
            dockPanel1.Dock = DockStyle.Fill;
            dockPanel1.Location = new Point(0, 0);
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Size = new Size(800, 379);
            dockPanel1.TabIndex = 7;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tslblStatus, tslblCount });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 9;
            statusStrip1.Text = "statusStrip1";
            // 
            // tslblStatus
            // 
            tslblStatus.Name = "tslblStatus";
            tslblStatus.Size = new Size(785, 17);
            tslblStatus.Spring = true;
            // 
            // tslblCount
            // 
            tslblCount.Name = "tslblCount";
            tslblCount.Size = new Size(0, 17);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(dockPanel1);
            toolStripContainer1.ContentPanel.Size = new Size(800, 379);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(800, 428);
            toolStripContainer1.TabIndex = 10;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(menuStrip1);
            toolStripContainer1.TopToolStripPanel.Controls.Add(tsTemporal);
            // 
            // menuStrip1
            // 
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem3, mnuView, mnuProject, toolStripMenuItem1, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.DropDownItems.AddRange(new ToolStripItem[] { mnuFileNewProject, toolStripSeparator2, exitToolStripMenuItem });
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(37, 20);
            toolStripMenuItem3.Text = "&FIle";
            // 
            // mnuFileNewProject
            // 
            mnuFileNewProject.Image = (Image)resources.GetObject("mnuFileNewProject.Image");
            mnuFileNewProject.Name = "mnuFileNewProject";
            mnuFileNewProject.Size = new Size(180, 22);
            mnuFileNewProject.Text = "New &Project";
            mnuFileNewProject.Click += mnuFileNewProject_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewResetPeripherals });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(44, 20);
            mnuView.Text = "&View";
            // 
            // mnuViewResetPeripherals
            // 
            mnuViewResetPeripherals.Name = "mnuViewResetPeripherals";
            mnuViewResetPeripherals.Size = new Size(180, 22);
            mnuViewResetPeripherals.Text = "&Reset Peripherals";
            mnuViewResetPeripherals.Click += mnuViewResetPeripherals_Click;
            // 
            // mnuProject
            // 
            mnuProject.DropDownItems.AddRange(new ToolStripItem[] { mnuProjectAddDish, toolStripSeparator1, mnuProjectSettings });
            mnuProject.Name = "mnuProject";
            mnuProject.Size = new Size(56, 20);
            mnuProject.Text = "&Project";
            // 
            // mnuProjectAddDish
            // 
            mnuProjectAddDish.Image = (Image)resources.GetObject("mnuProjectAddDish.Image");
            mnuProjectAddDish.Name = "mnuProjectAddDish";
            mnuProjectAddDish.Size = new Size(180, 22);
            mnuProjectAddDish.Text = "Add &Dish";
            mnuProjectAddDish.Click += mnuProjectAddDish_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // mnuProjectSettings
            // 
            mnuProjectSettings.Name = "mnuProjectSettings";
            mnuProjectSettings.Size = new Size(180, 22);
            mnuProjectSettings.Text = "&Settings";
            mnuProjectSettings.Click += mnuProjectSettings_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { mnuToolsInitializeAvatar });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(46, 20);
            toolStripMenuItem1.Text = "&Tools";
            // 
            // mnuToolsInitializeAvatar
            // 
            mnuToolsInitializeAvatar.Name = "mnuToolsInitializeAvatar";
            mnuToolsInitializeAvatar.Size = new Size(180, 22);
            mnuToolsInitializeAvatar.Text = "&Initialize Avatar...";
            mnuToolsInitializeAvatar.Click += mnuToolsInitializeAvatar_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutHelloWormToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutHelloWormToolStripMenuItem
            // 
            aboutHelloWormToolStripMenuItem.Name = "aboutHelloWormToolStripMenuItem";
            aboutHelloWormToolStripMenuItem.Size = new Size(174, 22);
            aboutHelloWormToolStripMenuItem.Text = "&About Hello Worm";
            // 
            // tsTemporal
            // 
            tsTemporal.Dock = DockStyle.None;
            tsTemporal.Items.AddRange(new ToolStripItem[] { tsbTemporalPlay, tsbTemporalPause });
            tsTemporal.Location = new Point(3, 24);
            tsTemporal.Name = "tsTemporal";
            tsTemporal.Size = new Size(58, 25);
            tsTemporal.TabIndex = 7;
            // 
            // tsbTemporalPlay
            // 
            tsbTemporalPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbTemporalPlay.Image = (Image)resources.GetObject("tsbTemporalPlay.Image");
            tsbTemporalPlay.ImageTransparentColor = Color.Magenta;
            tsbTemporalPlay.Name = "tsbTemporalPlay";
            tsbTemporalPlay.Size = new Size(23, 22);
            tsbTemporalPlay.Text = "Play";
            tsbTemporalPlay.Click += tsbTemporalPlay_Click;
            // 
            // tsbTemporalPause
            // 
            tsbTemporalPause.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbTemporalPause.Image = (Image)resources.GetObject("tsbTemporalPause.Image");
            tsbTemporalPause.ImageTransparentColor = Color.Magenta;
            tsbTemporalPause.Name = "tsbTemporalPause";
            tsbTemporalPause.Size = new Size(23, 22);
            tsbTemporalPause.Text = "toolStripButton1";
            tsbTemporalPause.Click += tsbTemporalPause_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStripContainer1);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            Name = "frmMain";
            Text = "Hello Worm - ei8.io";
            Load += frmMain_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tsTemporal.ResumeLayout(false);
            tsTemporal.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private StatusStrip statusStrip1;
        private ToolStripContainer toolStripContainer1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem mnuToolsInitializeAvatar;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutHelloWormToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripStatusLabel tslblStatus;
        private ToolStripStatusLabel tslblCount;
        private ToolStrip tsTemporal;
        private ToolStripButton tsbTemporalPlay;
        private ToolStripButton tsbTemporalPause;
        private ToolStripMenuItem mnuProject;
        private ToolStripMenuItem mnuProjectSettings;
        private ToolStripMenuItem mnuProjectAddDish;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuFileNewProject;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem mnuViewResetPeripherals;
    }
}