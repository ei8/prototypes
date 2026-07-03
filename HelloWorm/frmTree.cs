using ei8.Cortex.Coding;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmTree : DockContent
    {
        private const string FormDescription = "Tree";
        private readonly ISpikableReporting2 spikable;
        private readonly ISelectionService selectionService;

        public frmTree(ISelectionService selectionService)
        {
            InitializeComponent();
            this.selectionService = selectionService;

            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            if (this.selectionService.PrimarySelection is ISpikableReporting2 spikable)
            {
                this.spikable = spikable;

                if (this.spikable is INamed n)
                    n.PropertyChanged += this.N_PropertyChanged;
            }
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
                        this.Text = this.spikable.GetName(frmTree.FormDescription);

                    break;
            }
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            this.tsbFocusReflexArc.Enabled = this.selectionService.PrimarySelection is IGraph;
        }

        private void frmTree_Load(object sender, EventArgs e)
        {
            this.tsbReload_Click(sender, EventArgs.Empty);
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            if (this.spikable.Network != null)
            {
                this.listView1.Items.Clear();

                foreach (var n in this.spikable.Network.GetItems<Neuron>())
                {
                    var lvi = this.listView1.Items.Add(n.Id.ToString(), n.Tag, null);
                    lvi.SubItems.Add(n.Id.ToString());
                    lvi.Checked = true;
                    lvi.Tag = n;
                }

                this.Text = this.spikable.GetName(frmTree.FormDescription);
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
            if (this.spikable.Network != null)
            {
                IEnumerable<Neuron> checkedNeurons = this.GetCheckedNeurons();

                var posts = new List<Neuron>();
                foreach (var cn in checkedNeurons)
                {
                    posts.AddRange(this.spikable.Network.GetPostsynapticNeurons(cn.Id));
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

                    outputNeurons = this.spikable.Network.GetPostsynapticNeurons(interneuron.Id);

                    foreach (var on in outputNeurons)
                    {
                        var onlvi = this.listView1.Items[on.Id.ToString()];

                        if (onlvi != null)
                            onlvi.Checked = true;
                    }
                }

                if (this.selectionService.PrimarySelection is IGraph fg)
                {
                    fg.FilterNeurons = this.GetCheckedNeurons();
                    fg.Reload();
                }
            }
        }

        private IEnumerable<Neuron> GetCheckedNeurons()
        {
            return this.listView1.Items
                .Cast<ListViewItem>()
                .Where(lvi => lvi.Checked)
                .Select(lvi => (Neuron)lvi.Tag!);
        }

        private void tsbSpike_Click(object sender, EventArgs e)
        {
            if (this.spikable.Network != null && this.spikable is ISpikableTemp spikableTemp)
            {
                IEnumerable<Neuron> checkedNeurons = this.GetCheckedNeurons();

                spikableTemp.Spike(checkedNeurons.ToArray());
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.tsbSpike_Click(sender, EventArgs.Empty);
                    break;
            }
        }

        private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case ' ':
                    this.tsbCheckSelected_Click(sender, EventArgs.Empty);
                    break;
            }
        }
    }
}
