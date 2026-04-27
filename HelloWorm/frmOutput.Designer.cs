namespace ei8.Prototypes.HelloWorm
{
    partial class frmOutput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOutput));
            richTextBox = new RichTextBox();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            tscbMin = new ToolStripComboBox();
            toolStripLabel2 = new ToolStripLabel();
            tscbMax = new ToolStripComboBox();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbClear = new ToolStripButton();
            tsbScrollToEnd = new ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBox
            // 
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox.HideSelection = false;
            richTextBox.Location = new Point(0, 25);
            richTextBox.Name = "richTextBox";
            richTextBox.Size = new Size(800, 425);
            richTextBox.TabIndex = 0;
            richTextBox.Text = "";
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, tscbMin, toolStripLabel2, tscbMax, toolStripSeparator1, tsbClear, tsbScrollToEnd });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(31, 22);
            toolStripLabel1.Text = "Min:";
            // 
            // tscbMin
            // 
            tscbMin.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbMin.Name = "tscbMin";
            tscbMin.Size = new Size(121, 25);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(33, 22);
            toolStripLabel2.Text = "Max:";
            // 
            // tscbMax
            // 
            tscbMax.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbMax.Name = "tscbMax";
            tscbMax.Size = new Size(121, 25);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // tsbClear
            // 
            tsbClear.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbClear.Image = (Image)resources.GetObject("tsbClear.Image");
            tsbClear.ImageTransparentColor = Color.Magenta;
            tsbClear.Name = "tsbClear";
            tsbClear.Size = new Size(23, 22);
            tsbClear.Text = "toolStripButton1";
            tsbClear.ToolTipText = "Clear";
            tsbClear.Click += tsbClear_Click;
            // 
            // tsbScrollToEnd
            // 
            tsbScrollToEnd.CheckOnClick = true;
            tsbScrollToEnd.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbScrollToEnd.Image = (Image)resources.GetObject("tsbScrollToEnd.Image");
            tsbScrollToEnd.ImageTransparentColor = Color.Magenta;
            tsbScrollToEnd.Name = "tsbScrollToEnd";
            tsbScrollToEnd.Size = new Size(23, 22);
            tsbScrollToEnd.ToolTipText = "Scroll to End";
            tsbScrollToEnd.Click += tsbScrollToEnd_Click;
            // 
            // frmOutput
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(richTextBox);
            Controls.Add(toolStrip1);
            Name = "frmOutput";
            Text = "Output";
            Load += frmOutput_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbScrollToEnd;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox tscbMin;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox tscbMax;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsbClear;
    }
}