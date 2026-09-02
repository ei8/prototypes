using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using ei8.Cortex.Coding.d23.Math.Arithmetic;
using ei8.Cortex.Coding.d23.Math.Logic;
using ei8.Cortex.Coding.d23.Process;
using ei8.Cortex.Coding.d23.Process.Iteration;
using ei8.Cortex.Coding.d23.Process.Operation;
using NLog;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmTree : DockContent
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string FormDescription = "Tree";
        private readonly ISpikableReporting? spikable;
        private readonly ISelectionService selectionService;
        private IProcess? process;

        public frmTree(ISelectionService selectionService)
        {
            InitializeComponent();
            this.selectionService = selectionService;

            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            if (this.selectionService.PrimarySelection is ISpikableReporting spikable)
            {
                this.spikable = spikable;

                if (this.spikable is INamed n)
                    n.PropertyChanged += this.N_PropertyChanged;

                this.spikable.Fired += this.Spikable_Fired;
            }

            this.process = null;
        }

        private void Spikable_Fired(object? sender, Cortex.Coding.Spiker.FiredEventArgs e)
        {
            if (this.spikable != null && this.process != null)
            {
                this.process.Log
                (
                    frmTree.logger,
                    LogLevel.Info,
                    new LogMessageGenerator
                    (
                        () => Cortex.Coding.d23.ExtensionMethods.LogFire(e.FireInfo.Target)
                    )
                );

                this.process.HandleFire(e.FireInfo.Target, this.spikable.Network);
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

                    if (process && this.spikable != null)
                        this.Text = this.spikable.GetName(frmTree.FormDescription);

                    break;
            }
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            this.tsbHideSelectedTags.Enabled =
            this.hideSelectedTagsToolStripMenuItem.Enabled =
            this.mnuHideLogicGatesInterneuronsTags.Enabled =
            this.tsbFocusReflexArc.Enabled =
            this.selectionService.PrimarySelection is IGraph;
        }

        private void frmTree_Load(object sender, EventArgs e)
        {
            this.tsbReload_Click(sender, EventArgs.Empty);
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            if (this.spikable?.Network != null)
            {
                this.listView1.Items.Clear();

                foreach (var n in this.spikable.Network.GetItems<Neuron>())
                    this.AddItem(n);

                this.Text = this.spikable.GetName(frmTree.FormDescription);
            }
        }

        private void AddItem(Neuron n)
        {
            var lvi = this.listView1.Items.Add(n.Id.ToString(), n.Tag, null);
            lvi.SubItems.Add(n.Id.ToString());
            lvi.Checked = false;
            lvi.Tag = n;
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
            if (this.spikable?.Network != null)
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
                    fg.Settings.FilterNeurons = this.GetCheckedNeurons();
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
            if (this.spikable?.Network != null && this.spikable is ISpikable spikable)
            {
                IEnumerable<Neuron> checkedNeurons = this.GetCheckedNeurons();

                spikable.Spike(checkedNeurons.ToArray());
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

        private void tstbFilter_TextChanged(object sender, EventArgs e)
        {
            if (this.spikable?.Network != null)
            {
                this.listView1.Items.Clear();

                foreach (var n in this.spikable.Network.GetItems<Neuron>().Where(n => n.Tag.ToUpper().Contains(this.tstbFilter.Text.ToUpper())))
                    this.AddItem(n);
            }
        }

        private void tsbFocusChecked_Click(object sender, EventArgs e)
        {
            if (this.spikable?.Network != null)
            {
                IEnumerable<Neuron> checkedNeurons = [.. this.GetCheckedNeurons()];

                this.listView1.Items.Clear();

                foreach (var n in this.spikable.Network.GetItems<Neuron>().Where(n => checkedNeurons.Any(cn => cn.Id == n.Id)))
                    this.AddItem(n);
            }
        }

        private void tsbHideSelectedTags_Click(object sender, EventArgs e)
        {
            if
            (
                this.spikable?.Network != null &&
                this.selectionService.PrimarySelection is IGraph fg
            )
            {
                IEnumerable<Neuron> checkedNeurons = [.. this.GetCheckedNeurons()];
                var currentHideTagsNeurons = fg.Settings.HideTagsNeurons.ToArray();
                var newHideTagsNeurons = checkedNeurons.Except(currentHideTagsNeurons);
                fg.Settings.HideTagsNeurons = fg.Settings.HideTagsNeurons.Concat(newHideTagsNeurons);
                fg.Reload();
            }
        }

        private void tsbCopyIds_Click(object sender, EventArgs e)
        {
            if (this.spikable?.Network != null)
            {
                IEnumerable<Neuron> checkedNeurons = [.. this.GetCheckedNeurons()];
                Clipboard.SetText(string.Join(',', checkedNeurons.Select(cn => cn.Id.ToString())));
            }
        }

        private void mnuStartProcessDoUntil_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
                var checkedNeurons = this.GetCheckedNeurons().ToArray();

                ArgumentOutOfRangeException.ThrowIfNotEqual(checkedNeurons.Count(), 3);

                this.timer1.Stop();

                this.process = new DoUntil
                (
                    new
                    (
                        new(checkedNeurons[0]),
                        new(this.spikable.Network.GetItems<Neuron>().Where(n => n.Tag.ToUpper().StartsWith("STEP")).Select(n => new NeuronChunk(n))),
                        new(checkedNeurons[1]),
                        new(checkedNeurons[2])
                    ),
                    null,
                    (du) => this.timer1.Stop()
                );

                this.timer1.Start();
            }
        }

        // Source - https://stackoverflow.com/a/1450889
        // Posted by Eamon Nerbonne, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-08-24, License - CC BY-SA 4.0
        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }

        private void mnuStartProcessAddition_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
                var checkedNeurons = this.GetCheckedNeurons().ToArray();
                ArgumentOutOfRangeException.ThrowIfNotEqual(checkedNeurons.Count(), 2);

                this.timer1.Stop();

                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 1", out var addend1_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 0", out var addend1_0);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 1", out var addend2_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 0", out var addend2_0);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 1", out var sum_1);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 0", out var sum_0);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 1", out var carryOver_1);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 0", out var carryOver_0);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 1", out var precedingCarryOver_1);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 0", out var precedingCarryOver_0);

                var addendString = InputBox.ShowDialog
                (
                    this,
                    "Addends",
                    "Enter two binary addends separated by a comma (,):",
                    string.Empty
                );

                ArgumentOutOfRangeException.ThrowIfNotEqual(addendString.Count(c => c == ','), 1);
                var addends = addendString.Split(',');
                ArgumentOutOfRangeException.ThrowIfNotEqual(addends.Length, 2);
                foreach (var addend in addends)
                    foreach (var chunk16Bit in frmTree.ChunksUpto(addend, 16))
                        int.Parse(chunk16Bit, System.Globalization.NumberStyles.BinaryNumber);

                this.process = new SequentialAddition
                (
                    new
                    (
                        new
                        (
                            [
                                new(precedingCarryOver_1.Single()),
                                new(precedingCarryOver_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend1_1.Single()),
                                new(addend1_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend2_1.Single()),
                                new(addend2_0.Single())
                            ]
                        ),
                        new([.. addends[0].Select(a => a == '0' ? addend1_0.Single() : addend1_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new([.. addends[1].Select(a => a == '0' ? addend2_0.Single() : addend2_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new
                        (
                            [
                                new(sum_1.Single()),
                                new(sum_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(carryOver_1.Single()),
                                new(carryOver_0.Single())
                            ]
                        )
                    ),
                    new
                    (
                        new(checkedNeurons[0]),
                        new(this.spikable.Network.GetItems<Neuron>().Where(n => n.Tag.ToUpper().StartsWith("DIGIT")).Select(n => new NeuronChunk(n))),
                        new(checkedNeurons[1])
                    ),
                    (n) => int.Parse(n.Tag.ToUpper().Replace("DIGIT", string.Empty)) - 1,
                    (a, d, s) =>
                    {
                        this.timer1.Stop();

                        frmTree.logger.Info
                        (
                            new LogMessageGenerator(() => $"Sum: {string.Join(string.Empty, s.Reverse().Select(s => s.Tag.Last()))}")
                        );
                    }
                );

                this.timer1.Start();
            }
        }

        private void tsbStopProcess_Click(object sender, EventArgs e)
        {
            if (this.process != null)
            {
                this.timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.spikable != null && this.process != null)
                this.spikable.Spike(
                    [.. this.process.GetCurrent()]
                );
        }

        private void mnuHideLogicGatesInterneuronsTags_Click(object sender, EventArgs e)
        {
            if
            (
                this.spikable?.Network != null &&
                this.selectionService.PrimarySelection is IGraph fg
            )
            {
                var gateNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where
                    (
                        x =>
                            typeof(ILogicGate)
                                .IsAssignableFrom(x) &&
                            !x.IsInterface &&
                            !x.IsAbstract
                    )
                    .Select(x => x.Name.ToUpper().Replace("GATE", "")).ToList();

                var gateNeurons = this.spikable.Network
                    .GetItems<Neuron>()
                    .Where
                    (
                        n => gateNames.Any(gn => n.Tag.Contains(gn))
                    );

                var currentHideTagsNeurons = fg.Settings.HideTagsNeurons.ToArray();
                var newHideTagsNeurons = gateNeurons.Except(currentHideTagsNeurons);
                fg.Settings.HideTagsNeurons = fg.Settings.HideTagsNeurons.Concat(newHideTagsNeurons);
                fg.Reload();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
                this.selectionService.SetSelectedComponents(new[] { this.listView1.SelectedItems.Cast<ListViewItem>().First().Tag });
        }

        private void setTimerIntervalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var interval = InputBox.ShowDialog(this, "Set timer interval", "Enter an integer value (milliseconds):", "1000");
            this.timer1.Interval = int.Parse(interval);
        }

        private void dynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
                this.timer1.Stop();

                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 1", out var addend1_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 0", out var addend1_0);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 1", out var addend2_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 0", out var addend2_0);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 1", out var sum_1);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 0", out var sum_0);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 1", out var carryOver_1);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 0", out var carryOver_0);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 1", out var precedingCarryOver_1);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 0", out var precedingCarryOver_0);

                var addendString = InputBox.ShowDialog
                (
                    this,
                    "Addends",
                    "Enter two binary addends separated by a comma (,):",
                    string.Empty
                );

                ArgumentOutOfRangeException.ThrowIfNotEqual(addendString.Count(c => c == ','), 1);
                var addends = addendString.Split(',');
                ArgumentOutOfRangeException.ThrowIfNotEqual(addends.Length, 2);
                foreach (var addend in addends)
                    foreach (var chunk16Bit in frmTree.ChunksUpto(addend, 16))
                        int.Parse(chunk16Bit, System.Globalization.NumberStyles.BinaryNumber);

                this.process = new DynamicAddition
                (
                    new
                    (
                        new
                        (
                            [
                                new(precedingCarryOver_1.Single()),
                                new(precedingCarryOver_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend1_1.Single()),
                                new(addend1_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend2_1.Single()),
                                new(addend2_0.Single())
                            ]
                        ),
                        new([.. addends[0].Select(a => a == '0' ? addend1_0.Single() : addend1_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new([.. addends[1].Select(a => a == '0' ? addend2_0.Single() : addend2_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new
                        (
                            [
                                new(sum_1.Single()),
                                new(sum_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(carryOver_1.Single()),
                                new(carryOver_0.Single())
                            ]
                        )
                    ),
                    (a, s) =>
                    {
                        this.timer1.Stop();

                        frmTree.logger.Info
                        (
                            new LogMessageGenerator(() => $"Sum: {string.Join(string.Empty, s.Reverse().Select(s => s.Tag.Last()))}")
                        );
                    }
                );

                this.timer1.Start();
            }
        }

        private void dynamicMultiplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
                this.timer1.Stop();

                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 1", out var addend1_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend1 = 0", out var addend1_0);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 1", out var addend2_1);
                this.spikable.Network.TryGetByTag("Adder1.Addend2 = 0", out var addend2_0);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 1", out var sum_1);
                this.spikable.Network.TryGetByTag("Adder1.Sum = 0", out var sum_0);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 1", out var carryOver_1);
                this.spikable.Network.TryGetByTag("Adder1.CarryOver = 0", out var carryOver_0);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 1", out var precedingCarryOver_1);
                this.spikable.Network.TryGetByTag("PrecedingCarryOver = 0", out var precedingCarryOver_0);

                this.spikable.Network.TryGetByTag("Multiplier1.Multiplicand = 1", out var multiplicand_1);
                this.spikable.Network.TryGetByTag("Multiplier1.Multiplicand = 0", out var multiplicand_0);
                this.spikable.Network.TryGetByTag("Multiplier1.Multiplier = 1", out var multiplier_1);
                this.spikable.Network.TryGetByTag("Multiplier1.Multiplier = 0", out var multiplier_0);
                this.spikable.Network.TryGetByTag("Multiplier1.Product = 1", out var product_1);
                this.spikable.Network.TryGetByTag("Multiplier1.Product = 0", out var product_0);

                var factorString = InputBox.ShowDialog
                (
                    this,
                    "Factors",
                    "Enter two binary factors separated by a comma (,):",
                    string.Empty
                );

                ArgumentOutOfRangeException.ThrowIfNotEqual(factorString.Count(c => c == ','), 1);
                var factors = factorString.Split(',');
                ArgumentOutOfRangeException.ThrowIfNotEqual(factors.Length, 2);
                foreach (var factor in factors)
                    foreach (var chunk16Bit in frmTree.ChunksUpto(factor, 16))
                        int.Parse(chunk16Bit, System.Globalization.NumberStyles.BinaryNumber);

                this.process = new DynamicMultiplication
                (
                    new
                    (
                        new
                        (
                            [
                                new(multiplicand_1.Single()),
                                new(multiplicand_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(multiplier_1.Single()),
                                new(multiplier_0.Single())
                            ]
                        ),
                        new([.. factors[0].Select(f => f == '0' ? multiplicand_0.Single() : multiplicand_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new([.. factors[1].Select(f => f == '0' ? multiplier_0.Single() : multiplier_1.Single()).Reverse().Select(n => new NeuronChunk(n))]),
                        new
                        (
                            [
                                new(product_1.Single()),
                                new(product_0.Single())
                            ]
                        )
                    ),
                    new
                    (
                        new
                        (
                            [
                                new(precedingCarryOver_1.Single()),
                                new(precedingCarryOver_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend1_1.Single()),
                                new(addend1_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(addend2_1.Single()),
                                new(addend2_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(sum_1.Single()),
                                new(sum_0.Single())
                            ]
                        ),
                        new
                        (
                            [
                                new(carryOver_1.Single()),
                                new(carryOver_0.Single())
                            ]
                        )
                    ),
                    (dm, p) =>
                    {
                        this.timer1.Stop();

                        frmTree.logger.Info
                        (
                            new LogMessageGenerator(() => $"Product: {string.Join(string.Empty, p.Reverse().Select(s => s.Tag.Last()))}")
                        );
                    }
                );

                this.timer1.Start();
            }
        }
    }
}
