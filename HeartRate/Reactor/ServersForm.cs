using Reactor.Neurons;
using Reactor.SpikeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class ServersForm : DockContent
    {
        private NeuronCollection neurons;
        private ISelectionService selectionService;

        public ServersForm(ISelectionService selectionService, NeuronCollection neurons)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            this.neurons = neurons;
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                this.selectionService.SetSelectedObjects(new Neuron[] { (Neuron)this.listView1.SelectedItems[0].Tag });
            }
        }

        private void showAllToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
            {
                this.selectionService.SetSelectedObjects(this.listView1.Items.Cast<ListViewItem>().Select(lvi => (Neuron) lvi.Tag));
            }
        }
    }
}
