using Aga.Controls.Tree.NodeControls;
using ReactiveUI;
using Reactor.Neurons;
using Reactor.ResultMarkers;
using Reactor.SpikeResults;
using Reactor.Spikes;
using Reactor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using works.ei8.Cortex.Graph.Domain.Model;

using SpikerNeuron = Reactor.Neurons.Neuron;
using SpikerTerminal = Reactor.Neurons.Terminal;

namespace Reactor.Views
{
    public partial class CortexGraphView : DockContent, IViewFor<CortexGraphViewModel>
    {
        private NeuronCollection neurons;
        private ISelectionService selectionService;
        private INotificationLogClient notificationLogClient;
        private bool updatingTreeviewSelection = false;
        private bool userSelectingTreeview = false;
        private bool showingTargets = false;

        public CortexGraphViewModel ViewModel { get; set; }
        object IViewFor.ViewModel { get => this.ViewModel; set => ViewModel = (CortexGraphViewModel) value; }

        public CortexGraphView(NeuronCollection neurons, INotificationLogClient notificationLogClient, IRepository<works.ei8.Cortex.Graph.Domain.Model.Neuron> neuronRepository, ISelectionService selectionService,
            ISpikeTargetListService spikeTargetListService, IResultMarkerService resultMarkerService)
        {
            InitializeComponent();
            this.neurons = neurons;
            this.notificationLogClient = notificationLogClient;
            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            this.nodeIcon1.ValueNeeded += this.NodeIcon1_ValueNeeded;
            this.nodeTextBox4.ValueNeeded += this.NodeTextBox4_ValueNeeded;

            this.WhenActivated(d =>
            {
                d(this.Bind(this.ViewModel, vm => vm.AvatarUri, v => v.avatarUriToolStripTextBox.Text));
                d(this.OneWayBind(this.ViewModel, vm => vm.TreeModel, v => v.treeView1.Model));
                // d(this.b)

                d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.loadAllToolStripButton));
                d(this.BindCommand(this.ViewModel, vm => vm.RenderCommand, v => v.renderToolStripButton));
            });

            this.ViewModel = new CortexGraphViewModel(this.neurons, this.notificationLogClient, neuronRepository, spikeTargetListService, resultMarkerService);
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (!this.userSelectingTreeview && Helper.IsSelectionNeuron(this.selectionService))
            {
                this.updatingTreeviewSelection = true;
                this.Select();
                this.treeView1.Select();
                this.treeView1.ClearSelection();
                this.treeView1.AllNodes.ToList().Where(n => n.Level == 1 && this.selectionService.SelectedObjects.Contains((SpikerNeuron)n.Tag)).ToList().ForEach(n => n.IsSelected = true);
                if (this.treeView1.SelectedNode != null)
                    this.treeView1.ScrollTo(this.treeView1.SelectedNode);
                this.updatingTreeviewSelection = false;
            }
        }

        private void NodeTextBox4_ValueNeeded(object sender, NodeControlValueEventArgs e)
        {
            if (e.Node.Parent != null && e.Node.Parent.Tag != null)
            {
                e.Value = CortexGraphView.GetNodeTerminal(e).Strength;
            }
        }

        private static SpikerTerminal GetNodeTerminal(NodeControlValueEventArgs e)
        {
            SpikerTerminal result = null;
            var nid = ((SpikerNeuron)e.Node.Tag).Id;
            if (((SpikerNeuron)e.Node.Parent.Tag).Terminals.Any(t => t.TargetId == nid))
                result = ((SpikerNeuron)e.Node.Parent.Tag).Terminals.First(t => t.TargetId == nid);
            return result;
        }

        private void NodeIcon1_ValueNeeded(object sender, NodeControlValueEventArgs e)
        {
            if (e.Node.Parent != null && e.Node.Parent.Tag != null)
            {
                e.Value = CortexGraphView.GetNodeTerminal(e).Effect == Neurons.NeurotransmitterEffect.Excite ?
                        Properties.Resources.Positive.ToBitmap() :
                        Properties.Resources.Negative.ToBitmap();
            }
        }

        private void loadToolStripButton_Click(object sender, EventArgs e)
        {
            // DEL:
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            this.loadAllToolStripButton_Click(this, EventArgs.Empty);
        }

        private void loadAllToolStripButton_Click(object sender, EventArgs e)
        {
            this.rootToolStripTextBox.Text = string.Empty;
            this.loadToolStripButton_Click(this, EventArgs.Empty);
            this.rootToolStripTextBox.Text = "[All Neurons]";
        }

        private void rootToolStripTextBox_Click(object sender, EventArgs e)
        {
            var textLength = this.rootToolStripTextBox.Text.Length;
            if (textLength > 0)
            {
                this.rootToolStripTextBox.SelectionStart = 0;
                this.rootToolStripTextBox.SelectionLength = textLength;
            }
        }

        private void treeView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!this.showingTargets && !this.updatingTreeviewSelection)
            {
                this.userSelectingTreeview = true;
                this.selectionService.SetSelectedObjects(this.treeView1.SelectedNodes.Select(tn => (SpikerNeuron)tn.Tag));
                this.userSelectingTreeview = false;
            }
        }

        private void jumpToNeuronToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var o = this.treeView1.SelectedNode.Tag;
            this.treeView1.SelectedNode = this.treeView1.AllNodes.First(n => n.Level == 1 && n.Tag == o);
        }
    }
}
