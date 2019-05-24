using Reactor.Neurons;
using Reactor.ResultMarkers;
using Reactor.SpikeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class ResultMarkersForm : DockContent
    {
        private NeuronCollection neurons;
        private IResultMarkerService resultMarkersService;
        private ISelectionService selectionService;

        public ResultMarkersForm(ISelectionService selectionService, IResultMarkerService resultMarkersService, NeuronCollection neurons)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.resultMarkersService = resultMarkersService;
            this.resultMarkersService.Initialized += this.ResultMarkersService_Initialized;
            this.resultMarkersService.Updated += this.ResultMarkersService_Updated;
            this.resultMarkersService.Added += this.ResultMarkersService_Added;
            this.resultMarkersService.Removed += this.ResultMarkersService_Removed;
            this.neurons = neurons;
        }

        private void ResultMarkersService_Removed(object sender, ResultMarkerEventArgs e)
        {
            this.listView1.Items.Remove(this.listView1.Items.Cast<ListViewItem>().First(l => l.Tag == e.Marker));
        }

        private void ResultMarkersService_Added(object sender, ResultMarkerEventArgs e)
        {
            this.listView1.Items.Add(new ListViewItem(new string[] { e.Marker.Id, this.neurons[e.Marker.Id].Data, string.Empty, string.Empty }) { Tag = e.Marker });
        }

        private void ResultMarkersService_Updated(object sender, ResultMarkerEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                var item = this.listView1.Items.Cast<ListViewItem>().ToList().FirstOrDefault(li => li.Tag == e.Marker);
                if (item != null)
                {
                    item.SubItems[2].Text = "Yes";
                    item.SubItems[3].Text = e.Marker.ElapsedTime.ToString();
                }
            }));
        }

        private void ResultMarkersService_Initialized(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.listView1.Items.Cast<ListViewItem>().ToList().ForEach(li =>
                    {
                        li.SubItems[2].Text = string.Empty;
                        li.SubItems[3].Text = string.Empty;
                    }
                );
            }
            ));
        }

        private void addSelectedToolStripButton_Click(object sender, EventArgs e)
        {
            if (Helper.IsSelectionNeuron(this.selectionService))
                this.selectionService.SelectedObjects?.ToList().ForEach(n => this.resultMarkersService.Add(new ResultMarker(((Neuron) n).Id)));
        }

        private void showToolStripButton_Click(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedObjects(this.resultMarkersService.Markers.Select(t => this.neurons[t.Id]));
        }

        private void removeToolStripButton_Click(object sender, EventArgs e)
        {
            this.listView1.SelectedItems?.Cast<ListViewItem>().ToList().ForEach(l => this.resultMarkersService.Remove((ResultMarker)l.Tag));
        }
    }
}
