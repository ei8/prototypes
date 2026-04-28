using ei8.Cortex.Coding;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmCode : DockContent
    {
        private readonly Worm worm;
        private readonly ISelectionService selectionService;

        public frmCode(ISelectionService selectionService)
        {
            InitializeComponent();

            this.selectionService = selectionService;

            if (this.selectionService.PrimarySelection is Worm w)
                this.worm = w;

        }

        private void frmCode_Load(object sender, EventArgs e)
        {
            if (this.worm.Network != null)
            {
                foreach (var n in this.worm.Network.GetItems<Neuron>())
                {
                    var lvi = this.listView1.Items.Add(n.Id.ToString(), n.Tag, null);
                    lvi.SubItems.Add(n.Id.ToString());
                    lvi.Checked = true;
                    lvi.Tag = n;
                }
            }

            this.tsbReload_Click(this, EventArgs.Empty);
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            if (this.worm.Network != null)
            {
                var cns = this.listView1.Items
                    .Cast<ListViewItem>()
                    .Where(lvi => lvi.Checked)
                    .Select(lvi => (Neuron)lvi.Tag!);

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

        private void tsbCheckAll_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
            {
                var check = !this.listView1.Items[0].Checked;
                foreach (var lvi in this.listView1.Items.Cast<ListViewItem>())
                {
                    lvi.Checked = check;
                }
            }
        }

        private void tsbCheckSelected_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                var check = !this.listView1.SelectedItems[0].Checked;
                foreach (var lvi in this.listView1.SelectedItems.Cast<ListViewItem>())
                {
                    lvi.Checked = check;
                }
            }
        }

        private void tsbFocusReflexArc_Click(object sender, EventArgs e)
        {
            if (this.worm.Network != null)
            {
                var checkedNeurons = this.listView1.Items
                        .Cast<ListViewItem>()
                        .Where(lvi => lvi.Checked)
                        .Select(lvi => (Neuron)lvi.Tag!);

                var posts = new List<Neuron>();
                foreach (var cn in checkedNeurons)
                {
                    posts.AddRange(this.worm.Network.GetPostsynapticNeurons(cn.Id));
                }

                var groups = posts.GroupBy(n => n)
                    .OrderByDescending(g => g.Count());

                Neuron? interneuron = null;
                if (groups.Count() == 1 || (groups.Count() > 1 && groups.First().Count() > groups.ElementAt(1).Count()))
                    interneuron = groups
                        .Select(n => n.Key)
                        .FirstOrDefault();

                var outputNeurons = Enumerable.Empty<Neuron>();

                if (interneuron != null)
                {
                    var inlvi = this.listView1.Items[interneuron.Id.ToString()];

                    if (inlvi != null)
                        inlvi.Checked = true;

                    outputNeurons = this.worm.Network.GetPostsynapticNeurons(interneuron.Id);

                    foreach (var on in outputNeurons)
                    {
                        var onlvi = this.listView1.Items[on.Id.ToString()];

                        if (onlvi != null)
                            onlvi.Checked = true;
                    }
                }

                this.tsbReload_Click(this, EventArgs.Empty);
            }
        }

        private void tsbChangeOrientation_Click(object sender, EventArgs e)
        {
            if (this.splitContainer1.Orientation == System.Windows.Forms.Orientation.Vertical)
                this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            else
                this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Vertical;
        }
    }
}
