using ei8.Cortex.Coding;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Domain.Model;
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
                    newWorm.Initialize(queryResult.ToNetwork(), this.settingsService.Mirrors);
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
                // frmToolbox.CreateAdders(net);
                frmToolbox.CreateSubtractors(net);

                sheet.Initialize(net, this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        // TODO: private static void CreateLogicGates(Network net)
        //{
        //    var outputs = net.CreateBinaryNeurons("Result", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper()); // rotateConfig);

        //    var not1Interneurons = net.CreateInverterInterneurons(outputs);
        //    var not2Interneurons = net.CreateInverterInterneurons(outputs);
        //    var andInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.And, outputs);
        //    var orInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Or, outputs);
        //    var nandInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nand, outputs);
        //    var norInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nor, outputs);
        //    var xorInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Xor, outputs);
        //    var xnorInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Xnor, outputs);
        //    var implyInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Imply, outputs);
        //    var nimplyInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nimply, outputs);

        //    #region Input Neurons
        //    var inputs = net.CreateTruthTableInputNeurons(
        //        // input1TrueConfig,
        //        "Input 1",
        //        // input2TrueConfig,
        //        "Input 2",
        //        Boolean.TrueString.ToUpper(),
        //        Boolean.FalseString.ToUpper()
        //    );

        //    var not = net.CreateNeuron(
        //        "NOT"
        //    );

        //    var and = net.CreateNeuron(
        //        "AND"
        //    );

        //    var or = net.CreateNeuron(
        //        "OR"
        //    );

        //    var nand = net.CreateNeuron(
        //        "NAND"
        //    );

        //    var nor = net.CreateNeuron(
        //        "NOR"
        //    );

        //    var xor = net.CreateNeuron(
        //        "XOR"
        //    );

        //    var xnor = net.CreateNeuron(
        //        "XNOR"
        //    );

        //    var imply = net.CreateNeuron(
        //        "IMPLY"
        //    );

        //    var nimply = net.CreateNeuron(
        //        "NIMPLY"
        //    );
        //    #endregion

        //    #region Link Input Neurons to Interneurons
        //    // "Nothing is True, Everything is permitted"
        //    #region Not
        //    net.LinkInverterInputNeuronsToInterneurons(
        //        inputs.Input1,
        //        not1Interneurons,
        //        not
        //    );
        //    net.LinkInverterInputNeuronsToInterneurons(
        //        inputs.Input2,
        //        not2Interneurons,
        //        not
        //    );
        //    #endregion

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        andInterneurons,
        //        and
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        orInterneurons,
        //        or
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        nandInterneurons,
        //        nand
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        norInterneurons,
        //        nor
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        xorInterneurons,
        //        xor
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        xnorInterneurons,
        //        xnor
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        implyInterneurons,
        //        imply
        //    );

        //    net.LinkTruthTableInputNeuronsToInterneurons(
        //        inputs,
        //        nimplyInterneurons,
        //        nimply
        //    );
        //    #endregion
        //}

        // TODO: private static void CreateAdders(Network net)
        //{
        //    BinaryNeuronInfo? precedingCarryOver = null;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        frmToolbox.CreateAdder(net, out BinaryNeuronInfo carryOver, i, precedingCarryOver);
        //        precedingCarryOver = carryOver;
        //    }
        //}

        //private static void CreateAdder(
        //    Network net, 
        //    out BinaryNeuronInfo carryOver,
        //    int base2Exponent = 0,
        //    BinaryNeuronInfo? precedingCarryOver = null
        //)
        //{
        //    string adderName = $"Adder{base2Exponent + 1}";
        //    bool isLeastSignificantBit = precedingCarryOver == null;

        //    #region Output neurons
        //    var result = net.CreateBinaryNeurons($"{adderName}.Result");

        //    BinaryNeuronInfo? 
        //        half1_Xor_Result = null, 
        //        half1_CarryOver = null, 
        //        half2_CarryOver = null;

        //    if (!isLeastSignificantBit)
        //    {
        //        half1_Xor_Result = net.CreateBinaryNeurons($"{adderName}.Half1.XOR.Result");
        //        half1_CarryOver = net.CreateBinaryNeurons($"{adderName}.Half1.CarryOver");
        //        half2_CarryOver = net.CreateBinaryNeurons($"{adderName}.Half2.CarryOver");
        //    }

        //    carryOver = net.CreateBinaryNeurons($"{adderName}.CarryOver");
        //    #endregion

        //    #region Interneurons
        //    // half1
        //    var half1_Xor__Addend1__Addend2 = net.CreateTruthTableInterneurons(
        //        ExtensionMethods.LogicGateType.Xor,
        //        half1_Xor_Result ?? result,
        //        new TruthTableInterneuronTagInfo(
        //            $"{adderName}.Addend1",
        //            $"{adderName}.Addend2",
        //            $"{adderName}.Half1"
        //        )
        //    );
            
        //    var half1_And__Addend1__Addend2 = net.CreateTruthTableInterneurons(
        //        ExtensionMethods.LogicGateType.And,
        //        half1_CarryOver ?? carryOver,
        //        new TruthTableInterneuronTagInfo(
        //            $"{adderName}.Addend1",
        //            $"{adderName}.Addend2",
        //            $"{adderName}.Half1"
        //        )
        //    );

        //    TruthTableInterneuronInfo?
        //        half2_Xor__PrecedingCarryOver__half1_Xor_Result = null,
        //        half2_And__PrecedingCarryOver__half1_Xor_Result = null,
        //        or__Half1_CarryOver__Half2_CarryOver = null;

        //    if (
        //        !isLeastSignificantBit && 
        //        half2_CarryOver != null
        //    )
        //    {
        //        string precedingAdderName = $"Adder{base2Exponent}";

        //        // half2
        //        half2_Xor__PrecedingCarryOver__half1_Xor_Result = net.CreateTruthTableInterneurons(
        //            ExtensionMethods.LogicGateType.Xor,
        //            result,
        //            new TruthTableInterneuronTagInfo(
        //                $"{precedingAdderName}.CarryOver",
        //                $"{adderName}.Half1.XOR.Result",
        //                $"{adderName}.Half2"
        //            )
        //        );

        //        half2_And__PrecedingCarryOver__half1_Xor_Result = net.CreateTruthTableInterneurons(
        //            ExtensionMethods.LogicGateType.And,
        //            half2_CarryOver,
        //            new TruthTableInterneuronTagInfo(
        //                $"{precedingAdderName}.CarryOver",
        //                $"{adderName}.Half1.XOR.Result",
        //                $"{adderName}.Half2"
        //            )
        //        );
        //        // OR carryOvers
        //        or__Half1_CarryOver__Half2_CarryOver = net.CreateTruthTableInterneurons(
        //            ExtensionMethods.LogicGateType.Or,
        //            carryOver,
        //            new TruthTableInterneuronTagInfo(
        //                $"{adderName}.Half1.CarryOver",
        //                $"{adderName}.Half2.CarryOver",
        //                adderName
        //            )
        //        );
        //    }
        //    #endregion

        //    #region Input Neurons
        //    var addends = net.CreateTruthTableInputNeurons($"{adderName}.Addend1", $"{adderName}.Addend2");
        //    #endregion

        //    // Link Input Neurons to Half1 Interneuron
        //    net.LinkTruthTableInputNeuronsToInterneurons(addends, half1_Xor__Addend1__Addend2);
        //    net.LinkTruthTableInputNeuronsToInterneurons(addends, half1_And__Addend1__Addend2);

        //    if (
        //        !isLeastSignificantBit && 
        //        half2_Xor__PrecedingCarryOver__half1_Xor_Result != null && 
        //        precedingCarryOver != null &&
        //        half1_Xor_Result != null &&
        //        half2_And__PrecedingCarryOver__half1_Xor_Result != null &&
        //        or__Half1_CarryOver__Half2_CarryOver != null &&
        //        half1_CarryOver != null &&
        //        half2_CarryOver != null
        //    )
        //    {
        //        // Link Half1 interneurons and precedingCarryOver to Half2 interneurons
        //        net.LinkTruthTableInputNeuronsToInterneurons(
        //            new InputInfo(
        //                precedingCarryOver,
        //                half1_Xor_Result
        //            ),
        //            half2_Xor__PrecedingCarryOver__half1_Xor_Result
        //        );

        //        net.LinkTruthTableInputNeuronsToInterneurons(
        //            new InputInfo(
        //                precedingCarryOver,
        //                half1_Xor_Result
        //            ),
        //            half2_And__PrecedingCarryOver__half1_Xor_Result
        //        );

        //        // OR carryOvers
        //        net.LinkTruthTableInputNeuronsToInterneurons(
        //            new InputInfo(
        //                half1_CarryOver,
        //                half2_CarryOver
        //            ),
        //            or__Half1_CarryOver__Half2_CarryOver
        //        );
        //    }
        //}

        private static void CreateSubtractors(Network net)
        {
            BinaryNeuronInfo? precedingBorrow = null;
            for (int i = 0; i < 4; i++)
            {
                if (frmToolbox.TryCreateSubtractor(net, out BinaryNeuronInfo? borrow, i, precedingBorrow))
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
            bool boolResult = false;
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
                boolResult = true;

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
                    CreateHalf1Interneurons(
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
                    CreateHalf1Interneurons(
                        net,
                        operands,
                        result,
                        half1_OUT___Half1_NOT___Minuend,
                        borrow,
                        subtractorName
                    );
                }
            }

            return boolResult;
        }

        private static void CreateHalf1Interneurons(
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
