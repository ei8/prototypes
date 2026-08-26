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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTree));
            listView1 = new ListView();
            columnHeader2 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            toolStrip1 = new ToolStrip();
            tsbReload = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbSpike = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            tsbStartProcess = new ToolStripDropDownButton();
            mnuStartProcessDoUntil = new ToolStripMenuItem();
            mnuStartProcessAddition = new ToolStripMenuItem();
            sequentialToolStripMenuItem = new ToolStripMenuItem();
            dynamicToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            setTimerIntervalToolStripMenuItem = new ToolStripMenuItem();
            tsbStopProcess = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            tsbCheckAll = new ToolStripButton();
            tsbCheckSelected = new ToolStripButton();
            tstbFilter = new ToolStripTextBox();
            toolStripSeparator2 = new ToolStripSeparator();
            tsbFocusChecked = new ToolStripButton();
            tsbFocusReflexArc = new ToolStripButton();
            tsbHideSelectedTags = new ToolStripDropDownButton();
            hideSelectedTagsToolStripMenuItem = new ToolStripMenuItem();
            mnuHideLogicGatesInterneuronsTags = new ToolStripMenuItem();
            tsbCopyIds = new ToolStripButton();
            timer1 = new System.Windows.Forms.Timer(components);
            toolStripMenuItem2 = new ToolStripMenuItem();
            dynamicMultiplicationToolStripMenuItem = new ToolStripMenuItem();
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
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
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
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbReload, toolStripSeparator1, tsbSpike, toolStripSeparator4, tsbStartProcess, tsbStopProcess, toolStripSeparator3, tsbCheckAll, tsbCheckSelected, tstbFilter, toolStripSeparator2, tsbFocusChecked, tsbFocusReflexArc, tsbHideSelectedTags, tsbCopyIds });
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
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // tsbStartProcess
            // 
            tsbStartProcess.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbStartProcess.DropDownItems.AddRange(new ToolStripItem[] { mnuStartProcessDoUntil, mnuStartProcessAddition, toolStripMenuItem2, toolStripMenuItem1, setTimerIntervalToolStripMenuItem });
            tsbStartProcess.Image = (Image)resources.GetObject("tsbStartProcess.Image");
            tsbStartProcess.ImageTransparentColor = Color.Magenta;
            tsbStartProcess.Name = "tsbStartProcess";
            tsbStartProcess.Size = new Size(29, 22);
            tsbStartProcess.Text = "Start Process";
            tsbStartProcess.ToolTipText = "Start Process";
            // 
            // mnuStartProcessDoUntil
            // 
            mnuStartProcessDoUntil.Name = "mnuStartProcessDoUntil";
            mnuStartProcessDoUntil.Size = new Size(180, 22);
            mnuStartProcessDoUntil.Text = "Do Until...";
            mnuStartProcessDoUntil.Click += mnuStartProcessDoUntil_Click;
            // 
            // mnuStartProcessAddition
            // 
            mnuStartProcessAddition.DropDownItems.AddRange(new ToolStripItem[] { sequentialToolStripMenuItem, dynamicToolStripMenuItem });
            mnuStartProcessAddition.Name = "mnuStartProcessAddition";
            mnuStartProcessAddition.Size = new Size(180, 22);
            mnuStartProcessAddition.Text = "Addition";
            // 
            // sequentialToolStripMenuItem
            // 
            sequentialToolStripMenuItem.Name = "sequentialToolStripMenuItem";
            sequentialToolStripMenuItem.Size = new Size(129, 22);
            sequentialToolStripMenuItem.Text = "Sequential";
            sequentialToolStripMenuItem.Click += mnuStartProcessAddition_Click;
            // 
            // dynamicToolStripMenuItem
            // 
            dynamicToolStripMenuItem.Name = "dynamicToolStripMenuItem";
            dynamicToolStripMenuItem.Size = new Size(129, 22);
            dynamicToolStripMenuItem.Text = "Dynamic";
            dynamicToolStripMenuItem.Click += dynamicToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(177, 6);
            // 
            // setTimerIntervalToolStripMenuItem
            // 
            setTimerIntervalToolStripMenuItem.Name = "setTimerIntervalToolStripMenuItem";
            setTimerIntervalToolStripMenuItem.Size = new Size(180, 22);
            setTimerIntervalToolStripMenuItem.Text = "Set timer interval...";
            setTimerIntervalToolStripMenuItem.Click += setTimerIntervalToolStripMenuItem_Click;
            // 
            // tsbStopProcess
            // 
            tsbStopProcess.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbStopProcess.Image = (Image)resources.GetObject("tsbStopProcess.Image");
            tsbStopProcess.ImageTransparentColor = Color.Magenta;
            tsbStopProcess.Name = "tsbStopProcess";
            tsbStopProcess.Size = new Size(23, 22);
            tsbStopProcess.Text = "Stop Process";
            tsbStopProcess.ToolTipText = "Stop Process";
            tsbStopProcess.Click += tsbStopProcess_Click;
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
            // tstbFilter
            // 
            tstbFilter.Name = "tstbFilter";
            tstbFilter.Size = new Size(100, 25);
            tstbFilter.TextChanged += tstbFilter_TextChanged;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // tsbFocusChecked
            // 
            tsbFocusChecked.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFocusChecked.Image = (Image)resources.GetObject("tsbFocusChecked.Image");
            tsbFocusChecked.ImageTransparentColor = Color.Magenta;
            tsbFocusChecked.Name = "tsbFocusChecked";
            tsbFocusChecked.Size = new Size(23, 22);
            tsbFocusChecked.Text = "Focus Checked";
            tsbFocusChecked.Click += tsbFocusChecked_Click;
            // 
            // tsbFocusReflexArc
            // 
            tsbFocusReflexArc.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFocusReflexArc.Image = (Image)resources.GetObject("tsbFocusReflexArc.Image");
            tsbFocusReflexArc.ImageTransparentColor = Color.Magenta;
            tsbFocusReflexArc.Name = "tsbFocusReflexArc";
            tsbFocusReflexArc.Size = new Size(23, 22);
            tsbFocusReflexArc.Text = "toolStripButton1";
            tsbFocusReflexArc.ToolTipText = "Focus Reflex Arc in Graph";
            tsbFocusReflexArc.Click += tsbFocusReflexArc_Click;
            // 
            // tsbHideSelectedTags
            // 
            tsbHideSelectedTags.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbHideSelectedTags.DropDownItems.AddRange(new ToolStripItem[] { hideSelectedTagsToolStripMenuItem, mnuHideLogicGatesInterneuronsTags });
            tsbHideSelectedTags.Image = (Image)resources.GetObject("tsbHideSelectedTags.Image");
            tsbHideSelectedTags.ImageTransparentColor = Color.Magenta;
            tsbHideSelectedTags.Name = "tsbHideSelectedTags";
            tsbHideSelectedTags.Size = new Size(29, 22);
            tsbHideSelectedTags.Text = "toolStripButton1";
            tsbHideSelectedTags.ToolTipText = "Hide Selected Tags in Graph";
            // 
            // hideSelectedTagsToolStripMenuItem
            // 
            hideSelectedTagsToolStripMenuItem.Name = "hideSelectedTagsToolStripMenuItem";
            hideSelectedTagsToolStripMenuItem.Size = new Size(259, 22);
            hideSelectedTagsToolStripMenuItem.Text = "Hide Selected Tags";
            hideSelectedTagsToolStripMenuItem.Click += tsbHideSelectedTags_Click;
            // 
            // mnuHideLogicGatesInterneuronsTags
            // 
            mnuHideLogicGatesInterneuronsTags.Name = "mnuHideLogicGatesInterneuronsTags";
            mnuHideLogicGatesInterneuronsTags.Size = new Size(259, 22);
            mnuHideLogicGatesInterneuronsTags.Text = "Hide Logic Gates Interneurons Tags";
            mnuHideLogicGatesInterneuronsTags.Click += mnuHideLogicGatesInterneuronsTags_Click;
            // 
            // tsbCopyIds
            // 
            tsbCopyIds.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbCopyIds.Image = (Image)resources.GetObject("tsbCopyIds.Image");
            tsbCopyIds.ImageTransparentColor = Color.Magenta;
            tsbCopyIds.Name = "tsbCopyIds";
            tsbCopyIds.Size = new Size(58, 22);
            tsbCopyIds.Text = "Copy IDs";
            tsbCopyIds.Click += tsbCopyIds_Click;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { dynamicMultiplicationToolStripMenuItem });
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(180, 22);
            toolStripMenuItem2.Text = "Multiplication";
            // 
            // dynamicMultiplicationToolStripMenuItem
            // 
            dynamicMultiplicationToolStripMenuItem.Name = "dynamicMultiplicationToolStripMenuItem";
            dynamicMultiplicationToolStripMenuItem.Size = new Size(180, 22);
            dynamicMultiplicationToolStripMenuItem.Text = "Dynamic";
            dynamicMultiplicationToolStripMenuItem.Click += dynamicMultiplicationToolStripMenuItem_Click;
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
        private ToolStripButton tsbFocusChecked;
        private ToolStripButton tsbCopyIds;
        private ToolStripDropDownButton tsbStartProcess;
        private ToolStripMenuItem mnuStartProcessDoUntil;
        private ToolStripButton tsbStopProcess;
        private ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Timer timer1;
        private ToolStripDropDownButton tsbHideSelectedTags;
        private ToolStripMenuItem hideSelectedTagsToolStripMenuItem;
        private ToolStripMenuItem mnuHideLogicGatesInterneuronsTags;
        private ToolStripMenuItem mnuStartProcessAddition;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem setTimerIntervalToolStripMenuItem;
        private ToolStripMenuItem sequentialToolStripMenuItem;
        private ToolStripMenuItem dynamicToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem dynamicMultiplicationToolStripMenuItem;
    }
}