using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
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

                var net = new Network();

                // frmToolbox.CreateLogicGates(net);
                // frmToolbox.CreateAdders(net, 4);
                frmToolbox.CreateSubtractors(net, 4);

                sheet.Initialize(this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static void CreateLogicGates(Network net)
        {
            if (
                net.TryCreateBinaryNeurons(out var result, trueString: Boolean.TrueString.ToUpper(), falseString: Boolean.FalseString.ToUpper()) && // rotateConfig);
                net.TryCreateInverterInterneurons(out var NOT___Input1, result) &&
                net.TryCreateInverterInterneurons(out var NOT___Input2, result) &&
                net.TryCreateTruthTableInterneurons(out var AND___Input1__Input2, ExtensionMethods.LogicGateType.And, result) &&
                net.TryCreateTruthTableInterneurons(out var OR___Input1__Input2, ExtensionMethods.LogicGateType.Or, result) &&
                net.TryCreateTruthTableInterneurons(out var NAND___Input1__Input2, ExtensionMethods.LogicGateType.Nand, result) &&
                net.TryCreateTruthTableInterneurons(out var NOR___Input1__Input2, ExtensionMethods.LogicGateType.Nor, result) &&
                net.TryCreateTruthTableInterneurons(out var XOR___Input1__Input2, ExtensionMethods.LogicGateType.Xor, result) &&
                net.TryCreateTruthTableInterneurons(out var XNOR___Input1__Input2, ExtensionMethods.LogicGateType.Xnor, result) &&
                net.TryCreateTruthTableInterneurons(out var IMPLY___Input1__Input2, ExtensionMethods.LogicGateType.Imply, result) &&
                net.TryCreateTruthTableInterneurons(out var NIMPLY___Input1__Input2, ExtensionMethods.LogicGateType.Nimply, result) &&
                net.TryCreateNeuron(out var NOT) &&
                net.TryCreateNeuron(out var AND) &&
                net.TryCreateNeuron(out var OR) &&
                net.TryCreateNeuron(out var NAND) &&
                net.TryCreateNeuron(out var NOR) &&
                net.TryCreateNeuron(out var XOR) &&
                net.TryCreateNeuron(out var XNOR) &&
                net.TryCreateNeuron(out var IMPLY) &&
                net.TryCreateNeuron(out var NIMPLY)
            )
            { 
                var inputs = net.CreateTruthTableInputNeurons(
                    // input1TrueConfig,
                    "Input1",
                    // input2TrueConfig,
                    "Input2",
                    Boolean.TrueString.ToUpper(),
                    Boolean.FalseString.ToUpper()
                );
                
                #region Link Input Neurons to Interneurons
                // "Nothing is True, Everything is permitted"
                #region Not
                net.LinkInverterInputNeuronsToInterneurons(
                    inputs.Input1,
                    NOT___Input1,
                    NOT
                );
                net.LinkInverterInputNeuronsToInterneurons(
                    inputs.Input2,
                    NOT___Input2,
                    NOT
                );
                #endregion

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    AND___Input1__Input2,
                    AND
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    OR___Input1__Input2,
                    OR
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    NAND___Input1__Input2,
                    NAND
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    NOR___Input1__Input2,
                    NOR
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    XOR___Input1__Input2,
                    XOR
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    XNOR___Input1__Input2,
                    XNOR
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    IMPLY___Input1__Input2,
                    IMPLY
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    inputs,
                    NIMPLY___Input1__Input2,
                    NIMPLY
                );
                #endregion
            }
        }

        private static void CreateAdders(Network net, int count)
        {
            BinaryNeuronInfo? precedingCarryOver = null;
            for (int i = 0; i < count; i++)
            {
                if (frmToolbox.TryCreateAdder(net, out var carryOver, i, precedingCarryOver))
                    precedingCarryOver = carryOver;
            }
        }

        private static bool TryCreateAdder(
            Network net,
            out BinaryNeuronInfo? carryOver,
            int base2Exponent = 0,
            BinaryNeuronInfo? precedingCarryOver = null
        )
        {
            var boolResult = false;
            carryOver = null;
            if ( net == null )
            { 
                boolResult = true;
            }

            return boolResult;
        }

        private static void CreateSubtractors(Network net, int count)
        {
            BinaryNeuronInfo? precedingBorrow = null;
            for (int i = 0; i < count; i++)
            {
                if (frmToolbox.TryCreateSubtractor(net, out var borrow, i, precedingBorrow))
                    precedingBorrow = borrow;
            }
        }

        private static bool TryCreateSubtractor(
            Network net,
            [NotNullWhen(true)] out BinaryNeuronInfo? borrow,
            int base2Exponent = 0,
            BinaryNeuronInfo? precedingBorrow = null
        )
        {
            var boolResult = false;
            borrow = null;
            string subtractorName = $"Subtractor{base2Exponent + 1}";

            // Declare Inputs
            var operands = net.CreateTruthTableInputNeurons($"{subtractorName}.Minuend", $"{subtractorName}.Subtrahend");

            // Declare Outputs
            if (
                net.TryCreateBinaryNeurons(out var result, subtractorName) &&
                net.TryCreateBinaryNeurons(out var half1_OUT___Half1_NOT___Minuend, subtractorName) && 
                net.TryCreateBinaryNeurons(out borrow, subtractorName) 
            )
            {
                if (
                    // is not least significant bit
                    precedingBorrow != null &&
                    net.TryCreateBinaryNeurons(out var half1_XOR_Result, subtractorName) && 
                    net.TryCreateBinaryNeurons(out var half2_OUT___Half2_NOT___Half1_XOR_Result, subtractorName) && 
                    net.TryCreateBinaryNeurons(out var half1_Borrow, subtractorName) && 
                    net.TryCreateBinaryNeurons(out var half2_Borrow, subtractorName) 
                )
                {
                    string precedingSubtractorName = $"Subtractor{base2Exponent}";

                    // half1 interneurons
                    CreateSubtractorHalf1Interneurons(
                        net,
                        operands,
                        half1_XOR_Result,
                        half1_OUT___Half1_NOT___Minuend,
                        half1_Borrow,
                        subtractorName
                    );

                    // half2 interneurons
                    if (
                        net.TryCreateTruthTableInterneurons(
                            out var half2_XOR___Borrow__Half1_XOR_Result,
                            ExtensionMethods.LogicGateType.Xor,
                            result,
                            new (
                                precedingSubtractorName,
                                subtractorName,
                                subtractorName
                            )
                        ) &&
                        net.TryCreateInverterInterneurons(
                            out var half2_NOT___Half1_XOR_Result,
                            half2_OUT___Half2_NOT___Half1_XOR_Result,
                            subtractorName,
                            subtractorName
                        ) &&
                        net.TryCreateTruthTableInterneurons(
                            out var half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result,
                            ExtensionMethods.LogicGateType.And,
                            half2_Borrow,
                            new (
                                precedingSubtractorName,
                                subtractorName,
                                subtractorName
                            )
                        ) &&
                        // OR Borrows
                        net.TryCreateTruthTableInterneurons(
                            out var OR___Half1_Borrow__Half2_Borrow,
                            ExtensionMethods.LogicGateType.Or,
                            borrow,
                            new (subtractorName)
                        )
                    )
                    {
                        // Link Half1 interneurons and precedingBorrow to Half2 interneurons
                        net.LinkTruthTableInputNeuronsToInterneurons(
                            new (
                                precedingBorrow,
                                half1_XOR_Result
                            ),
                            half2_XOR___Borrow__Half1_XOR_Result
                        );

                        net.LinkInverterInputNeuronsToInterneurons(
                            half1_XOR_Result,
                            half2_NOT___Half1_XOR_Result
                        );

                        net.LinkTruthTableInputNeuronsToInterneurons(
                            new (
                                precedingBorrow,
                                half2_OUT___Half2_NOT___Half1_XOR_Result
                            ),
                            half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result
                        );

                        // OR Borrows
                        net.LinkTruthTableInputNeuronsToInterneurons(
                            new (
                                half1_Borrow,
                                half2_Borrow
                            ),
                            OR___Half1_Borrow__Half2_Borrow
                        );
                    }
                }
                else
                {
                    // half1
                    CreateSubtractorHalf1Interneurons(
                        net,
                        operands,
                        result,
                        half1_OUT___Half1_NOT___Minuend,
                        borrow,
                        subtractorName
                    );
                }

                boolResult = true;
            }

            return boolResult;
        }

        private static void CreateSubtractorHalf1Interneurons(
            Network net,
            InputInfo operands,
            BinaryNeuronInfo xorOutput,
            BinaryNeuronInfo notOutput,
            BinaryNeuronInfo andOutput,
            string prefix
        )
        {
            // Link half1 interneurons
            if (
                net.TryCreateTruthTableInterneurons(
                    out var half1_XOR___Minuend__Subtrahend,
                    ExtensionMethods.LogicGateType.Xor,
                    xorOutput,
                    new (prefix)
                ) &&
                net.TryCreateInverterInterneurons(
                    out var half1_NOT___Minuend,
                    notOutput,
                    prefix,
                    prefix
                ) &&
                net.TryCreateTruthTableInterneurons(
                    out var half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend,
                    ExtensionMethods.LogicGateType.And,
                    andOutput,
                    new (prefix)
                )
            )
            {
                // Input Neurons to Interneurons
                net.LinkTruthTableInputNeuronsToInterneurons(operands, half1_XOR___Minuend__Subtrahend);
                net.LinkInverterInputNeuronsToInterneurons(operands.Input1, half1_NOT___Minuend);

                // Intermediate Results to Interneurons
                net.LinkTruthTableInputNeuronsToInterneurons(
                    new InputInfo(
                        notOutput,
                        operands.Input2
                    ),
                    half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend
                );
            }
        }
    }
}
