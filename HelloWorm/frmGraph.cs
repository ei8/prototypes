using ei8.Cortex.Coding;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmGraph : DockContent
    {
        private readonly Worm worm;
        private readonly ISelectionService selectionService;

        public frmGraph(ISelectionService selectionService)
        {
            InitializeComponent();

            this.selectionService = selectionService;

            if (this.selectionService.PrimarySelection is Worm w)
                this.worm = w;
            else
                throw new ArgumentException("Cannot construct Graph without a selected Worm.");
        }

        private void frmGraph_Load(object sender, EventArgs e)
        {
            if (this.worm.Network != null)
                this.Reload(this.worm.Network.GetItems<Neuron>());
        }

        public void Reload(IEnumerable<Neuron> cns)
        {
            if (this.worm.Network != null)
            {
                var graph = new Graph("graph");

                foreach (var t in this.worm.Network.GetItems<Terminal>())
                    if (cns.Any(cn => cn.Id == t.PresynapticNeuronId) && cns.Any(cn => cn.Id == t.PostsynapticNeuronId))
                    {
                        var edge = graph.AddEdge(
                            t.PresynapticNeuronId.ToString(),
                            (t.Effect == NeurotransmitterEffect.Excite ? "+" : "-") + t.Strength.ToString(),
                            t.PostsynapticNeuronId.ToString()
                        );
                    }

                foreach (var n in this.worm.Network.GetItems<Neuron>().Where(n => cns.Any(cn => cn.Id == n.Id)))
                {
                    var node = graph.FindNode(n.Id.ToString());
                    if (node != null)
                    {
                        node.LabelText = n.Tag;
                        node.UserData = n;

                        if (string.IsNullOrEmpty(n.Tag))
                            node.Attr.Shape = Shape.Diamond;
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
    }
}
