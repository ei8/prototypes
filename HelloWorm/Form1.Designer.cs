
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
            wormToolStripMenuItem = new ToolStripMenuItem();
            createToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            mnuWormInitialize = new ToolStripMenuItem();
            worldPanel = new WorldPanel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { wormToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // wormToolStripMenuItem
            // 
            wormToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createToolStripMenuItem, toolStripMenuItem1, mnuWormInitialize });
            wormToolStripMenuItem.Name = "wormToolStripMenuItem";
            wormToolStripMenuItem.Size = new Size(52, 20);
            wormToolStripMenuItem.Text = "Worm";
            // 
            // createToolStripMenuItem
            // 
            createToolStripMenuItem.Name = "createToolStripMenuItem";
            createToolStripMenuItem.Size = new Size(117, 22);
            createToolStripMenuItem.Text = "Create";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(114, 6);
            // 
            // mnuWormInitialize
            // 
            mnuWormInitialize.Name = "mnuWormInitialize";
            mnuWormInitialize.Size = new Size(117, 22);
            mnuWormInitialize.Text = "Initialize";
            mnuWormInitialize.Click += mnuWormInitialize_Click;
            // 
            // panel1
            // 
            worldPanel.Dock = DockStyle.Fill;
            worldPanel.Location = new Point(0, 24);
            worldPanel.Name = "panel1";
            worldPanel.Size = new Size(800, 426);
            worldPanel.TabIndex = 1;
            worldPanel.DoubleClick += this.WorldPanel_DoubleClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(worldPanel);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Hello Worm";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem wormToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuWormInitialize;
        private WorldPanel worldPanel;
        private System.Windows.Forms.Timer timer1;
    }
}
