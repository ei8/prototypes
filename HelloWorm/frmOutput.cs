using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmOutput : DockContent
    {
        public frmOutput()
        {
            InitializeComponent();
        }

        private void frmOutput_Load(object sender, EventArgs e)
        {
            this.tscbMin.Items.AddRange(NLog.LogLevel.AllLevels.Select(l => l.Name).ToArray());
            this.tscbMax.Items.AddRange(NLog.LogLevel.AllLevels.Select(l => l.Name).ToArray());

            this.tscbMin.Tag = this.tscbMin.SelectedIndex = this.tscbMin.FindStringExact(NLog.LogLevel.Off.Name);
            this.tscbMax.Tag = this.tscbMax.SelectedIndex = this.tscbMax.FindStringExact(NLog.LogLevel.Off.Name);

            this.UpdateLogRange();

            this.tscbMin.Validating += this.TscbMinMax_Validating;
            this.tscbMax.Validating += this.TscbMinMax_Validating;
            this.tscbMin.Validated += this.tscbMinMax_Validated;
            this.tscbMax.Validated += this.tscbMinMax_Validated;

            this.tsbScrollToEnd_Click(sender, e);
        }

        private void TscbMinMax_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (
                NLog.LogLevel.FromString(this.tscbMin.Text).Ordinal >
                NLog.LogLevel.FromString(this.tscbMax.Text).Ordinal
            )
            {
                MessageBox.Show("Min log level cannot be greater than Max.", "Invalid Log Level", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;

                ToolStripComboBox tscb = (ToolStripComboBox)sender!;
                if (tscb.Tag != null)
                    tscb.SelectedIndex = (int) tscb.Tag;
            }
        }

        private void UpdateLogRange()
        {
            var rtbt = new NLog.Windows.Forms.AsyncRichTextBoxTarget();
            rtbt.TargetForm = this;
            rtbt.TargetRichTextBox = this.richTextBox;

            var config = new NLog.Config.LoggingConfiguration();
            
            config.AddRule(
                NLog.LogLevel.FromString(this.tscbMin.Text),
                NLog.LogLevel.FromString(this.tscbMax.Text), 
                rtbt
            );
            NLog.LogManager.Configuration = config;
        }

        private void tsbScrollToEnd_Click(object sender, EventArgs e)
        {
            this.richTextBox.HideSelection = !this.tsbScrollToEnd.Checked;
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            this.richTextBox.Clear();
        }

        private void tscbMinMax_Validated(object? sender, EventArgs e)
        {
            ToolStripComboBox tscb = (ToolStripComboBox)sender!;

            if (tscb.Tag != null && (int)tscb.Tag != tscb.SelectedIndex)
            {
                tscb.Tag = tscb.SelectedIndex;
                this.UpdateLogRange();
            }
        }
    }
}
