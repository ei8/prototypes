using Reactor.Neurons;
using Reactor.SpikeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class DendritesForm : DockContent
    {
        private NeuronCollection neurons;
        private ISelectionService selectionService;

        public DendritesForm(ISelectionService selectionService, NeuronCollection neurons)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            this.neurons = neurons;
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (Helper.IsSelectionNeuron(this.selectionService))
            {
                this.listView1.Items.Clear();
                this.selectionService.SelectedObjects.ToList()
                    .SelectMany(n =>
                    {
                        var ne = (Neuron)n;
                        var ds = DendritesForm.GetDendrites(ne.Id, neurons);
                        return ds.Select(d => new string[] {
                        d.Data,
                        d.Terminals.First(t => t.TargetId == ne.Id).Strength.ToString(),
                        d.Threshold.ToString(),
                        d.Id
                        }
                        );
                    })
                    .ToList().ForEach(ss => this.listView1.Items.Add(new ListViewItem(ss)));

                this.countToolStripStatusLabel.Text = $"{this.listView1.Items.Count} dendrite(s)";
            }
        }

        private static IEnumerable<Neuron> GetDendrites(string id, NeuronCollection neurons)
        {
            return neurons.ToList().Where(n => n.Terminals.Where(t => t.TargetId == id).Count() > 0);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                this.selectionService.SetSelectedObjects(new Neuron[] { (Neuron)this.neurons[this.listView1.SelectedItems[0].SubItems[3].Text] });
            }
        }

        private void showAllToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
            {
                this.selectionService.SetSelectedObjects(this.listView1.Items.Cast<ListViewItem>().Select(lvi => (Neuron)this.neurons[lvi.SubItems[3].Text]));
            }
        }
    }
}
