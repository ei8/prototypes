namespace ei8.Prototypes.HelloWorm
{
    partial class frmToolbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmToolbox));
            toolStrip1 = new ToolStrip();
            tsbFood = new ToolStripButton();
            tsbWorm = new ToolStripButton();
            tslblNoUsableControls = new ToolStripLabel();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Fill;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbFood, tsbWorm, tslblNoUsableControls });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 450);
            toolStrip1.TabIndex = 1;
            // 
            // tsbFood
            // 
            tsbFood.DoubleClickEnabled = true;
            tsbFood.Image = (Image)resources.GetObject("tsbFood.Image");
            tsbFood.ImageAlign = ContentAlignment.MiddleLeft;
            tsbFood.ImageTransparentColor = Color.Magenta;
            tsbFood.Name = "tsbFood";
            tsbFood.Size = new Size(798, 20);
            tsbFood.Text = "Food";
            tsbFood.DoubleClick += tsbFood_DoubleClick;
            // 
            // tsbWorm
            // 
            tsbWorm.DoubleClickEnabled = true;
            tsbWorm.Image = (Image)resources.GetObject("tsbWorm.Image");
            tsbWorm.ImageAlign = ContentAlignment.MiddleLeft;
            tsbWorm.ImageTransparentColor = Color.Magenta;
            tsbWorm.Name = "tsbWorm";
            tsbWorm.Size = new Size(798, 20);
            tsbWorm.Text = "Worm";
            tsbWorm.DoubleClick += tsbWorm_DoubleClick;
            // 
            // tslblNoUsableControls
            // 
            tslblNoUsableControls.Name = "tslblNoUsableControls";
            tslblNoUsableControls.Size = new Size(798, 15);
            tslblNoUsableControls.Text = "No usable controls.";
            // 
            // frmToolbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStrip1);
            Name = "frmToolbox";
            Text = "Toolbox";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton tsbFood;
        private ToolStripButton tsbWorm;
        private ToolStripLabel tslblNoUsableControls;
    }
}