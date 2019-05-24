namespace Reactor
{
    partial class LogsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogsForm));
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.autoscrollToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.Location = new System.Drawing.Point(0, 25);
            this.logTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(600, 341);
            this.logTextBox.TabIndex = 8;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripButton,
            this.autoscrollToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(600, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // clearToolStripButton
            // 
            this.clearToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.clearToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("clearToolStripButton.Image")));
            this.clearToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearToolStripButton.Name = "clearToolStripButton";
            this.clearToolStripButton.Size = new System.Drawing.Size(38, 22);
            this.clearToolStripButton.Text = "Clear";
            this.clearToolStripButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // autoscrollToolStripButton
            // 
            this.autoscrollToolStripButton.Checked = true;
            this.autoscrollToolStripButton.CheckOnClick = true;
            this.autoscrollToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoscrollToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.autoscrollToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("autoscrollToolStripButton.Image")));
            this.autoscrollToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.autoscrollToolStripButton.Name = "autoscrollToolStripButton";
            this.autoscrollToolStripButton.Size = new System.Drawing.Size(65, 22);
            this.autoscrollToolStripButton.Text = "Autoscroll";
            // 
            // LogsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.toolStrip1);
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "LogsForm";
            this.Text = "Logs";
            this.Load += new System.EventHandler(this.LogsForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearToolStripButton;
        private System.Windows.Forms.ToolStripButton autoscrollToolStripButton;
    }
}