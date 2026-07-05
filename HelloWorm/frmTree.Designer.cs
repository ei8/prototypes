namespace ei8.Prototypes.HelloWorm
{
    partial class frmTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTree));
            listView1 = new ListView();
            columnHeader2 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            toolStrip1 = new ToolStrip();
            tsbReload = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbSpike = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            tsbCheckAll = new ToolStripButton();
            tsbCheckSelected = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            tsbFocusReflexArc = new ToolStripButton();
            tstbFilter = new ToolStripTextBox();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.CheckBoxes = true;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader2, columnHeader1 });
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, 25);
            listView1.Name = "listView1";
            listView1.Size = new Size(800, 425);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.KeyDown += listView1_KeyDown;
            listView1.KeyPress += listView1_KeyPress;
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
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbReload, toolStripSeparator1, tsbSpike, toolStripSeparator3, tsbCheckAll, tsbCheckSelected, tstbFilter, toolStripSeparator2, tsbFocusReflexArc });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 3;
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
            // tsbSpike
            // 
            tsbSpike.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbSpike.Image = (Image)resources.GetObject("tsbSpike.Image");
            tsbSpike.ImageTransparentColor = Color.Magenta;
            tsbSpike.Name = "tsbSpike";
            tsbSpike.Size = new Size(23, 22);
            tsbSpike.Text = "Spike";
            tsbSpike.Click += tsbSpike_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
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
            // tstbFilter
            // 
            tstbFilter.Name = "tstbFilter";
            tstbFilter.Size = new Size(100, 25);
            tstbFilter.TextChanged += tstbFilter_TextChanged;
            // 
            // frmTree
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listView1);
            Controls.Add(toolStrip1);
            Name = "frmTree";
            Text = "Tree";
            Load += frmTree_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader1;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbReload;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsbCheckAll;
        private ToolStripButton tsbCheckSelected;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tsbFocusReflexArc;
        private ToolStripButton tsbSpike;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripTextBox tstbFilter;
    }
}