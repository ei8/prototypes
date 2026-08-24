using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using ei8.Cortex.Coding.d23.Sequences;
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

        public frmToolbox
        (
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
                        (i) => $"{nameof(Food)}{i}",
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
                string neuronQuery = InputBox.ShowDialog(
                    this, 
                    "neurUL Query", 
                    "Enter neurUL Query",
                    "http://fibona.cc/worm1/av8r/cortex/neurons?sortorder=1&sortby=1&pagesize=29&depth=5&direction=1"
                );
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
                            (i) => $"{nameof(Worm)}{i}",
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
                string option = InputBox.ShowDialog(
                    this,
                    "Select Worksheet Type",
                    string.Join(
                        Environment.NewLine,
                        [
                            "1 - Logic Gates",
                            "2 - Addition - Fast",
                            "3 - Subtraction",
                            "4 - Next",
                            "5 - Biphasic Nexts",
                            "6 - Addition - Sequential",
                            "7 - Addition - Dynamic"
                        ]
                    ), 
                    string.Empty
                );

                var sheet = this.serviceProvider.GetRequiredService<Worksheet>();
                var suffix = string.Empty;
                if (!string.IsNullOrWhiteSpace(option))
                {
                    int numberOfDigits = 1;
                    switch (option)
                    {
                        case "1":
                            sheet.Load(frmToolbox.CreateLogicGates());
                            suffix = "Logic Gates";
                            break;
                        case "2":
                            numberOfDigits = int.Parse(InputBox.ShowDialog(this, "Number of digits", "Enter number of digits:", "1"));
                            ArgumentOutOfRangeException.ThrowIfLessThan(numberOfDigits, 1);
                            sheet.Load(frmToolbox.CreateAdders(numberOfDigits));
                            suffix = "Addition - Fast";
                            break;
                        case "3":
                            sheet.Load(frmToolbox.CreateSubtractors(4));
                            suffix = "Subtraction";
                            break;
                        case "4":
                            sheet.Load(frmToolbox.CreateNexts(10));
                            suffix = "Next";
                            break;
                        case "5":
                            sheet.Load(frmToolbox.CreateBiphasicNexts(4));
                            suffix = "Biphasic Next";
                            break;
                        case "6":
                            numberOfDigits = int.Parse(InputBox.ShowDialog(this, "Number of digits", "Enter number of digits:", "1"));
                            ArgumentOutOfRangeException.ThrowIfLessThan(numberOfDigits, 1);
                            sheet.Load(frmToolbox.CreateSequentialAdders(numberOfDigits));
                            suffix = "Addition - Sequential";
                            break;
                        case "7":
                            sheet.Load(frmToolbox.CreateDynamicAdder());
                            suffix = "Addition - Dynamic";
                            break;
                    }
                }
                
                sheet.Initialize(
                    ExtensionMethods.CreateUnusedName(
                        (i) => $"{nameof(Worksheet)}{i} ({suffix})",
                        (s) => dish.Components.OfType<INamed>().Any(dcn => dcn.Name == s)
                    ),
                    dish
                );
                sheet.Initialize(this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static ReadOnlyNetwork CreateSequentialAdders(int last)
        {
            Network net = new();

            FunctionalCircuitParameter<Adder.Input, Adder.Output>? adderParameters = null;

            if (
                BinaryNeuronParameter.TryCreate(out var precedingCarryOver) &&
                BinaryNeuronParameter.TryCreate(out var addend1) &&
                BinaryNeuronParameter.TryCreate(out var addend2) &&
                BinaryNeuronParameter.TryCreate(out var sum) &&
                BinaryNeuronParameter.TryCreate(out var carryOver) &&
                UnaryNeuronParameter.TryCreate(out var NEXT) &&
                UnaryNeuronParameter.TryCreate(out var currentDigit, parameterExpression: "digit1") &&
                Adder.TryCreate
                (
                    out Adder? adder,
                    0,
                    precedingCarryOver,
                    null
                )
            )
            {
                FunctionalCircuitParameter<Next.Input, Next.Output>? biphasicNextParameters = null;
                Next.InterneuronSet? previousInterneurons = null;
                var inputStrength = 0.25f;

                for (int i = 2; i <= last; i++)
                {
                    if
                    (
                        (
                            adderParameters =
                                new
                                (
                                    new
                                    (
                                        addend1,
                                        addend2,
                                        precedingCarryOver
                                    ),
                                    new
                                    (
                                        sum,
                                        carryOver
                                    )
                                )
                        ) != null &&
                        UnaryNeuronParameter.TryCreate(out var nextDigit, parameterExpression: "digit" + i) &&
                        (
                            biphasicNextParameters =
                                new
                                (
                                    new
                                    (
                                        NEXT,
                                        currentDigit
                                    ),
                                    new
                                    (
                                        nextDigit
                                    )
                                )
                        ) != null &&
                        currentDigit.VariableInfo != null &&
                        Next.TryCreate
                        (
                            out Next? next,
                            biphasicNextParameters,
                            inputStrength,
                            previousInterneurons,
                            $"{nameof(NEXT)}___{currentDigit.VariableInfo.Inputs.Single()}"
                        ) &&
                        SequentialAdder.TryCreate
                        (
                            out SequentialAdder? sequentialAdder,
                            next,
                            adder,
                            inputStrength
                        )
                    )
                    { 
                        net.AddReplaceItems(sequentialAdder);
                        currentDigit = nextDigit;
                        previousInterneurons = sequentialAdder.Next.Interneurons;
                    }
                }
            }

            return net;
        }

        private static ReadOnlyNetwork CreateBiphasicNexts(int last)
        {
            Network net = new();

            if (
                UnaryNeuronParameter.TryCreate(out var NEXT) &&
                UnaryNeuronParameter.TryCreate(out var currentStep, parameterExpression: "step0")
            )
            {
                BiphasicNext.InterneuronSet? previousInterneurons = null;
                for (int i = 1; i <= last; i++)
                {
                    if (
                        UnaryNeuronParameter.TryCreate(out var nextStep, parameterExpression: "step" + i) &&
                        currentStep.VariableInfo != null &&
                        BiphasicNext.TryCreate(
                            out BiphasicNext? n,
                            new(new(NEXT, currentStep), new(nextStep)),
                            0.5f,
                            previousInterneurons,
                            parameterExpression: 
                                $"{nameof(NEXT)}___{currentStep.VariableInfo.Inputs.Single()}"
                        )
                    )
                    {
                        net.AddReplaceItems(n);
                        currentStep = nextStep;
                        previousInterneurons = n.Interneurons;
                    }
                }
            }

            return net;
        }

        private static ReadOnlyNetwork CreateNexts(int last)
        {
            Network net = new();

            if (
                UnaryNeuronParameter.TryCreate(out var NEXT) &&
                UnaryNeuronParameter.TryCreate(out var currentStep, parameterExpression: "step0")
            )
            {
                Next.InterneuronSet? previousInterneurons = null;
                for (int i = 1; i <= last; i++)
                {
                    if (
                        UnaryNeuronParameter.TryCreate(out var nextStep, parameterExpression: "step" + i) &&
                        currentStep.VariableInfo != null &&
                        Next.TryCreate(
                            out Next? n,
                            new(new(NEXT, currentStep), new(nextStep)),
                            0.5f,
                            previousInterneurons,
                            $"{nameof(NEXT)}___{currentStep.VariableInfo.Inputs.Single()}"
                        )
                    )
                    {
                        net.AddReplaceItems(n);
                        currentStep = nextStep;
                        previousInterneurons = n.Interneurons;
                    }
                }
            }

            return net;
        }

        private static ReadOnlyNetwork CreateLogicGates()
        {
            Network net = new();

            var input1 = BinaryNeuronParameter.Create("Input1", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper());
            var input2 = BinaryNeuronParameter.Create("Input2", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper());

            if (
                BinaryNeuronParameter.TryCreate(out var result, trueString: Boolean.TrueString.ToUpper(), falseString: Boolean.FalseString.ToUpper()) && // rotateConfig);
                NetworkHelper.TryCreateNeuron(out var NOT) &&
                NetworkHelper.TryCreateNeuron(out var AND) &&
                NetworkHelper.TryCreateNeuron(out var OR) &&
                NetworkHelper.TryCreateNeuron(out var NAND) &&
                NetworkHelper.TryCreateNeuron(out var NOR) &&
                NetworkHelper.TryCreateNeuron(out var XOR) &&
                NetworkHelper.TryCreateNeuron(out var XNOR) &&
                NetworkHelper.TryCreateNeuron(out var IMPLY) &&
                NetworkHelper.TryCreateNeuron(out var NIMPLY) &&
                NotGate.TryCreate(
                    out NotGate? NOT___Input1,
                    new(new(input1), new(result)),
                    additionalInputs: NOT
                ) &&
                NotGate.TryCreate(
                    out NotGate? NOT___Input2,
                    new(new(input2), new(result)),
                    additionalInputs: NOT
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out AndGate? AND___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: AND
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out OrGate? OR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: OR
                    ) &&
                DualInputLogicGateBase.TryCreate(
                    out NandGate? NAND___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: NAND
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out NorGate? NOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: NOR
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out XorGate? XOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: XOR
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out XnorGate? XNOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: XNOR
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out ImplyGate? IMPLY___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: IMPLY
                ) &&
                DualInputLogicGateBase.TryCreate(
                    out NimplyGate? NIMPLY___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputs: NIMPLY
                )
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
                        input1,
                        input2
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
            BinaryNeuronParameter? precedingCarryOver = null;
            VariableInfo? precedingVariableInfo = null;
            for (int i = 0; i < count; i++)
            {
                if (Adder.TryCreate(out Adder? a, i, precedingCarryOver, precedingVariableInfo, nameof(Adder) + (i +1)))
                {
                    net.AddReplaceItems(a);
                    precedingCarryOver = a.Parameters.Outputs.CarryOver;
                    precedingVariableInfo = a.VariableInfo;
                }
            }
            return net;
        }

        private static ReadOnlyNetwork CreateDynamicAdder()
        {
            Network net = new();
            
            if 
            (
                BinaryNeuronParameter.TryCreate(out var precedingCarryOver) &&
                Adder.TryCreate(out Adder? a, 0, precedingCarryOver, null, nameof(Adder) + 1)
            )
            {
                net.AddReplaceItems(a);
            }

            return net;
        }

        private static ReadOnlyNetwork CreateSubtractors(int count)
        {
            Network net = new();
            BinaryNeuronParameter? precedingBorrow = null;
            VariableInfo? precedingVariableInfo = null;
            for (int i = 0; i < count; i++)
            {
                if (Subtractor.TryCreate(out Subtractor? s, i, precedingBorrow, precedingVariableInfo, nameof(Subtractor) + (i + 1)))
                {
                    net.AddReplaceItems(s);
                    precedingBorrow = s.Parameters.Outputs.Borrow;
                    precedingVariableInfo = s.VariableInfo;
                }
            }
            return net;
        }
    }
}
