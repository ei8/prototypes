using ei8.Cortex.Coding;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Domain.Model;
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
                frmToolbox.CreateAdders(net);

                sheet.Initialize(net, this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static void CreateLogicGates(Network net)
        {
            var outputs = net.CreateBinaryNeurons("Result", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper()); // rotateConfig);

            var notInterneurons = net.CreateTruthTableInterneurons(outputs.Neuron1, outputs.Neuron0, outputs.Neuron1, outputs.Neuron0, new OutputInterneuronTagInfo());
            var andInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.And, outputs);
            var orInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Or, outputs);
            var nandInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nand, outputs);
            var norInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nor, outputs);
            var xorInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Xor, outputs);
            var xnorInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Xnor, outputs);
            var implyInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Imply, outputs);
            var nimplyInterneurons = net.CreateTruthTableInterneurons(ExtensionMethods.LogicGateType.Nimply, outputs);

            #region Input Neurons
            var inputs = net.CreateTruthTableInputNeurons(
                // input1TrueConfig,
                "Input 1",
                // input2TrueConfig,
                "Input 2",
                Boolean.TrueString.ToUpper(),
                Boolean.FalseString.ToUpper()
            );

            var not = net.CreateNeuron(
                "NOT"
            );

            var and = net.CreateNeuron(
                "AND"
            );

            var or = net.CreateNeuron(
                "OR"
            );

            var nand = net.CreateNeuron(
                "NAND"
            );

            var nor = net.CreateNeuron(
                "NOR"
            );

            var xor = net.CreateNeuron(
                "XOR"
            );

            var xnor = net.CreateNeuron(
                "XNOR"
            );

            var imply = net.CreateNeuron(
                "IMPLY"
            );

            var nimply = net.CreateNeuron(
                "NIMPLY"
            );
            #endregion

            #region Link Input Neurons to Interneurons
            // "Nothing is True, Everything is permitted"
            #region Not
            net.LinkInputNeuronsToInterneuron(
                notInterneurons.Interneuron1,
                not,
                inputs.Input1.Neuron0
            );
            net.LinkInputNeuronsToInterneuron(
                notInterneurons.Interneuron2,
                not,
                inputs.Input1.Neuron1
            );
            net.LinkInputNeuronsToInterneuron(
                notInterneurons.Interneuron3,
                not,
                inputs.Input2.Neuron0
            );
            net.LinkInputNeuronsToInterneuron(
                notInterneurons.Interneuron4,
                not,
                inputs.Input2.Neuron1
            );
            #endregion

            net.LinkTruthTableInputNeuronsToInterneurons(
                andInterneurons,
                inputs,
                and
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                orInterneurons,
                inputs,
                or
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                nandInterneurons,
                inputs,
                nand
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                norInterneurons,
                inputs,
                nor
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                xorInterneurons,
                inputs,
                xor
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                xnorInterneurons,
                inputs,
                xnor
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                implyInterneurons,
                inputs,
                imply
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                nimplyInterneurons,
                inputs,
                nimply
            );
            #endregion
        }

        private static void CreateAdders(Network net)
        {
            frmToolbox.CreateAdder(net, out BinaryNeuronInfo carryOver1);
            frmToolbox.CreateAdder(net, out BinaryNeuronInfo carryOver2, 1, carryOver1);
            frmToolbox.CreateAdder(net, out BinaryNeuronInfo carryOver3, 2, carryOver2);
            frmToolbox.CreateAdder(net, out BinaryNeuronInfo carryOver4, 3, carryOver3);
        }

        private static void CreateAdder(
            Network net, 
            out BinaryNeuronInfo carryOver,
            int base2Exponent = 0,
            BinaryNeuronInfo? precedingCarryOver = null
        )
        {
            string adderName = $"Adder{base2Exponent + 1}";
            bool isFirstAdder = precedingCarryOver == null;

            #region Output neurons
            var result = net.CreateBinaryNeurons($"{adderName}.Result");

            BinaryNeuronInfo? 
                half1_Xor_Result = null, 
                half1_CarryOver = null, 
                half2_CarryOver = null;

            if (!isFirstAdder)
            {
                half1_Xor_Result = net.CreateBinaryNeurons($"{adderName}.Half1.XOR.Result");
                half1_CarryOver = net.CreateBinaryNeurons($"{adderName}.Half1.CarryOver");
                half2_CarryOver = net.CreateBinaryNeurons($"{adderName}.Half2.CarryOver");
            }

            carryOver = net.CreateBinaryNeurons($"{adderName}.CarryOver");
            #endregion

            #region Interneurons
            // half1
            var half1_Xor__Addend1__Addend2 = net.CreateTruthTableInterneurons(
                ExtensionMethods.LogicGateType.Xor,
                half1_Xor_Result ?? result,
                new TruthTableInterneuronTagInfo(
                    $"{adderName}.Addend1",
                    $"{adderName}.Addend2",
                    $"{adderName}.Half1"
                )
            );
            
            var half1_And__Addend1__Addend2 = net.CreateTruthTableInterneurons(
                ExtensionMethods.LogicGateType.And,
                half1_CarryOver ?? carryOver,
                new TruthTableInterneuronTagInfo(
                    $"{adderName}.Addend1",
                    $"{adderName}.Addend2",
                    $"{adderName}.Half1"
                )
            );

            TruthTableInterneuronInfo?
                half2_Xor__PrecedingCarryOver__half1_Xor_Result = null,
                half2_And__PrecedingCarryOver__half1_Xor_Result = null,
                or__Half1_CarryOver__Half2_CarryOver = null;

            if (
                !isFirstAdder && 
                half2_CarryOver != null
            )
            {
                string precedingAdderName = $"Adder{base2Exponent}";

                // half2
                half2_Xor__PrecedingCarryOver__half1_Xor_Result = net.CreateTruthTableInterneurons(
                    ExtensionMethods.LogicGateType.Xor,
                    result,
                    new TruthTableInterneuronTagInfo(
                        $"{precedingAdderName}.CarryOver",
                        $"{adderName}.Half1.XOR.Result",
                        $"{adderName}.Half2"
                    )
                );

                half2_And__PrecedingCarryOver__half1_Xor_Result = net.CreateTruthTableInterneurons(
                    ExtensionMethods.LogicGateType.And,
                    half2_CarryOver,
                    new TruthTableInterneuronTagInfo(
                        $"{precedingAdderName}.CarryOver",
                        $"{adderName}.Half1.XOR.Result",
                        $"{adderName}.Half2"
                    )
                );
                // OR carryOvers
                or__Half1_CarryOver__Half2_CarryOver = net.CreateTruthTableInterneurons(
                    ExtensionMethods.LogicGateType.Or,
                    carryOver,
                    new TruthTableInterneuronTagInfo(
                        $"{adderName}.Half1.CarryOver",
                        $"{adderName}.Half2.CarryOver",
                        adderName
                    )
                );
            }
            #endregion

            #region Input Neurons
            var addends = net.CreateTruthTableInputNeurons($"{adderName}.Addend1", $"{adderName}.Addend2");
            #endregion

            // Link Input Neurons to Half1 Interneuron
            net.LinkTruthTableInputNeuronsToInterneurons(half1_Xor__Addend1__Addend2, addends);
            net.LinkTruthTableInputNeuronsToInterneurons(half1_And__Addend1__Addend2, addends);

            if (
                !isFirstAdder && 
                half2_Xor__PrecedingCarryOver__half1_Xor_Result != null && 
                precedingCarryOver != null &&
                half1_Xor_Result != null &&
                half2_And__PrecedingCarryOver__half1_Xor_Result != null &&
                or__Half1_CarryOver__Half2_CarryOver != null &&
                half1_CarryOver != null &&
                half2_CarryOver != null
            )
            {
                // Link Half1 interneurons and precedingCarryOver to Half2 interneurons
                net.LinkTruthTableInputNeuronsToInterneurons(
                    half2_Xor__PrecedingCarryOver__half1_Xor_Result,
                    new InputInfo(
                        precedingCarryOver,
                        half1_Xor_Result
                    )
                );

                net.LinkTruthTableInputNeuronsToInterneurons(
                    half2_And__PrecedingCarryOver__half1_Xor_Result,
                    new InputInfo(
                        precedingCarryOver,
                        half1_Xor_Result
                    )
                );

                // OR carryOvers
                net.LinkTruthTableInputNeuronsToInterneurons(
                    or__Half1_CarryOver__Half2_CarryOver,
                    new InputInfo(
                        half1_CarryOver,
                        half2_CarryOver
                    )
                );
            }
        }
    }
}
