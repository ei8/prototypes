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
            TriggeredPreviously,
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
        private static readonly Color InactiveColor = Color.Silver;
        private readonly ISpikableReporting2 spikable;
        private readonly ISelectionService selectionService;
        private readonly ConcurrentDictionary<Guid, SpikeUIInfo> spikes;
        private readonly GraphSettings settings;

        public frmGraph(ISelectionService selectionService)
        {
            InitializeComponent();

            this.selectionService = selectionService;

            if (this.selectionService.PrimarySelection is ISpikableReporting2 s)
            {
                this.spikable = s;
                this.spikable.Triggered += this.Spikable_Triggered;
                this.spikable.Fired += this.Spikable_Fired;

                if (this.spikable is INamed n)
                    n.PropertyChanged += this.N_PropertyChanged;
                
            }
            else
                throw new ArgumentException("Cannot construct Graph without a selected ISpikable.");

            this.spikes = new();
            this.settings = new GraphSettings();
            this.settings.PropertyChanged += this.Settings_PropertyChanged;
            this.settings.SpikeVisualization.PropertyChanged += this.SpikeVisualization_PropertyChanged;

            this.settings.ShortenMirrorTags = true;
            this.settings.SpikeVisualization.Enabled = true;
            this.settings.SpikeVisualization.SustainTriggered = false;
            this.settings.SpikeVisualization.SustainFired = true;
        }

        private void N_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(INamed.Name):
                case nameof(IPerishable.Life):
                    bool process = true;
                    if (
                        e.PropertyName == nameof(IPerishable.Life) && 
                        sender is IPerishable perishable &&
                        perishable.Life > 0
                    )
                        process = false;

                    if (process)
                        frmGraph.UpdateName(this.spikable, this);

                    break;
            }
        }

        private void SpikeVisualization_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SpikeVisualizationSettings.Enabled):
                    this.timer1.Enabled = this.settings.SpikeVisualization.Enabled;
                    this.spikes.Clear();
                    this.frmGraph_Load(this, EventArgs.Empty);
                    break;
            }
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GraphSettings.ShortenMirrorTags):
                    this.frmGraph_Load(this, EventArgs.Empty);
                    break;
            }
        }

        private void Spikable_Fired(object? sender, FiredEventArgs e)
        {
            if (this.settings.SpikeVisualization.Enabled)
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
            if (this.settings.SpikeVisualization.Enabled)
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
                            Color.Black,
                            Color.Red,
                            Color.Black,
                            frmGraph.TriggeredWidth
                        );
                        break;
                    case Status.TriggeredPreviously:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            spikes,
                            Color.DarkRed,
                            Color.Black,
                            Color.DarkRed,
                            Color.Black,
                            frmGraph.TriggeredWidth,
                            true
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
                            Color.Black,
                            Color.DodgerBlue,
                            Color.Black,
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
                            Color.Black,
                            Color.DarkSlateBlue,
                            Color.Black,
                            frmGraph.FiredPreviouslyWidth,
                            true
                        );
                        break;
                    case Status.NotSet:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            sourceIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            spikes,
                            frmGraph.InactiveColor,
                            frmGraph.InactiveColor,
                            frmGraph.InactiveColor,
                            frmGraph.InactiveColor,
                            frmGraph.InitialWidth
                        );
                        break;
                }

                foreach (var rectangle in invalidateRectangles)
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
            Color nodeLabelFontColor,
            Color edgeColor,
            Color edgeLabelFontColor,
            double width,
            bool forceUpdateInEdges = false
        )
        {
            var result = new List<Rectangle>();
            frmGraph.UpdateNodeStyle(targetNode, nodeColor, nodeLabelFontColor, width);
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
                        frmGraph.UpdateEdgeStyle(ie, edgeColor, edgeLabelFontColor, width);
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
                        frmGraph.UpdateEdgeStyle(ie, edgeColor, edgeLabelFontColor, width);
                        result.Add(ie.BoundingBox);
                    }
                }
            }

            return result;
        }

        private static void UpdateEdgeStyle(Edge edge, Color edgeColor, Color labelFontColor, double width)
        {
            edge.Label.FontColor = labelFontColor;
            edge.Attr.Color = edgeColor;
            edge.Attr.LineWidth = width;
        }

        private static void UpdateNodeStyle(Node node, Color nodeColor, Color labelFontColor, double width)
        {
            node.Label.FontColor = labelFontColor;
            node.Attr.Color = nodeColor;
            node.Attr.LineWidth = width;
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

                        if (this.settings.SpikeVisualization.Enabled)
                            frmGraph.UpdateEdgeStyle(edge, frmGraph.InactiveColor, frmGraph.InactiveColor, frmGraph.InitialWidth);
                    }

                foreach (var n in this.spikable.Network.GetItems<Neuron>().Where(n => cns.Any(cn => cn.Id == n.Id)))
                {
                    var node = graph.FindNode(n.Id.ToString());
                    if (node != null)
                    {
                        var tag = n.Tag;
                        if (!string.IsNullOrWhiteSpace(tag) && this.settings.ShortenMirrorTags)
                        {
                            var lastIndex = tag.LastIndexOfAny(['.', '+']);
                            if (lastIndex > -1)
                                tag = tag.Substring(lastIndex + 1);
                        }
                        node.LabelText = tag;
                        node.UserData = n;

                        if (string.IsNullOrEmpty(tag))
                            node.Attr.Shape = Shape.Circle;

                        if (this.settings.SpikeVisualization.Enabled)
                            frmGraph.UpdateNodeStyle(node, frmGraph.InactiveColor, frmGraph.InactiveColor, frmGraph.InitialWidth);
                    }
                }
                this.gViewer1.CurrentLayoutMethod = LayoutMethod.MDS;
                this.gViewer1.Graph = graph;

                frmGraph.UpdateName(this.spikable, this);
            }
        }

        private static void UpdateName(ISpikableReporting2 spikable, frmGraph form)
        {
            var fullName = string.Empty;
            var lifeText = string.Empty;
            if (spikable is IComponent component)
                fullName = component.GetFullName();
            if (spikable is IPerishable perishable && perishable.Life <= 0)
                lifeText = $"{(!string.IsNullOrEmpty(fullName) ? " " : string.Empty)}[Dead]";

            form.Text =
                $"{fullName}{lifeText}" +
                $"{(!(string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(lifeText)) ?
                        " - " :
                        string.Empty
                    )}" +
                $"Graph";
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
            if (this.settings.SpikeVisualization.Enabled)
            {
                var activeSpikes = this.spikes.Where(s => s.Value.Status != Status.NotSet);

                foreach (var spike in activeSpikes)
                {
                    if (spike.Value.Status == Status.FiredPreviously || spike.Value.Status == Status.TriggeredPreviously)
                    {
                        if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > (
                                (this.settings.SpikeVisualization.ResetPeriod + this.settings.SpikeVisualization.SustainPeriod) / 1000
                            )
                        )
                            frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.NotSet);
                    }
                    else if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > (this.settings.SpikeVisualization.ResetPeriod / 1000))
                    {
                        if (spike.Value.Status == Status.Triggered && this.settings.SpikeVisualization.SustainTriggered)
                            frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.TriggeredPreviously);
                        else if (spike.Value.Status == Status.Fired && this.settings.SpikeVisualization.SustainFired)
                            frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.FiredPreviously);
                        else
                            frmGraph.UpdateNode(this.gViewer1, this.spikes, Enumerable.Empty<Guid>(), spike.Key, Status.NotSet);
                    }
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedComponents(new[] { this.settings });
        }
    }
}
