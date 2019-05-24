using Reactor.Neurons;
using Reactor.ResultMarkers;
using Reactor.SpikeResults;
using Reactor.Spikes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class SpikeTargetsForm : DockContent
    {
        private NeuronCollection neurons;
        private ISpikeTargetListService spikeService;
        private ISelectionService selectionService;

        public SpikeTargetsForm(ISelectionService selectionService, ISpikeTargetListService spikeService, NeuronCollection neurons)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.spikeService = spikeService;
            this.spikeService.Added += this.SpikeService_Added;
            this.spikeService.Removed += this.SpikeService_Removed;
            this.neurons = neurons;
        }

        private void SpikeService_Removed(object sender, SpikeTargetEventArgs e)
        {
            this.listView1.Items.Remove(this.listView1.Items.Cast<ListViewItem>().First(li => (SpikeTarget) li.Tag == e.Target));
        }

        private void SpikeService_Added(object sender, SpikeTargetEventArgs e)
        {
            this.listView1.Items.Add(new ListViewItem(new string[] { e.Target.Id, this.neurons[e.Target.Id].Data }) { Tag = e.Target });
        }

        private void addSelectedToolStripButton_Click(object sender, EventArgs e)
        {
            if (Helper.IsSelectionNeuron(this.selectionService))
                this.selectionService.SelectedObjects?.ToList().ForEach(n => this.spikeService.Add(new SpikeTarget(((Neuron) n).Id)));
        }

        private void showToolStripButton_Click(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedObjects(this.spikeService.Targets.Select(t => this.neurons[t.Id]));
        }

        private void removeToolStripButton_Click(object sender, EventArgs e)
        {
            this.listView1.SelectedItems?.Cast<ListViewItem>().ToList().ForEach(l => this.spikeService.Remove((SpikeTarget)l.Tag));
        }
    }
}
