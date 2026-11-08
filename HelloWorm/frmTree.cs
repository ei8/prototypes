using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using ei8.Cortex.Coding.d23.Math.Logic;
using ei8.Cortex.Coding.d23.Math.TruthTables;
using ei8.Cortex.Coding.d23.Process;
using ei8.Cortex.Coding.d23.Process.Iteration;
using ei8.Cortex.Coding.d23.Process.Operation;
using neurUL.Common.Domain.Model;
using NLog;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;
using Comparator = ei8.Cortex.Coding.d23.Process.Operation.Comparator;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmTree : DockContent
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string FormDescription = "Tree";
        private readonly ISpikableReporting? spikable;
        private readonly ISelectionService selectionService;
        private IProcess? process;
        private bool isProcesing = false;

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

                this.process.HandleFire(e.FireInfo.Target);
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

                this.StartProcess(
                    new DoUntil
                    (
                        new
                        (
                            new NeuronChunk(checkedNeurons[0]),
                            new EnumerableChunk<INeuronChunk>(this.spikable.Network.GetItems<Neuron>().Where(n => n.Tag.ToUpper().StartsWith("STEP")).Select(n => new NeuronChunk(n))),
                            new NeuronChunk(checkedNeurons[1]),
                            new NeuronChunk(checkedNeurons[2])
                        ),
                        null,
                        (du) => this.StopProcessing()
                    )
                );
            }
        }

        private void StartProcess(IProcess process)
        {
            AssertionConcern.AssertArgumentFalse(this.isProcesing, "Cannot start process as another one is already in progress.");

            this.process = process;
            this.isProcesing = true;
            this.timer1.Start();
        }

        private void StopProcessing()
        {
            this.isProcesing = false;
            this.timer1.Stop();
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

                ArgumentNullException.ThrowIfNull(addend1_1);
                ArgumentNullException.ThrowIfNull(addend1_0);
                ArgumentNullException.ThrowIfNull(addend2_1);
                ArgumentNullException.ThrowIfNull(addend2_0);
                ArgumentNullException.ThrowIfNull(sum_1);
                ArgumentNullException.ThrowIfNull(sum_0);
                ArgumentNullException.ThrowIfNull(carryOver_1);
                ArgumentNullException.ThrowIfNull(carryOver_0);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_1);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_0);

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

                this.StartProcess(
                    new SequentialAddition
                    (
                        new Addition.WorkingMemoryInfo
                        (
                            new EnumerableChunk<INeuronChunk>
                            (
                                [
                                    new NeuronChunk(precedingCarryOver_1.Single()),
                                    new NeuronChunk(precedingCarryOver_0.Single())
                                ]
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend1_1.Single()),
                                        new NeuronChunk(addend1_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        .. addends[0].Select(a => a == '0' ? addend1_0.Single() : addend1_1.Single()).Reverse().Select(n => new NeuronChunk(n))
                                    ]
                                )
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend2_1.Single()),
                                        new NeuronChunk(addend2_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        .. addends[1].Select(a => a == '0' ? addend2_0.Single() : addend2_1.Single()).Reverse().Select(n => new NeuronChunk(n))
                                    ]
                                )
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(sum_1.Single()),
                                        new NeuronChunk(sum_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.Sum)
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(carryOver_1.Single()),
                                        new NeuronChunk(carryOver_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.CarryOver)
                            )
                        ),
                        new
                        (
                            new NeuronChunk(checkedNeurons[0]),
                            new EnumerableChunk<INeuronChunk>(this.spikable.Network.GetItems<Neuron>().Where(n => n.Tag.ToUpper().StartsWith("DIGIT")).Select(n => new NeuronChunk(n))),
                            new NeuronChunk(checkedNeurons[1])
                        ),
                        (n) => int.Parse(n.Tag.ToUpper().Replace("DIGIT", string.Empty)) - 1,
                        (a, d, s) =>
                        {
                            this.StopProcessing();

                            frmTree.logger.Info
                            (
                                new LogMessageGenerator(() => $"Sum: {string.Join(string.Empty, s.Reverse().Select(s => s.Tag.Last()))}")
                            );
                        }
                    )
                );
            }
        }

        private void tsbStopProcess_Click(object sender, EventArgs e)
        {
            this.StopProcessing();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.spikable != null && this.process != null && this.isProcesing)
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
                            typeof(ITruthTable)
                                .IsAssignableFrom(x) &&
                            !x.IsInterface &&
                            !x.IsAbstract &&
                            x.Namespace != null &&
                            x.Namespace.Contains("Math.Logic")
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

                ArgumentNullException.ThrowIfNull(addend1_1);
                ArgumentNullException.ThrowIfNull(addend1_0);
                ArgumentNullException.ThrowIfNull(addend2_1);
                ArgumentNullException.ThrowIfNull(addend2_0);
                ArgumentNullException.ThrowIfNull(sum_1);
                ArgumentNullException.ThrowIfNull(sum_0);
                ArgumentNullException.ThrowIfNull(carryOver_1);
                ArgumentNullException.ThrowIfNull(carryOver_0);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_1);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_0);

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

                this.StartProcess(
                    new DynamicAddition
                    (
                        new Addition.WorkingMemoryInfo
                        (
                            new EnumerableChunk<INeuronChunk>
                            (
                                [
                                    new NeuronChunk(precedingCarryOver_1.Single()),
                                    new NeuronChunk(precedingCarryOver_0.Single())
                                ]
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend1_1.Single()),
                                        new NeuronChunk(addend1_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        .. addends[0].Select(a => a == '0' ? addend1_0.Single() : addend1_1.Single()).Reverse().Select(n => new NeuronChunk(n))
                                    ]
                                )
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend2_1.Single()),
                                        new NeuronChunk(addend2_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        .. addends[1].Select(a => a == '0' ? addend2_0.Single() : addend2_1.Single()).Reverse().Select(n => new NeuronChunk(n))
                                    ]
                                )
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(sum_1.Single()),
                                        new NeuronChunk(sum_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.Sum)
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(carryOver_1.Single()),
                                        new NeuronChunk(carryOver_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.CarryOver)
                            )
                        ),
                        (a, s) =>
                        {
                            this.StopProcessing();

                            frmTree.logger.Info
                            (
                                new LogMessageGenerator(() => $"Sum: {string.Join(string.Empty, s.Reverse().Select(s => s.Tag.Last()))}")
                            );
                        }
                    )
                );
            }
        }

        private void dynamicMultiplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
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

                this.spikable.Network.TryGetByTag("Multiplier.Multiplicand = 1", out var multiplicand_1);
                this.spikable.Network.TryGetByTag("Multiplier.Multiplicand = 0", out var multiplicand_0);
                this.spikable.Network.TryGetByTag("Multiplier.Multiplier = 1", out var multiplier_1);
                this.spikable.Network.TryGetByTag("Multiplier.Multiplier = 0", out var multiplier_0);
                this.spikable.Network.TryGetByTag("Multiplier.Product = 1", out var product_1);
                this.spikable.Network.TryGetByTag("Multiplier.Product = 0", out var product_0);

                ArgumentNullException.ThrowIfNull(addend1_1);
                ArgumentNullException.ThrowIfNull(addend1_0);
                ArgumentNullException.ThrowIfNull(addend2_1);
                ArgumentNullException.ThrowIfNull(addend2_0);
                ArgumentNullException.ThrowIfNull(sum_1);
                ArgumentNullException.ThrowIfNull(sum_0);
                ArgumentNullException.ThrowIfNull(carryOver_1);
                ArgumentNullException.ThrowIfNull(carryOver_0);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_1);
                ArgumentNullException.ThrowIfNull(precedingCarryOver_0);

                ArgumentNullException.ThrowIfNull(multiplicand_1);
                ArgumentNullException.ThrowIfNull(multiplicand_0);
                ArgumentNullException.ThrowIfNull(multiplier_1);
                ArgumentNullException.ThrowIfNull(multiplier_0);
                ArgumentNullException.ThrowIfNull(product_1);
                ArgumentNullException.ThrowIfNull(product_0);

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

                this.StartProcess(
                    new DynamicMultiplication
                    (
                        new DynamicMultiplication.WorkingMemoryInfo
                        (
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>(
                                    [
                                        new NeuronChunk(multiplicand_1.Single()),
                                        new NeuronChunk(multiplicand_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>(
                                    factors[0].Select(f => f == '0' ? multiplicand_0.Single() : multiplicand_1.Single()).Reverse().Select(n => new NeuronChunk(n)).ToArray()
                                )
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>(
                                    [
                                        new NeuronChunk(multiplier_1.Single()),
                                        new NeuronChunk(multiplier_0.Single())
                                    ]
                                ),
                                new EnumerableChunk<INeuronChunk>(
                                    factors[1].Select(f => f == '0' ? multiplier_0.Single() : multiplier_1.Single()).Reverse().Select(n => new NeuronChunk(n)).ToArray()
                                )
                            ),
                            new NestedNeuronListChunk(),
                            null,
                            null,
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>(
                                    [
                                        new NeuronChunk(product_1.Single()),
                                        new NeuronChunk(product_0.Single())
                                    ]
                                ),
                                nameof(DynamicMultiplication.WorkingMemoryInfo.Product)
                            )
                        ),
                        new Addition.WorkingMemoryInfo
                        (
                            new EnumerableChunk<INeuronChunk>
                            (
                                [
                                    new NeuronChunk(precedingCarryOver_1.Single()),
                                    new NeuronChunk(precedingCarryOver_0.Single())
                                ]
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend1_1.Single()),
                                        new NeuronChunk(addend1_0.Single())
                                    ]
                                )
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(addend2_1.Single()),
                                        new NeuronChunk(addend2_0.Single())
                                    ]
                                )
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(sum_1.Single()),
                                        new NeuronChunk(sum_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.Sum)
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(carryOver_1.Single()),
                                        new NeuronChunk(carryOver_0.Single())
                                    ]
                                ),
                                nameof(Addition.WorkingMemoryInfo.CarryOver)
                            )
                        ),
                        (dm, p) =>
                        {
                            this.StopProcessing();

                            frmTree.logger.Info
                            (
                                // TODO: Result Reversal should be performed internally
                                new LogMessageGenerator(() => $"Product: {string.Join(string.Empty, p.Reverse().Select(s => s.Tag.Last()))}")
                            );
                        }
                    )
                );
            }
        }

        private void lessThanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spikable != null)
            {
                this.spikable.Network.TryGetByTag("Comparator1.Input1 = 1", out var input1_1);
                this.spikable.Network.TryGetByTag("Comparator1.Input1 = 0", out var input1_0);
                this.spikable.Network.TryGetByTag("Comparator1.Input2 = 1", out var input2_1);
                this.spikable.Network.TryGetByTag("Comparator1.Input2 = 0", out var input2_0);
                this.spikable.Network.TryGetByTag("Comparator1.AreEqual = 1", out var areEqual_1);
                this.spikable.Network.TryGetByTag("Comparator1.AreEqual = 0", out var areEqual_0);
                this.spikable.Network.TryGetByTag("Comparator1.IsLessThan = 1", out var isLessThan_1);
                this.spikable.Network.TryGetByTag("Comparator1.IsLessThan = 0", out var isLessThan_0);
                this.spikable.Network.TryGetByTag("Comparator1.IsGreaterThan = 1", out var isGreaterThan_1);
                this.spikable.Network.TryGetByTag("Comparator1.IsGreaterThan = 0", out var isGreaterThan_0);

                ArgumentNullException.ThrowIfNull(input1_1);
                ArgumentNullException.ThrowIfNull(input1_0);
                ArgumentNullException.ThrowIfNull(input2_1);
                ArgumentNullException.ThrowIfNull(input2_0);
                ArgumentNullException.ThrowIfNull(areEqual_1);
                ArgumentNullException.ThrowIfNull(areEqual_0);
                ArgumentNullException.ThrowIfNull(isLessThan_1);
                ArgumentNullException.ThrowIfNull(isLessThan_0);
                ArgumentNullException.ThrowIfNull(isGreaterThan_1);
                ArgumentNullException.ThrowIfNull(isGreaterThan_0);

                var comparandString = InputBox.ShowDialog
                (
                    this,
                    "Comparands",
                    "Enter two binary comparands separated by a comma (,):",
                    string.Empty
                );

                ArgumentOutOfRangeException.ThrowIfNotEqual(comparandString.Count(c => c == ','), 1);
                var comparands = comparandString.Split(',');
                ArgumentOutOfRangeException.ThrowIfNotEqual(comparands.Length, 2);
                foreach (var factor in comparands)
                    foreach (var chunk16Bit in frmTree.ChunksUpto(factor, 16))
                        int.Parse(chunk16Bit, System.Globalization.NumberStyles.BinaryNumber);

                var comparand1 = new EnumerableChunk<INeuronChunk>([.. comparands[0].Select(c => c == '0' ? input1_0.Single() : input1_1.Single()).Reverse().Select(n => new NeuronChunk(n))]);
                var comparand2 = new EnumerableChunk<INeuronChunk>([.. comparands[1].Select(c => c == '0' ? input2_0.Single() : input2_1.Single()).Reverse().Select(n => new NeuronChunk(n))]);

                this.StartProcess(
                    new DynamicLessThan
                    (
                        new Comparator.WorkingMemoryInfo
                        (
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(input1_1.Single()),
                                        new NeuronChunk(input1_0.Single())
                                    ]
                                ),
                                comparand1,
                                comparand1.Items.Last()
                            ),
                            new OperandChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(input2_1.Single()),
                                        new NeuronChunk(input2_0.Single())
                                    ]
                                ),
                                comparand2,
                                comparand2.Items.Last()
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(areEqual_1.Single()),
                                        new NeuronChunk(areEqual_0.Single())
                                    ]
                                ),
                                nameof(Comparator.WorkingMemoryInfo.AreEqual)
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(isLessThan_1.Single()),
                                        new NeuronChunk(isLessThan_0.Single())
                                    ]
                                ),
                                nameof(Comparator.WorkingMemoryInfo.IsLessThan)
                            ),
                            new ResultChunk
                            (
                                new EnumerableChunk<INeuronChunk>
                                (
                                    [
                                        new NeuronChunk(isGreaterThan_1.Single()),
                                        new NeuronChunk(isGreaterThan_0.Single())
                                    ]
                                ),
                                nameof(Comparator.WorkingMemoryInfo.IsGreaterThan)
                            )
                        ),
                        (dm, p) =>
                        {
                            this.StopProcessing();

                            frmTree.logger.Info
                            (
                                new LogMessageGenerator(() => $"Result: {string.Join(string.Empty, p.Select(s => s.Tag.Last()))}")
                            );
                        }
                    )
                );
            }
        }
    }
}
