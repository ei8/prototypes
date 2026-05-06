using ei8.Cortex.Coding;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmTree : DockContent
    {
        private readonly Worm worm;
        private readonly ISelectionService selectionService;

        public frmTree(ISelectionService selectionService)
        {
            InitializeComponent();
            this.selectionService = selectionService;

            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            if (this.selectionService.PrimarySelection is Worm w)
                this.worm = w;
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            this.tsbFocusReflexArc.Enabled = this.selectionService.PrimarySelection is frmGraph;
        }

        private void frmTree_Load(object sender, EventArgs e)
        {
            this.tsbReload_Click(sender, EventArgs.Empty);
        }

        private void tsbReload_Click(object sender, EventArgs e)
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
                IEnumerable<Neuron> checkedNeurons = this.GetCheckedNeurons();

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

                if (this.selectionService.PrimarySelection is frmGraph fg)
                    fg.Reload(this.GetCheckedNeurons());
            }
        }

        private IEnumerable<Neuron> GetCheckedNeurons()
        {
            return this.listView1.Items
                .Cast<ListViewItem>()
                .Where(lvi => lvi.Checked)
                .Select(lvi => (Neuron)lvi.Tag!);
        }
    }
}
