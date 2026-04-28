namespace ei8.Prototypes.HelloWorm
{
    partial class frmProjectExplorer
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProjectExplorer));
            treeView = new TreeView();
            imageList1 = new ImageList(components);
            toolStrip1 = new ToolStrip();
            tsbReload = new ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView
            // 
            treeView.Dock = DockStyle.Fill;
            treeView.ImageIndex = 0;
            treeView.ImageList = imageList1;
            treeView.Indent = 19;
            treeView.Location = new Point(0, 25);
            treeView.Name = "treeView";
            treeView.SelectedImageIndex = 0;
            treeView.ShowRootLines = false;
            treeView.Size = new Size(800, 425);
            treeView.TabIndex = 0;
            treeView.NodeMouseClick += treeView_NodeMouseClick;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "icons8-refrigerator-15.png");
            imageList1.Images.SetKeyName(1, "icons8-petri-dish-15.png");
            imageList1.Images.SetKeyName(2, "icons8-bacteria-15.png");
            imageList1.Images.SetKeyName(3, "icons8-worm-16.png");
            imageList1.Images.SetKeyName(4, "icons8-nose-15.png");
            imageList1.Images.SetKeyName(5, "icons8-10%-15.png");
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbReload });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbReload
            // 
            tsbReload.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbReload.Image = (Image)resources.GetObject("tsbReload.Image");
            tsbReload.ImageTransparentColor = Color.Magenta;
            tsbReload.Name = "tsbReload";
            tsbReload.Size = new Size(23, 22);
            tsbReload.ToolTipText = "Reload";
            tsbReload.Click += tsbReload_Click;
            // 
            // frmProjectExplorer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(treeView);
            Controls.Add(toolStrip1);
            Name = "frmProjectExplorer";
            Text = "Project Explorer";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeView;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbReload;
        private ImageList imageList1;
    }
}