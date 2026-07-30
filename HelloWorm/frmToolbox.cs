using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using ei8.Cortex.Coding.d23.Collections;
using ei8.Cortex.Coding.d23.Math.Arithmetic;
using ei8.Cortex.Coding.d23.Math.Logic;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
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

                    newWorm.Load(queryResult.ToNetwork());
                    newWorm.Initialize(this.settingsService.Mirrors);
                    dish.Add(newWorm);
                }
            }
        }

        private void tsbWorksheet_Click(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish dish)
            {
                string option = InputBox.ShowDialog(this, "1 - Logic Gates; 2 - Addition; 3 - Subtraction; 4 - Next", string.Empty);

                var sheet = this.serviceProvider.GetRequiredService<Worksheet>();
                var suffix = string.Empty;
                if (!string.IsNullOrWhiteSpace(option))
                {
                    switch (option)
                    {
                        case "1":
                            sheet.Load(frmToolbox.CreateLogicGates());
                            suffix = "Logic Gates";
                            break;
                        case "2":
                            sheet.Load(frmToolbox.CreateAdders(4));
                            suffix = "Addition";
                            break;
                        case "3":
                            sheet.Load(frmToolbox.CreateSubtractors(4));
                            suffix = "Subtraction";
                            break;
                        case "4":
                            sheet.Load(frmToolbox.CreateNexts(10));
                            suffix = "Next";
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

        private static ReadOnlyNetwork CreateNexts(int last)
        {
            Network net = new();

            if (
                UnaryNeuronInfo.TryCreate(out var currentStep, parameterExpression: "step0") &&
                NetworkHelper.TryCreateNeuron(out var NEXT)
            )
            {
                net.AddReplace(NEXT);
                for(int i = 1; i <= last; i++)
                {
                    if (
                        UnaryNeuronInfo.TryCreate(out var nextStep, parameterExpression: "step" + i) &&
                        currentStep.VariableInfo != null &&
                        Next.TryCreate(
                            out Next? n,
                            new([currentStep], [nextStep]),
                            $"{nameof(NEXT)}___{currentStep.VariableInfo.Inputs.Single()}",
                            additionalInputs: NEXT
                        )
                    )
                    {
                        net.AddReplaceItems(n);
                        currentStep = nextStep;
                    }
                }
            }

            return net;
        }

        private static ReadOnlyNetwork CreateLogicGates()
        {
            Network net = new();
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
                LogicGateBase.TryCreate(out NotGate? NOT___Input1, new FunctionalParameter<BinaryNeuronInfo>([inputs[0]], [result]), additionalInputs: NOT) &&
                LogicGateBase.TryCreate(out NotGate? NOT___Input2, new FunctionalParameter<BinaryNeuronInfo>([inputs[1]], [result]), additionalInputs: NOT) &&
                LogicGateBase.TryCreate(out AndGate? AND___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: AND) &&
                LogicGateBase.TryCreate(out OrGate? OR___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: OR) &&
                LogicGateBase.TryCreate(out NandGate? NAND___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: NAND) &&
                LogicGateBase.TryCreate(out NorGate? NOR___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: NOR) &&
                LogicGateBase.TryCreate(out XorGate? XOR___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: XOR) &&
                LogicGateBase.TryCreate(out XnorGate? XNOR___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: XNOR) &&
                LogicGateBase.TryCreate(out ImplyGate? IMPLY___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: IMPLY) &&
                LogicGateBase.TryCreate(out NimplyGate? NIMPLY___Input1__Input2, new FunctionalParameter<BinaryNeuronInfo>(inputs, [result]), additionalInputs: NIMPLY)
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
            return net;
        }

        private static ReadOnlyNetwork CreateAdders(int count)
        {
            Network net = new();
            BinaryNeuronInfo? precedingCarryOver = null;
            VariableInfo? precedingVariableInfo = null;
            for (int i = 0; i < count; i++)
            {
                if (Adder.TryCreate(out Adder? a, i, precedingCarryOver, precedingVariableInfo, nameof(Adder) + (i +1)))
                {
                    net.AddReplaceItems(a);
                    precedingCarryOver = a.Parameters.Outputs.ElementAt((int)Adder.Output.CarryOver);
                    precedingVariableInfo = a.VariableInfo;
                }
            }
            return net;
        }

        private static ReadOnlyNetwork CreateSubtractors(int count)
        {
            Network net = new();
            BinaryNeuronInfo? precedingBorrow = null;
            VariableInfo? precedingVariableInfo = null;
            for (int i = 0; i < count; i++)
            {
                if (Subtractor.TryCreate(out Subtractor? s, i, precedingBorrow, precedingVariableInfo, nameof(Subtractor) + (i + 1)))
                {
                    net.AddReplaceItems(s);
                    precedingBorrow = s.Parameters.Outputs.ElementAt((int)Subtractor.Output.Borrow);
                    precedingVariableInfo = s.VariableInfo;
                }
            }
            return net;
        }
    }
}
