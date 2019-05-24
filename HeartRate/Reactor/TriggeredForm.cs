using Reactor.SpikeResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class TriggeredForm : DockContent
    {
        public TriggeredForm(ISpikeResultsService resultsService)
        {
            InitializeComponent();

            resultsService.Cleared += this.ResultsService_Cleared;
            resultsService.TriggeredAdded += this.ResultsService_TriggeredAdded;
        }

        private void ResultsService_Cleared(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.listView1.Items.Clear();
                this.toolStripLabel1.Text = string.Empty;
            }
            ));
        }

        private void ResultsService_TriggeredAdded(object sender, SpikeResultEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                var item = this.listView1.Items.Add(new ListViewItem(new string[] {
                    e.Result.Data,
                    e.Result.Effect == Neurons.NeurotransmitterEffect.Excite ? "+" : "-",
                    e.Result.Charge + " mV",
                    e.Result.Id,
                    e.Result.Timestamp.ToString(Properties.Settings.Default.TimestampFormat)
                }));
                if (this.autoscrollToolStripButton.Checked)
                    item.EnsureVisible();

                this.toolStripLabel1.Text = $"{this.listView1.Items.Count} neuron(s)";
            }
            )
            );
        }
    }
}
