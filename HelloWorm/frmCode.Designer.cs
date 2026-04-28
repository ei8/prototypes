namespace ei8.Prototypes.HelloWorm
{
    partial class frmCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCode));
            gViewer1 = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            splitContainer1 = new SplitContainer();
            listView1 = new ListView();
            columnHeader2 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            toolStrip1 = new ToolStrip();
            tsbReload = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbCheckAll = new ToolStripButton();
            tsbCheckSelected = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            tsbFocusReflexArc = new ToolStripButton();
            tsbChangeOrientation = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // gViewer1
            // 
            gViewer1.ArrowheadLength = 10D;
            gViewer1.AsyncLayout = false;
            gViewer1.AutoScroll = true;
            gViewer1.BackwardEnabled = false;
            gViewer1.BuildHitTree = true;
            gViewer1.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            gViewer1.Dock = DockStyle.Fill;
            gViewer1.EdgeInsertButtonVisible = true;
            gViewer1.FileName = "";
            gViewer1.ForwardEnabled = false;
            gViewer1.Graph = null;
            gViewer1.IncrementalDraggingModeAlways = false;
            gViewer1.InsertingEdge = false;
            gViewer1.LayoutAlgorithmSettingsButtonVisible = true;
            gViewer1.LayoutEditingEnabled = true;
            gViewer1.Location = new Point(0, 0);
            gViewer1.LooseOffsetForRouting = 0.25D;
            gViewer1.MouseHitDistance = 0.05D;
            gViewer1.Name = "gViewer1";
            gViewer1.NavigationVisible = true;
            gViewer1.NeedToCalculateLayout = true;
            gViewer1.OffsetForRelaxingInRouting = 0.6D;
            gViewer1.PaddingForEdgeRouting = 8D;
            gViewer1.PanButtonPressed = false;
            gViewer1.SaveAsImageEnabled = true;
            gViewer1.SaveAsMsaglEnabled = true;
            gViewer1.SaveButtonVisible = true;
            gViewer1.SaveGraphButtonVisible = true;
            gViewer1.SaveInVectorFormatEnabled = true;
            gViewer1.Size = new Size(800, 225);
            gViewer1.TabIndex = 0;
            gViewer1.TightOffsetForRouting = 0.125D;
            gViewer1.ToolBarIsVisible = true;
            gViewer1.Transform = (Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)resources.GetObject("gViewer1.Transform");
            gViewer1.UndoRedoButtonsVisible = true;
            gViewer1.WindowZoomButtonPressed = false;
            gViewer1.ZoomF = 1D;
            gViewer1.ZoomWindowThreshold = 0.05D;
            gViewer1.MouseClick += gViewer1_MouseClick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(gViewer1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listView1);
            splitContainer1.Panel2.Controls.Add(toolStrip1);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 225;
            splitContainer1.TabIndex = 1;
            // 
            // listView1
            // 
            listView1.CheckBoxes = true;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader2, columnHeader1 });
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, 25);
            listView1.Name = "listView1";
            listView1.Size = new Size(800, 196);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Tag";
            columnHeader2.Width = 600;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "ID";
            columnHeader1.Width = 300;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbReload, toolStripSeparator1, tsbCheckAll, tsbCheckSelected, toolStripSeparator2, tsbFocusReflexArc, tsbChangeOrientation });
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
            tsbReload.Text = "toolStripButton1";
            tsbReload.ToolTipText = "Reload";
            tsbReload.Click += tsbReload_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // tsbCheckAll
            // 
            tsbCheckAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCheckAll.Image = (Image)resources.GetObject("tsbCheckAll.Image");
            tsbCheckAll.ImageTransparentColor = Color.Magenta;
            tsbCheckAll.Name = "tsbCheckAll";
            tsbCheckAll.Size = new Size(23, 22);
            tsbCheckAll.Text = "toolStripButton1";
            tsbCheckAll.ToolTipText = "Check/Uncheck All";
            tsbCheckAll.Click += tsbCheckAll_Click;
            // 
            // tsbCheckSelected
            // 
            tsbCheckSelected.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCheckSelected.Image = (Image)resources.GetObject("tsbCheckSelected.Image");
            tsbCheckSelected.ImageTransparentColor = Color.Magenta;
            tsbCheckSelected.Name = "tsbCheckSelected";
            tsbCheckSelected.Size = new Size(23, 22);
            tsbCheckSelected.Text = "toolStripButton1";
            tsbCheckSelected.ToolTipText = "Check/Uncheck Selected";
            tsbCheckSelected.Click += tsbCheckSelected_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // tsbFocusReflexArc
            // 
            tsbFocusReflexArc.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFocusReflexArc.Image = (Image)resources.GetObject("tsbFocusReflexArc.Image");
            tsbFocusReflexArc.ImageTransparentColor = Color.Magenta;
            tsbFocusReflexArc.Name = "tsbFocusReflexArc";
            tsbFocusReflexArc.Size = new Size(23, 22);
            tsbFocusReflexArc.Text = "toolStripButton1";
            tsbFocusReflexArc.ToolTipText = "Focus Reflex Arc";
            tsbFocusReflexArc.Click += tsbFocusReflexArc_Click;
            // 
            // tsbChangeOrientation
            // 
            tsbChangeOrientation.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbChangeOrientation.Image = (Image)resources.GetObject("tsbChangeOrientation.Image");
            tsbChangeOrientation.ImageTransparentColor = Color.Magenta;
            tsbChangeOrientation.Name = "tsbChangeOrientation";
            tsbChangeOrientation.Size = new Size(23, 22);
            tsbChangeOrientation.Text = "Change orientation";
            tsbChangeOrientation.Click += tsbChangeOrientation_Click;
            // 
            // frmCode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "frmCode";
            Text = "Code";
            Load += frmCode_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Msagl.GraphViewerGdi.GViewer gViewer1;
        private SplitContainer splitContainer1;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbReload;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsbCheckAll;
        private ToolStripButton tsbCheckSelected;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tsbFocusReflexArc;
        private ToolStripButton tsbChangeOrientation;
    }
}