using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using neurUL.Common.Domain.Model;
using static ei8.Prototypes.HelloWorm.Constants;

namespace ei8.Prototypes.HelloWorm
{
    partial class frmDish
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
            dishPanel = new DishPanel();
            timer1 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // dishPanel
            // 
            dishPanel.Dish = null;
            dishPanel.Dock = DockStyle.Fill;
            dishPanel.Location = new Point(0, 0);
            dishPanel.Name = "dishPanel";
            dishPanel.Size = new Size(800, 450);
            dishPanel.TabIndex = 1;
            dishPanel.DoubleClick += DishPanel_DoubleClick;
            // 
            // timer1
            // 
            timer1.Tick += Timer1_Tick;
            // 
            // frmDish
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dishPanel);
            Name = "frmDish";
            Text = "Dish";
            Load += frmDish_Load;
            ResumeLayout(false);
        }

        #endregion
        private DishPanel dishPanel;
        private System.Windows.Forms.Timer timer1;
    }
}
