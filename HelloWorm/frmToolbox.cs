using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using ei8.Prototypes.HelloWorm.Math.Arithmetic;
using ei8.Prototypes.HelloWorm.Math.Logic;
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
                string option = InputBox.ShowDialog(this, "1 - Logic Gates; 2 - Addition; 3 - Subtraction", string.Empty);

                var sheet = this.serviceProvider.GetRequiredService<Worksheet>();
                var suffix = string.Empty;
                if (!string.IsNullOrWhiteSpace(option))
                {
                    switch (option)
                    {
                        case "1":
                            frmToolbox.CreateLogicGates(sheet.Network);
                            suffix = "Logic Gates";
                            break;
                        case "2":
                            frmToolbox.CreateAdders(sheet.Network, 4);
                            suffix = "Addition";
                            break;
                        case "3":
                            frmToolbox.CreateSubtractors(sheet.Network, 4);
                            suffix = "Subtraction";
                            break;
                    }
                }
                
                sheet.Initialize(
                    ExtensionMethods.CreateUnusedName(
                        (i) => $"{nameof(Worksheet)}{i.ToString()} ({suffix})",
                        (s) => dish.Components.OfType<INamed>().Any(dcn => dcn.Name == s)
                    ),
                    dish
                );
                sheet.Initialize(this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static void CreateLogicGates(Network net)
        {
            BinaryNeuronInfo[] inputs = [
                BinaryNeuronInfo.Create("Input1", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper()),
                BinaryNeuronInfo.Create("Input2", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper())
            ];

            if (
                BinaryNeuronInfo.TryCreate(out var result, trueString: Boolean.TrueString.ToUpper(), falseString: Boolean.FalseString.ToUpper()) && // rotateConfig);
                NetworkHelper.TryCreateNeuron(out var NOT) &&
                NetworkHelper.TryCreateNeuron(out var AND) &&
                NetworkHelper.TryCreateNeuron(out var OR) &&
                NetworkHelper.TryCreateNeuron(out var NAND) &&
                NetworkHelper.TryCreateNeuron(out var NOR) &&
                NetworkHelper.TryCreateNeuron(out var XOR) &&
                NetworkHelper.TryCreateNeuron(out var XNOR) &&
                NetworkHelper.TryCreateNeuron(out var IMPLY) &&
                NetworkHelper.TryCreateNeuron(out var NIMPLY) &&
                LogicGateBase.TryCreate(out NotGate? NOT___Input1, new ParameterInfo([inputs[0]], [result]), additionalInputs: NOT) &&
                LogicGateBase.TryCreate(out NotGate? NOT___Input2, new ParameterInfo([inputs[1]], [result]), additionalInputs: NOT) &&
                LogicGateBase.TryCreate(out AndGate? AND___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: AND) &&
                LogicGateBase.TryCreate(out OrGate? OR___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: OR) &&
                LogicGateBase.TryCreate(out NandGate? NAND___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: NAND) &&
                LogicGateBase.TryCreate(out NorGate? NOR___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: NOR) &&
                LogicGateBase.TryCreate(out XorGate? XOR___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: XOR) &&
                LogicGateBase.TryCreate(out XnorGate? XNOR___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: XNOR) &&
                LogicGateBase.TryCreate(out ImplyGate? IMPLY___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: IMPLY) &&
                LogicGateBase.TryCreate(out NimplyGate? NIMPLY___Input1__Input2, new ParameterInfo(inputs, [result]), additionalInputs: NIMPLY)
            )
            {
                // "Nothing is True, Everything is permitted"
                net.AddReplaceItems(
                    [
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
                        ..inputs
                    ]
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
            }
        }

        private static void CreateAdders(Network net, int count)
        {
            BinaryNeuronInfo? precedingCarryOver = null;
            for (int i = 0; i < count; i++)
            {
                Adder a;
                net.AddReplaceItems(a = new Adder(i, precedingCarryOver));
                precedingCarryOver = a.Parameters.Outputs[(int) Adder.Output.CarryOver];
            }
        }

        private static void CreateSubtractors(Network net, int count)
        {
            BinaryNeuronInfo? precedingBorrow = null;
            for (int i = 0; i < count; i++)
            {
                Subtractor s;
                net.AddReplaceItems(s = new Subtractor(i, precedingBorrow));
                precedingBorrow = s.Parameters.Outputs[(int) Subtractor.Output.Borrow];
            }
        }
    }
}
