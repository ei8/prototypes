using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using neurUL.Common.Domain.Model;
using static ei8.Prototypes.HelloWorm.Constants;

namespace ei8.Prototypes.HelloWorm
{
    partial class frmWorld
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
            worldPanel = new WorldPanel();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // worldPanel
            // 
            worldPanel.Dock = DockStyle.Fill;
            worldPanel.Location = new Point(0, 0);
            worldPanel.Name = "worldPanel";
            worldPanel.Size = new Size(800, 450);
            worldPanel.TabIndex = 1;
            worldPanel.World = null;
            worldPanel.DoubleClick += WorldPanel_DoubleClick;
            // 
            // frmWorld
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(worldPanel);
            Name = "frmWorld";
            Text = "Dish";
            Load += frmWorld_Load;
            ResumeLayout(false);
        }

        #endregion
        private WorldPanel worldPanel;
        private System.Windows.Forms.Timer timer1;
    }
}
