using Aga.Controls.Tree;
using System.Windows.Forms;

namespace Reactor.Views
{
    partial class CortexGraphView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CortexGraphView));
            this.treeView1 = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn4 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn3 = new Aga.Controls.Tree.TreeColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.jumpToNeuronToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox4 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox3 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.loadAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.renderToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.avatarUriToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.loadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rootToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.Columns.Add(this.treeColumn1);
            this.treeView1.Columns.Add(this.treeColumn4);
            this.treeView1.Columns.Add(this.treeColumn2);
            this.treeView1.Columns.Add(this.treeColumn3);
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.FullRowSelect = true;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.LoadOnDemand = true;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this.nodeIcon1);
            this.treeView1.NodeControls.Add(this.nodeTextBox1);
            this.treeView1.NodeControls.Add(this.nodeTextBox4);
            this.treeView1.NodeControls.Add(this.nodeTextBox2);
            this.treeView1.NodeControls.Add(this.nodeTextBox3);
            this.treeView1.SelectedNode = null;
            this.treeView1.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(930, 341);
            this.treeView1.TabIndex = 0;
            this.treeView1.UseColumns = true;
            this.treeView1.SelectionChanged += new System.EventHandler(this.treeView1_SelectionChanged);
            // 
            // treeColumn1
            // 
            this.treeColumn1.Header = "Data";
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = null;
            this.treeColumn1.Width = 400;
            // 
            // treeColumn4
            // 
            this.treeColumn4.Header = "Strength";
            this.treeColumn4.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn4.TooltipText = null;
            this.treeColumn4.Width = 120;
            // 
            // treeColumn2
            // 
            this.treeColumn2.Header = "Threshold (mV)";
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TooltipText = null;
            this.treeColumn2.Width = 125;
            // 
            // treeColumn3
            // 
            this.treeColumn3.Header = "Id";
            this.treeColumn3.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn3.TooltipText = null;
            this.treeColumn3.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jumpToNeuronToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 26);
            // 
            // jumpToNeuronToolStripMenuItem
            // 
            this.jumpToNeuronToolStripMenuItem.Name = "jumpToNeuronToolStripMenuItem";
            this.jumpToNeuronToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.jumpToNeuronToolStripMenuItem.Text = "Jump to Neuron";
            this.jumpToNeuronToolStripMenuItem.Click += new System.EventHandler(this.jumpToNeuronToolStripMenuItem_Click);
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.treeColumn1;
            this.nodeIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            this.nodeIcon1.VirtualMode = true;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Data";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBox4
            // 
            this.nodeTextBox4.IncrementalSearchEnabled = true;
            this.nodeTextBox4.LeftMargin = 3;
            this.nodeTextBox4.ParentColumn = this.treeColumn4;
            this.nodeTextBox4.VirtualMode = true;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Threshold";
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.treeColumn2;
            // 
            // nodeTextBox3
            // 
            this.nodeTextBox3.DataPropertyName = "Id";
            this.nodeTextBox3.IncrementalSearchEnabled = true;
            this.nodeTextBox3.LeftMargin = 3;
            this.nodeTextBox3.ParentColumn = this.treeColumn3;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAllToolStripButton,
            this.renderToolStripButton,
            this.avatarUriToolStripTextBox,
            this.loadToolStripButton,
            this.rootToolStripTextBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(930, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // loadAllToolStripButton
            // 
            this.loadAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loadAllToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("loadAllToolStripButton.Image")));
            this.loadAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadAllToolStripButton.Name = "loadAllToolStripButton";
            this.loadAllToolStripButton.Size = new System.Drawing.Size(65, 22);
            this.loadAllToolStripButton.Text = "Download";
            this.loadAllToolStripButton.Click += new System.EventHandler(this.loadAllToolStripButton_Click);
            // 
            // renderToolStripButton
            // 
            this.renderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renderToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("renderToolStripButton.Image")));
            this.renderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderToolStripButton.Name = "renderToolStripButton";
            this.renderToolStripButton.Size = new System.Drawing.Size(50, 22);
            this.renderToolStripButton.Text = "Refresh";
            // 
            // avatarUriToolStripTextBox
            // 
            this.avatarUriToolStripTextBox.Name = "avatarUriToolStripTextBox";
            this.avatarUriToolStripTextBox.Size = new System.Drawing.Size(226, 25);
            // 
            // loadToolStripButton
            // 
            this.loadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loadToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("loadToolStripButton.Image")));
            this.loadToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadToolStripButton.Name = "loadToolStripButton";
            this.loadToolStripButton.Size = new System.Drawing.Size(34, 22);
            this.loadToolStripButton.Text = "Find";
            this.loadToolStripButton.Click += new System.EventHandler(this.loadToolStripButton_Click);
            // 
            // rootToolStripTextBox
            // 
            this.rootToolStripTextBox.Name = "rootToolStripTextBox";
            this.rootToolStripTextBox.Size = new System.Drawing.Size(114, 25);
            this.rootToolStripTextBox.Click += new System.EventHandler(this.rootToolStripTextBox_Click);
            // 
            // CortexGraphView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 366);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CortexGraphView";
            this.Text = "Graph";
            this.Load += new System.EventHandler(this.GraphForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeViewAdv treeView1;
        private ToolStrip toolStrip1;
        private ToolStripTextBox rootToolStripTextBox;
        private ToolStripButton loadToolStripButton;
        private TreeColumn treeColumn1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private ToolStripButton loadAllToolStripButton;
        private TreeColumn treeColumn2;
        private TreeColumn treeColumn3;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox3;
        private TreeColumn treeColumn4;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox4;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem jumpToNeuronToolStripMenuItem;
        private ToolStripTextBox avatarUriToolStripTextBox;
        private ToolStripButton renderToolStripButton;
    }
}