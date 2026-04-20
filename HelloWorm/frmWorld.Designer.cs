using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using neurUL.Common.Domain.Model;
using static ei8.Prototypes.HelloWorm.Constants;

namespace ei8.Prototypes.HelloWorm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            objectsToolStripMenuItem = new ToolStripMenuItem();
            mnuObjectsCreate = new ToolStripMenuItem();
            mnuObjectsCreateFood = new ToolStripMenuItem();
            mnuObjectsCreateWorm = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            worldToolStripMenuItem = new ToolStripMenuItem();
            mnuSettingsWorldRegenerate = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            mnuToolsInitializeAvatar = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutHelloWormToolStripMenuItem = new ToolStripMenuItem();
            worldPanel = new WorldPanel();
            statusStrip1 = new StatusStrip();
            tlblCount = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { objectsToolStripMenuItem, toolStripMenuItem2, settingsToolStripMenuItem, toolStripMenuItem1, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // objectsToolStripMenuItem
            // 
            objectsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuObjectsCreate });
            objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
            objectsToolStripMenuItem.Size = new Size(59, 20);
            objectsToolStripMenuItem.Text = "&Objects";
            // 
            // mnuObjectsCreate
            // 
            mnuObjectsCreate.DropDownItems.AddRange(new ToolStripItem[] { mnuObjectsCreateFood, mnuObjectsCreateWorm });
            mnuObjectsCreate.Name = "mnuObjectsCreate";
            mnuObjectsCreate.Size = new Size(180, 22);
            mnuObjectsCreate.Text = "&Add";
            // 
            // mnuObjectsCreateFood
            // 
            mnuObjectsCreateFood.Name = "mnuObjectsCreateFood";
            mnuObjectsCreateFood.Size = new Size(180, 22);
            mnuObjectsCreateFood.Text = "&Food";
            mnuObjectsCreateFood.Click += mnuObjectsCreateFood_Click;
            // 
            // mnuObjectsCreateWorm
            // 
            mnuObjectsCreateWorm.Name = "mnuObjectsCreateWorm";
            mnuObjectsCreateWorm.Size = new Size(180, 22);
            mnuObjectsCreateWorm.Text = "&Worm";
            mnuObjectsCreateWorm.Click += mnuObjectsCreateWorm_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(44, 20);
            toolStripMenuItem2.Text = "&View";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { worldToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(61, 20);
            settingsToolStripMenuItem.Text = "&Settings";
            // 
            // worldToolStripMenuItem
            // 
            worldToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuSettingsWorldRegenerate });
            worldToolStripMenuItem.Name = "worldToolStripMenuItem";
            worldToolStripMenuItem.Size = new Size(180, 22);
            worldToolStripMenuItem.Text = "&World";
            // 
            // mnuSettingsWorldRegenerate
            // 
            mnuSettingsWorldRegenerate.Checked = true;
            mnuSettingsWorldRegenerate.CheckState = CheckState.Checked;
            mnuSettingsWorldRegenerate.Name = "mnuSettingsWorldRegenerate";
            mnuSettingsWorldRegenerate.Size = new Size(133, 22);
            mnuSettingsWorldRegenerate.Text = "&Regenerate";
            mnuSettingsWorldRegenerate.Click += mnuSettingsWorldRegenerate_Click;
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
            mnuToolsInitializeAvatar.Click += this.mnuToolsInitializeAvatar_Click;
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
            aboutHelloWormToolStripMenuItem.Size = new Size(180, 22);
            aboutHelloWormToolStripMenuItem.Text = "&About Hello Worm";
            // 
            // worldPanel
            // 
            worldPanel.Dock = DockStyle.Fill;
            worldPanel.Location = new Point(0, 24);
            worldPanel.Name = "worldPanel";
            worldPanel.Size = new Size(800, 404);
            worldPanel.TabIndex = 1;
            worldPanel.World = null;
            worldPanel.DoubleClick += WorldPanel_DoubleClick;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tlblCount });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tlblCount
            // 
            tlblCount.Name = "tlblCount";
            tlblCount.Size = new Size(0, 17);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(worldPanel);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Hello Worm";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private MenuStrip menuStrip1;
        private WorldPanel worldPanel;
        private System.Windows.Forms.Timer timer1;
        private ToolStripMenuItem objectsToolStripMenuItem;
        private ToolStripMenuItem mnuObjectsCreate;
        private ToolStripMenuItem mnuObjectsCreateWorm;
        private ToolStripMenuItem mnuObjectsCreateFood;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem worldToolStripMenuItem;
        private ToolStripMenuItem mnuSettingsWorldRegenerate;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutHelloWormToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem mnuToolsInitializeAvatar;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tlblCount;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}
