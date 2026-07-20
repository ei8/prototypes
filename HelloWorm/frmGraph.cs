using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;
using Color = Microsoft.Msagl.Drawing.Color;
using Rectangle = Microsoft.Msagl.Core.Geometry.Rectangle;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmGraph : DockContent
    {
        private const int TriggeredWidth = 2;
        private const int FiredPreviouslyWidth = 3;
        private const int FiredWidth = 3;
        private const int InitialWidth = 1;
        private const int EdgeFontSize = 10;
        private const int NodeFontSize = 10;
        private static readonly Color InactiveColor = Color.Silver;

        private readonly ISelectionService selectionService;
        private IGraph graph;

        public frmGraph(ISelectionService selectionService)
        {
            InitializeComponent();
            this.gViewer1.CurrentLayoutMethod = LayoutMethod.MDS;

            this.selectionService = selectionService;

            if (this.selectionService.PrimarySelection is ISpikableReporting spikable)
            {
                this.graph = new Graph(spikable);

                if (this.graph.Spikable is INamed n)
                    n.PropertyChanged += this.N_PropertyChanged;

                this.graph.ActivityLogged += this.Graph_ActivityLogged;
                this.graph.Reloaded += this.Graph_Reloaded;
                this.graph.Settings.PropertyChanged += this.Settings_PropertyChanged;
                this.graph.PropertyChanged += this.Graph_PropertyChanged;
            }
            else
                throw new ArgumentException("Cannot construct Graph without a selected ISpikable.");
        }

        private void Graph_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IGraph.IsPlaying):
                    if (this.graph.IsPlaying)
                        this.graph.Reload();

                    this.timer1.Enabled = this.graph.IsPlaying;
                    break;
            }
        }

        private void Graph_Reloaded(object? sender, EventArgs e)
        {
            if (this.graph.Spikable.Network != null)
            {
                var filterNeurons = this.graph.Spikable.Network.GetItems<Neuron>();

                if (this.graph.Settings.FilterNeurons.Any())
                    filterNeurons = this.graph.Settings.FilterNeurons;

                var graph = new Microsoft.Msagl.Drawing.Graph("graph");

                foreach (var t in this.graph.Spikable.Network.GetItems<Terminal>())
                    if (filterNeurons.Any(fn => fn.Id == t.PresynapticNeuronId) && filterNeurons.Any(fn => fn.Id == t.PostsynapticNeuronId))
                    {
                        var edge = graph.AddEdge(
                            t.PresynapticNeuronId.ToString(),
                            (t.Effect == NeurotransmitterEffect.Excite ? string.Empty : "-") + t.Strength.ToString("0.##"),
                            t.PostsynapticNeuronId.ToString()
                        );

                        edge.UserData = t;

                        if (this.graph.IsPlaying)
                            frmGraph.UpdateEdgeStyle(edge, frmGraph.InactiveColor, frmGraph.InactiveColor, frmGraph.InitialWidth);
                    }

                foreach (var n in this.graph.Spikable.Network.GetItems<Neuron>().Where(n => filterNeurons.Any(fn => fn.Id == n.Id)))
                {
                    var node = graph.FindNode(n.Id.ToString());
                    if (node != null)
                    {
                        var tag = !this.graph.Settings.HideTagsNeurons.Any(htn => htn.Id == n.Id) ?
                            n.Tag :
                            string.Empty;

                        if (
                            !string.IsNullOrWhiteSpace(tag) &&
                            !string.IsNullOrWhiteSpace(n.MirrorUrl) &&
                            this.graph.Settings.ShortenMirrorTags
                        )
                        {
                            var lastIndex = tag.LastIndexOfAny(['.', '+']);
                            if (lastIndex > -1)
                                tag = tag.Substring(lastIndex + 1);
                        }
                        node.LabelText = tag;
                        node.UserData = n;

                        if (string.IsNullOrEmpty(tag))
                            node.Attr.Shape = Shape.Circle;

                        if (this.graph.IsPlaying)
                            frmGraph.UpdateNodeStyle(node, frmGraph.InactiveColor, frmGraph.InactiveColor, frmGraph.InitialWidth);
                    }
                }
                this.gViewer1.Graph = graph;

                this.Text = this.graph.Spikable.GetName(nameof(Graph));
            }
        }

        private void Graph_ActivityLogged(object? sender, ActivityLoggedEventArgs e)
        {
            frmGraph.UpdateNode(
                this.gViewer1,
                e.PresynapticIds,
                e.NeuronId,
                e.NewStatus
            );
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
                        this.Text = this.graph.Spikable.GetName(nameof(Graph));

                    break;
            }
        }

        private static void UpdateNode(GViewer gViewer, IEnumerable<Guid> presynapticIds, Guid targetId, NeuronStatusValue newStatus)
        {
            var targetNode = gViewer.Graph.FindNode(targetId.ToString());

            if (targetNode != null)
            {
                var invalidateRectangles = Enumerable.Empty<Rectangle>();
                switch (newStatus)
                {
                    case NeuronStatusValue.Triggered:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            presynapticIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            Color.Red,
                            Color.Black,
                            Color.Red,
                            Color.Black,
                            frmGraph.TriggeredWidth
                        );
                        break;
                    case NeuronStatusValue.TriggeredPreviously:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            presynapticIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            Color.DarkRed,
                            Color.Black,
                            Color.DarkRed,
                            Color.Black,
                            frmGraph.TriggeredWidth,
                            true
                        );
                        break;
                    case NeuronStatusValue.Fired:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            presynapticIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
                            !targetNode.OutEdges.Any() ?
                                Color.Lime :
                                Color.DodgerBlue,
                            Color.Black,
                            Color.DodgerBlue,
                            Color.Black,
                            frmGraph.FiredWidth
                        );
                        break;
                    case NeuronStatusValue.FiredPreviously:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            presynapticIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
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
                    case NeuronStatusValue.NotSet:
                        invalidateRectangles = frmGraph.UpdateNodeCore(
                            presynapticIds.Select(s => gViewer.Graph.FindNode(s.ToString())),
                            targetNode,
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
            }
        }

        private static IEnumerable<Rectangle> UpdateNodeCore(
            IEnumerable<Node> sourceNodes,
            Node targetNode,
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
            edge.Label.FontSize = frmGraph.EdgeFontSize;
            edge.Label.FontColor = labelFontColor;
            edge.Attr.Color = edgeColor;
            edge.Attr.LineWidth = width;
        }

        private static void UpdateNodeStyle(Node node, Color nodeColor, Color labelFontColor, double width)
        {
            node.Label.FontSize = frmGraph.NodeFontSize;
            node.Label.FontColor = labelFontColor;
            node.Attr.Color = nodeColor;
            node.Attr.LineWidth = width;
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GraphSettings.ShortenMirrorTags):
                    this.graph.Reload();
                    break;
            }
        }

        private void frmGraph_Load(object sender, EventArgs e)
        {
            if (this.graph.Spikable.Network != null)
                this.graph.Reload();
        }

        private void gViewer1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.gViewer1.SelectedObject is DrawingObject @do && @do.UserData != null)
                this.selectionService.SetSelectedComponents(new[] { @do.UserData });

            else
                this.selectionService.SetSelectedComponents(new[] { this.graph });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.graph.ProcessTick();
        }

        private void mnuResetFilters_Click(object sender, EventArgs e)
        {
            if (this.graph.Spikable.Network != null)
            {
                this.graph.Settings.ResetFilters();
                this.graph.Reload();
            }
        }

        private void mnuReload_Click(object sender, EventArgs e)
        {
            if (this.graph.Spikable.Network != null)
            {
                this.graph.Reload();
            }
        }
    }
}
