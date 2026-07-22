using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using ei8.Prototypes.HelloWorm.Math.Arithmetic;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmToolbox : DockContent
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISelectionService selectionService;
        private readonly ISettingsService settingsService;
        private readonly INeuronQueryClient neuronQueryClient;

        public frmToolbox(
            IServiceProvider serviceProvider,
            ISelectionService selectionService,
            ISettingsService settingsService,
            INeuronQueryClient neuronQueryClient
        )
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            this.settingsService = settingsService;
            this.neuronQueryClient = neuronQueryClient;

            this.HideOnClose = true;
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            bool isDish = this.selectionService.PrimarySelection is Dish;
            this.tsbFood.Visible =
                this.tsbWorm.Visible =
                isDish;

            this.tslblNoUsableControls.Visible = !isDish;
        }

        private void tsbFood_Click(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish d)
            {
                var newFood = this.serviceProvider.GetRequiredService<Food>();
                newFood.Initialize(
                    ExtensionMethods.CreateUnusedName(
                        (i) => $"{nameof(Food)}{i.ToString()}",
                        (s) => d.Components.OfType<INamed>().Any(dcn => dcn.Name == s)
                    ),
                    d
                );
                d.Add(newFood);
            }
        }

        private async void tsbWorm_Click(object sender, EventArgs e)
        {
            if (
                this.selectionService.PrimarySelection is Dish dish &&
                this.settingsService.Mirrors != null
            )
            {
                string neuronQuery = InputBox.ShowDialog(this, "neurUL Query", "http://fibona.cc/worm1/av8r/cortex/neurons?sortorder=1&sortby=1&pagesize=29&depth=5&direction=1");
                if (!string.IsNullOrEmpty(neuronQuery) &&
                    neuronQuery.Contains('?') &&
                    QueryUrl.TryParse(neuronQuery, out QueryUrl queryUrl) &&
                    ei8.Cortex.Library.Common.NeuronQuery.TryParse(queryUrl.QueryString, out ei8.Cortex.Library.Common.NeuronQuery query)
                )
                {
                    var queryResult = await this.neuronQueryClient.GetNeurons(
                        queryUrl.AvatarUrl,
                        query,
                        "Guest"
                    );

                    var newWorm = this.serviceProvider.GetRequiredService<Worm>();
                    newWorm.Initialize(
                        ExtensionMethods.CreateUnusedName(
                            (i) => $"{nameof(Worm)}{i.ToString()}",
                            (s) => dish.Components.OfType<INamed>().Any(dcn => dcn.Name == s)
                        ),
                        dish
                    );

                    newWorm.Network.AddReplaceItems(queryResult.ToNetwork());
                    newWorm.Initialize(this.settingsService.Mirrors);
                    dish.Add(newWorm);
                }
            }
        }

        private void tsbWorksheet_Click(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish dish)
            {
                var sheet = this.serviceProvider.GetRequiredService<Worksheet>();
                sheet.Initialize(
                    ExtensionMethods.CreateUnusedName(
                            (i) => $"{nameof(Worksheet)}{i.ToString()}",
                            (s) => dish.Components.OfType<INamed>().Any(dcn => dcn.Name == s)
                        ),
                        dish
                    );

                // frmToolbox.CreateLogicGates(sheet.Network);
                // frmToolbox.CreateAdders(sheet.Network, 4);
                frmToolbox.CreateSubtractors(sheet.Network, 4);

                sheet.Initialize(this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static void CreateLogicGates(Network net)
        {
            if (
                BinaryNeuronInfo.TryCreate(out var result, trueString: Boolean.TrueString.ToUpper(), falseString: Boolean.FalseString.ToUpper()) && // rotateConfig);
                InverterInterneuronInfo.TryCreate(out var NOT___Input1, result) &&
                InverterInterneuronInfo.TryCreate(out var NOT___Input2, result) &&
                TruthTableInterneuronInfo.TryCreate(out var AND___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.And, result) &&
                TruthTableInterneuronInfo.TryCreate(out var OR___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Or, result) &&
                TruthTableInterneuronInfo.TryCreate(out var NAND___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Nand, result) &&
                TruthTableInterneuronInfo.TryCreate(out var NOR___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Nor, result) &&
                TruthTableInterneuronInfo.TryCreate(out var XOR___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Xor, result) &&
                TruthTableInterneuronInfo.TryCreate(out var XNOR___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Xnor, result) &&
                TruthTableInterneuronInfo.TryCreate(out var IMPLY___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Imply, result) &&
                TruthTableInterneuronInfo.TryCreate(out var NIMPLY___Input1__Input2, TruthTableInterneuronInfo.LogicGateType.Nimply, result) &&
                NetworkHelper.TryCreateNeuron(out var NOT) &&
                NetworkHelper.TryCreateNeuron(out var AND) &&
                NetworkHelper.TryCreateNeuron(out var OR) &&
                NetworkHelper.TryCreateNeuron(out var NAND) &&
                NetworkHelper.TryCreateNeuron(out var NOR) &&
                NetworkHelper.TryCreateNeuron(out var XOR) &&
                NetworkHelper.TryCreateNeuron(out var XNOR) &&
                NetworkHelper.TryCreateNeuron(out var IMPLY) &&
                NetworkHelper.TryCreateNeuron(out var NIMPLY)
            )
            {                 
                var inputs = InputInfo.Create(
                    // input1TrueConfig,
                    "Input1",
                    // input2TrueConfig,
                    "Input2",
                    Boolean.TrueString.ToUpper(),
                    Boolean.FalseString.ToUpper()
                );

                // "Nothing is True, Everything is permitted"
                net.AddReplaceItems(
                    result,
                    NOT___Input1,
                    NOT___Input2,
                    AND___Input1__Input2,
                    OR___Input1__Input2,
                    NAND___Input1__Input2,
                    NOR___Input1__Input2,
                    XOR___Input1__Input2,
                    XNOR___Input1__Input2,
                    IMPLY___Input1__Input2,
                    NIMPLY___Input1__Input2,
                    inputs
                );
                net.AddReplaceItems(
                    [
                        NOT,
                        AND,
                        OR,
                        NAND,
                        NOR,
                        XOR,
                        XNOR,
                        IMPLY,
                        NIMPLY
                    ]
                );
                // Link Input Neurons to Interneurons
                net.AddReplaceItems(
                    NOT___Input1.LinkInputNeurons(
                        inputs.Input1,
                        NOT
                    ),
                    NOT___Input2.LinkInputNeurons(
                        inputs.Input2,
                        NOT
                    ),
                    AND___Input1__Input2.LinkInputNeurons(
                        inputs,
                        AND
                    ),
                    OR___Input1__Input2.LinkInputNeurons(
                        inputs,
                        OR
                    ),
                    NAND___Input1__Input2.LinkInputNeurons(
                        inputs,
                        NAND
                    ),
                    NOR___Input1__Input2.LinkInputNeurons(
                        inputs,
                        NOR
                    ),
                    XOR___Input1__Input2.LinkInputNeurons(
                        inputs,
                        XOR
                    ),
                    XNOR___Input1__Input2.LinkInputNeurons(
                        inputs,
                        XNOR
                    ),
                    IMPLY___Input1__Input2.LinkInputNeurons(
                        inputs,
                        IMPLY
                    ),
                    NIMPLY___Input1__Input2.LinkInputNeurons(
                        inputs,
                        NIMPLY
                    )
                );
            }
        }

        private static void CreateAdders(Network net, int count)
        {
            BinaryNeuronInfo? precedingCarryOver = null;
            for (int i = 0; i < count; i++)
            {
                Adder a;
                net.AddReplaceItems(a = new Adder(i, precedingCarryOver));
                precedingCarryOver = a.CarryOver;
            }
        }

        private static void CreateSubtractors(Network net, int count)
        {
            BinaryNeuronInfo? precedingBorrow = null;
            for (int i = 0; i < count; i++)
            {
                Subtractor s;
                net.AddReplaceItems(s = new Subtractor(i, precedingBorrow));
                precedingBorrow = s.Borrow;
            }
        }
    }
}
