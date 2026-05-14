using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;
using Color = Microsoft.Msagl.Drawing.Color;
using Rectangle = Microsoft.Msagl.Core.Geometry.Rectangle;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmGraph : DockContent
    {
        private enum Status
        {
            NotSet,
            Triggered,
            Fired,
            FiredPreviously
        }

        private class SpikeUIInfo(Guid id, Status status, DateTime timestamp)
        {
            public Guid Id { get; } = id;
            public Status Status { get; set; } = status;
            public DateTime Timestamp { get; set; } = timestamp;
        }

        private const int TriggeredWidth = 2;
        private const int FiredPreviouslyWidth = 3;
        private const int FiredWidth = 3;
        private const int InitialWidth = 1;
        private readonly ISpikableReporting2 spikable;
        private readonly ISelectionService selectionService;
        private readonly ConcurrentDictionary<Guid, SpikeUIInfo> spikes;

        public frmGraph(ISelectionService selectionService)
        {
            InitializeComponent();

            this.selectionService = selectionService;

            if (this.selectionService.PrimarySelection is ISpikableReporting2 s)
            {
                this.spikable = s;
                this.spikable.Triggered += this.Spikable_Triggered;
                this.spikable.Fired += this.Spikable_Fired;
            }
            else
                throw new ArgumentException("Cannot construct Graph without a selected ISpikable.");

            this.spikes = new();
        }

        private void Spikable_Fired(object? sender, FiredEventArgs e)
        {
            frmGraph.UpdateNode(
                this.gViewer1, 
                spikes, 
                e.FireInfo.Triggers.Select(t => t.PresynapticId), 
                e.FireInfo.Target.Id, 
                Status.Fired
            );
        }

        private void Spikable_Triggered(object? sender, TriggeredEventArgs e)
        {
            frmGraph.UpdateNode(
                this.gViewer1, 
                spikes, 
                e.ReflexArc.Any() ? [e.ReflexArc.Last().Target.Id] : Enumerable.Empty<Guid>(), 
                e.Target.Id, 
                Status.Triggered
            );
        }

        private static void UpdateNode(GViewer gViewer, ConcurrentDictionary<Guid, SpikeUIInfo> spikes, IEnumerable<Guid> sourceIds, Guid targetId, Status newStatus)
        {
            var targetNode = gViewer.Graph.FindNode(targetId.ToString());

            if (targetNode != null && spikes.TryGetAdd(targetId, id => new SpikeUIInfo(id, Status.NotSet, DateTime.Now), out SpikeUIInfo? sui))
            {
                var invalidateRectangles = Enumerable.Empty<Rectangle>();
                switch (newStatus)
                {
                    case Status.Triggered:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())), 
                            targetNode, 
                            spikes, 
                            Color.Red, 
                            Color.Red, 
                            frmGraph.TriggeredWidth
                        );
                        break;
                    case Status.Fired:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode, 
                            spikes,
                            !targetNode.OutEdges.Any() ?
                                Color.Lime :
                                Color.DodgerBlue, 
                            Color.DodgerBlue,
                            frmGraph.FiredWidth
                        );
                        break;
                    case Status.FiredPreviously:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            spikes,
                            !targetNode.OutEdges.Any() ?
                                Color.ForestGreen :
                                Color.DarkSlateBlue,
                            Color.DarkSlateBlue,
                            frmGraph.FiredPreviouslyWidth,
                            true
                        );
                        break;
                    case Status.NotSet:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode, 
                            spikes, 
                            Color.Black, 
                            Color.Black,
                            InitialWidth
                        );
                        break;
                }

                foreach(var rectangle in invalidateRectangles)
                    gViewer.Invalidate(
                        new System.Drawing.Rectangle(
                            (int)rectangle.Left,
                            (int)rectangle.Top,
                            (int)rectangle.Width,
                            (int)rectangle.Height
                        ),
                        false
                    );
                sui.Status = newStatus;
                sui.Timestamp = DateTime.Now;
            }
        }

        private static IEnumerable<Rectangle> UpdateNodeCore(
            IEnumerable<Node> sourceNodes, 
            Node targetNode, 
            ConcurrentDictionary<Guid, SpikeUIInfo> spikes, 
            Color nodeColor, 
            Color edgeColor, 
            double width,
            bool forceUpdateInEdges = false
        )
        {
            var result = new List<Rectangle>();

            targetNode.Attr.Color = nodeColor;
            targetNode.Attr.LineWidth = width;

            result.Add(targetNode.BoundingBox);

            foreach (var ie in targetNode.Edges)
            {
                // if "in" edge
                if (ie.TargetNode == targetNode)
                {
                    // if resetting or presynaptic is in triggers list
                    if (
                        width == frmGraph.InitialWidth || 
                        sourceNodes.Contains(ie.SourceNode) || 
                        (
                            forceUpdateInEdges && 
                            ie.Attr.LineWidth != frmGraph.InitialWidth
                        )
                    )
                    {
                        // apply style to edge
                        ie.Attr.Color = edgeColor;
                        ie.Attr.LineWidth = width;
                        result.Add(ie.BoundingBox);
                    }
                }
                // else if "out" edge
                else if (ie.SourceNode == targetNode)
                {
                    // if resetting
                    if (width == frmGraph.InitialWidth)
                    {
                        // apply style to edge
                        ie.Attr.Color = edgeColor;
                        ie.Attr.LineWidth = width;
                        result.Add(ie.BoundingBox);
                    }
                }
            }

            return result;
        }

        private void frmGraph_Load(object sender, EventArgs e)
        {
            if (this.spikable.Network != null)
                this.Reload(this.spikable.Network.GetItems<Neuron>());
        }

        public void Reload(IEnumerable<Neuron> cns)
        {
            if (this.spikable.Network != null)
            {
                var graph = new Graph("graph");

                foreach (var t in this.spikable.Network.GetItems<Terminal>())
                    if (cns.Any(cn => cn.Id == t.PresynapticNeuronId) && cns.Any(cn => cn.Id == t.PostsynapticNeuronId))
                    {
                        var edge = graph.AddEdge(
                            t.PresynapticNeuronId.ToString(),
                            (t.Effect == NeurotransmitterEffect.Excite ? "+" : "-") + t.Strength.ToString(),
                            t.PostsynapticNeuronId.ToString()
                        );
                    }

                foreach (var n in this.spikable.Network.GetItems<Neuron>().Where(n => cns.Any(cn => cn.Id == n.Id)))
                {
                    var node = graph.FindNode(n.Id.ToString());
                    if (node != null)
                    {
                        var tag = n.Tag;
                        if (!string.IsNullOrWhiteSpace(tag))
                        {
                            var lastIndex = tag.LastIndexOfAny(['.', '+']);
                            if (lastIndex > -1)
                                tag = tag.Substring(lastIndex + 1);
                        }
                        node.LabelText = tag;
                        node.UserData = n;

                        if (string.IsNullOrEmpty(tag))
                            node.Attr.Shape = Shape.Circle;
                    }
                }
                this.gViewer1.CurrentLayoutMethod = LayoutMethod.Ranking;
                this.gViewer1.Graph = graph;
            }
        }

        private void gViewer1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.gViewer1.SelectedObject is Node n)
                this.selectionService.SetSelectedComponents(new[] { n.UserData });
        }

        private void selectGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedComponents(new[] { this });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var activeSpikes = this.spikes.Where(s => s.Value.Status != Status.NotSet);

            foreach (var spike in activeSpikes)
            {
                if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > 4 && spike.Value.Status == Status.FiredPreviously)
                {
                    frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.NotSet);
                }
                else if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > 2)
                {
                    if (spike.Value.Status == Status.Triggered)
                        frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.NotSet);
                    else if (spike.Value.Status == Status.Fired)
                        frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.FiredPreviously);
                }
            }
        }
    }
}
