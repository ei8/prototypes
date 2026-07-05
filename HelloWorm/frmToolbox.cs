using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;
using static SQLite.SQLite3;

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

                // TODO: frmToolbox.CreateLogicGates(net);
                frmToolbox.CreateAdder(net);

                sheet.Initialize(net, this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }

        private static void CreateLogicGates(Network net)
        {
            #region Output neurons
            var resultTrue = net.CreateNeuron("Result = TRUE"); // rotateConfig);
            var resultFalse = net.CreateNeuron("Result = FALSE");
            #endregion

            #region Interneurons
            net.CreateTruthTableInterneurons(
                resultTrue, resultFalse, resultTrue, resultFalse,
                out Neuron notInput1False,
                out Neuron notInput1True,
                out Neuron notInput2False,
                out Neuron notInput2True
            );

            net.CreateTruthTableInterneurons(
                resultFalse, resultFalse, resultFalse, resultTrue,
                out Neuron andInput1FalseInput2False,
                out Neuron andInput1TrueInput2False,
                out Neuron andInput1FalseInput2True,
                out Neuron andInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultFalse, resultTrue, resultTrue, resultTrue,
                out Neuron orInput1FalseInput2False,
                out Neuron orInput1TrueInput2False,
                out Neuron orInput1FalseInput2True,
                out Neuron orInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultTrue, resultTrue, resultTrue, resultFalse,
                out Neuron nandInput1FalseInput2False,
                out Neuron nandInput1TrueInput2False,
                out Neuron nandInput1FalseInput2True,
                out Neuron nandInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultTrue, resultFalse, resultFalse, resultFalse,
                out Neuron norInput1FalseInput2False,
                out Neuron norInput1TrueInput2False,
                out Neuron norInput1FalseInput2True,
                out Neuron norInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultFalse, resultTrue, resultTrue, resultFalse,
                out Neuron xorInput1FalseInput2False,
                out Neuron xorInput1TrueInput2False,
                out Neuron xorInput1FalseInput2True,
                out Neuron xorInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultTrue, resultFalse, resultFalse, resultTrue,
                out Neuron xnorInput1FalseInput2False,
                out Neuron xnorInput1TrueInput2False,
                out Neuron xnorInput1FalseInput2True,
                out Neuron xnorInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultTrue, resultFalse, resultTrue, resultTrue,
                out Neuron implyInput1FalseInput2False,
                out Neuron implyInput1TrueInput2False,
                out Neuron implyInput1FalseInput2True,
                out Neuron implyInput1TrueInput2True
            );

            net.CreateTruthTableInterneurons(
                resultFalse, resultTrue, resultFalse, resultFalse,
                out Neuron nimplyInput1FalseInput2False,
                out Neuron nimplyInput1TrueInput2False,
                out Neuron nimplyInput1FalseInput2True,
                out Neuron nimplyInput1TrueInput2True
            );
            #endregion

            #region Input Neurons
            var input1True = net.CreateNeuron(
                // input1TrueConfig,
                "Input 1 = TRUE"
            );

            var input1False = net.CreateNeuron(
                "Input 1 = FALSE"
            );

            var input2True = net.CreateNeuron(
                // input2TrueConfig,
                "Input 2 = TRUE"
            );

            var input2False = net.CreateNeuron(
                "Input 2 = FALSE"
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
            #region AND
            net.LinkInputNeuronsToInterneuron(
                notInput1False,
                not,
                input1False
            );
            net.LinkInputNeuronsToInterneuron(
                notInput1True,
                not,
                input1True
            );
            net.LinkInputNeuronsToInterneuron(
                notInput2False,
                not,
                input2False
            );
            net.LinkInputNeuronsToInterneuron(
                notInput2True,
                not,
                input2True
            );
            #endregion

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                and,
                andInput1FalseInput2False,
                andInput1TrueInput2False,
                andInput1FalseInput2True,
                andInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                or,
                orInput1FalseInput2False,
                orInput1TrueInput2False,
                orInput1FalseInput2True,
                orInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                nand,
                nandInput1FalseInput2False,
                nandInput1TrueInput2False,
                nandInput1FalseInput2True,
                nandInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                nor,
                norInput1FalseInput2False,
                norInput1TrueInput2False,
                norInput1FalseInput2True,
                norInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                xor,
                xorInput1FalseInput2False,
                xorInput1TrueInput2False,
                xorInput1FalseInput2True,
                xorInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                xnor,
                xnorInput1FalseInput2False,
                xnorInput1TrueInput2False,
                xnorInput1FalseInput2True,
                xnorInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                imply,
                implyInput1FalseInput2False,
                implyInput1TrueInput2False,
                implyInput1FalseInput2True,
                implyInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );

            net.LinkTruthTableInputNeuronsToInterneuronsTyped(
                nimply,
                nimplyInput1FalseInput2False,
                nimplyInput1TrueInput2False,
                nimplyInput1FalseInput2True,
                nimplyInput1TrueInput2True,
                input1True,
                input1False,
                input2True,
                input2False
            );
            #endregion
        }

        private static void CreateAdder(Network net)
        {
            // TODO: Unify CreateAdders
            frmToolbox.CreateAdderTwoPowerOfZero(net, out Neuron carryOver_1, out Neuron carryOver_0, true);
            frmToolbox.CreateAdder(net, 1, carryOver_1, carryOver_0, out Neuron carryOver2_1, out Neuron carryOver2_0, true);
            frmToolbox.CreateAdder(net, 2, carryOver2_1, carryOver2_0, out Neuron carryOver3_1, out Neuron carryOver3_0, true);
            frmToolbox.CreateAdder(net, 3, carryOver3_1, carryOver3_0, out Neuron carryOver4_1, out Neuron carryOver4_0, true);
        }

        private static void CreateAdderTwoPowerOfZero(Network net, out Neuron carryOver_1, out Neuron carryOver_0, bool clearOptionalTags = false)
        {
            // 2^0
            #region Output neurons
            var adder1_Result_1 = net.CreateNeuron("Adder1.Result = 1");
            var adder1_Result_0 = net.CreateNeuron("Adder1.Result = 0");

            carryOver_1 = net.CreateNeuron("Adder1.CarryOver = 1");
            carryOver_0 = net.CreateNeuron("Adder1.CarryOver = 0");
            #endregion

            #region Interneurons
            Neuron adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_0 = net.CreateInterneuron("Adder1.Addend1 = 0 Adder1.Half1.XOR Adder1.Addend2 = 0", adder1_Result_0);
            Neuron adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_0 = net.CreateInterneuron("Adder1.Addend1 = 1 Adder1.Half1.XOR Adder1.Addend2 = 0", adder1_Result_1);
            Neuron adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_1 = net.CreateInterneuron("Adder1.Addend1 = 0 Adder1.Half1.XOR Adder1.Addend2 = 1", adder1_Result_1);
            Neuron adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_1 = net.CreateInterneuron("Adder1.Addend1 = 1 Adder1.Half1.XOR Adder1.Addend2 = 1", adder1_Result_0);

            Neuron adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_0 = net.CreateInterneuron("Adder1.Addend1 = 0 Adder1.Half1.AND Adder1.Addend2 = 0", carryOver_0);
            Neuron adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_0 = net.CreateInterneuron("Adder1.Addend1 = 1 Adder1.Half1.AND Adder1.Addend2 = 0", carryOver_0);
            Neuron adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_1 = net.CreateInterneuron("Adder1.Addend1 = 0 Adder1.Half1.AND Adder1.Addend2 = 1", carryOver_0);
            Neuron adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_1 = net.CreateInterneuron("Adder1.Addend1 = 1 Adder1.Half1.AND Adder1.Addend2 = 1", carryOver_1);
            #endregion

            #region Input Neurons
            var adder1_Addend1_1 = net.CreateNeuron(
                "Adder1.Addend1 = 1"
            );

            var adder1_Addend1_0 = net.CreateNeuron(
                "Adder1.Addend1 = 0"
            );

            var adder1_Addend2_1 = net.CreateNeuron(
                "Adder1.Addend2 = 1"
            );

            var adder1_Addend2_0 = net.CreateNeuron(
                "Adder1.Addend2 = 0"
            );
            #endregion

            // Link Input Neurons to Interneurons
            net.LinkTruthTableInputNeuronsToInterneurons(
                adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_0,
                adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_0,
                adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_1,
                adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_1,
                adder1_Addend1_1,
                adder1_Addend1_0,
                adder1_Addend2_1,
                adder1_Addend2_0
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_0,
                adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_0,
                adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_1,
                adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_1,
                adder1_Addend1_1,
                adder1_Addend1_0,
                adder1_Addend2_1,
                adder1_Addend2_0
            );

            if (clearOptionalTags)
            {
                adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_0.Tag =
                adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_0.Tag =
                adder1_Half1_Xor__Adder1_Addend1_0__Adder1_Addend2_1.Tag =
                adder1_Half1_Xor__Adder1_Addend1_1__Adder1_Addend2_1.Tag =

                adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_0.Tag =
                adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_0.Tag =
                adder1_Half1_And__Adder1_Addend1_0__Adder1_Addend2_1.Tag =
                adder1_Half1_And__Adder1_Addend1_1__Adder1_Addend2_1.Tag = 
                
                string.Empty;
            }
        }

        private static void CreateAdder(
            Network net, 
            int base2Exponent, 
            Neuron precedingCarryOver_1, 
            Neuron precedingCarryOver_0, 
            out Neuron carryOver_1, 
            out Neuron carryOver_0, 
            bool clearOptionalTags = false
        )
        {
            string precedingAdderName = $"Adder{base2Exponent}";
            string adderName = $"Adder{base2Exponent + 1}";

            #region Output neurons
            var adder2_Result_1 = net.CreateNeuron($"{adderName}.Result = 1");
            var adder2_Result_0 = net.CreateNeuron($"{adderName}.Result = 0");

            var adder2_Half1_Xor_Result_1 = net.CreateNeuron($"{adderName}.Half1.XOR.Result = 1");
            var adder2_Half1_Xor_Result_0 = net.CreateNeuron($"{adderName}.Half1.XOR.Result = 0");
            var adder2_Half1_CarryOver_1 = net.CreateNeuron($"{adderName}.Half1.CarryOver = 1");
            var adder2_Half1_CarryOver_0 = net.CreateNeuron($"{adderName}.Half1.CarryOver = 0");
            var adder2_Half2_CarryOver_1 = net.CreateNeuron($"{adderName}.Half2.CarryOver = 1");
            var adder2_Half2_CarryOver_0 = net.CreateNeuron($"{adderName}.Half2.CarryOver = 0");

            carryOver_1 = net.CreateNeuron($"{adderName}.CarryOver = 1");
            carryOver_0 = net.CreateNeuron($"{adderName}.CarryOver = 0");
            #endregion

            #region Interneurons
            // half2
            Neuron adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 0 Adder2.Half2.XOR Adder2.Half1.XOR.Result = 0", adder2_Result_0);
            Neuron adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 1 Adder2.Half2.XOR Adder2.Half1.XOR.Result = 0", adder2_Result_1);
            Neuron adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 0 Adder2.Half2.XOR Adder2.Half1.XOR.Result = 1", adder2_Result_1);
            Neuron adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 1 Adder2.Half2.XOR Adder2.Half1.XOR.Result = 1", adder2_Result_0);

            Neuron adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 0 Adder2.Half2.AND Adder2.Half1.XOR.Result = 0", adder2_Half2_CarryOver_0);
            Neuron adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 1 Adder2.Half2.AND Adder2.Half1.XOR.Result = 0", adder2_Half2_CarryOver_0);
            Neuron adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 0 Adder2.Half2.AND Adder2.Half1.XOR.Result = 1", adder2_Half2_CarryOver_0);
            Neuron adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1 = net.CreateInterneuron($"{precedingAdderName}.CarryOver = 1 Adder2.Half2.AND Adder2.Half1.XOR.Result = 1", adder2_Half2_CarryOver_1);

            // half1
            Neuron adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_0 = net.CreateInterneuron($"{adderName}.Addend1 = 0 Adder2.Half1.XOR Adder2.Addend2 = 0", adder2_Half1_Xor_Result_0);
            Neuron adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_0 = net.CreateInterneuron($"{adderName}.Addend1 = 1 Adder2.Half1.XOR Adder2.Addend2 = 0", adder2_Half1_Xor_Result_1);
            Neuron adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_1 = net.CreateInterneuron($"{adderName}.Addend1 = 0 Adder2.Half1.XOR Adder2.Addend2 = 1", adder2_Half1_Xor_Result_1);
            Neuron adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_1 = net.CreateInterneuron($"{adderName}.Addend1 = 1 Adder2.Half1.XOR Adder2.Addend2 = 1", adder2_Half1_Xor_Result_0);

            Neuron adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_0 = net.CreateInterneuron($"{adderName}.Addend1 = 0 Adder2.Half1.AND Adder2.Addend2 = 0", adder2_Half1_CarryOver_0);
            Neuron adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_0 = net.CreateInterneuron($"{adderName}.Addend1 = 1 Adder2.Half1.AND Adder2.Addend2 = 0", adder2_Half1_CarryOver_0);
            Neuron adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_1 = net.CreateInterneuron($"{adderName}.Addend1 = 0 Adder2.Half1.AND Adder2.Addend2 = 1", adder2_Half1_CarryOver_0);
            Neuron adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_1 = net.CreateInterneuron($"{adderName}.Addend1 = 1 Adder2.Half1.AND Adder2.Addend2 = 1", adder2_Half1_CarryOver_1);

            // OR carryOvers
            Neuron adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_0 = net.CreateInterneuron($"{adderName}.Half1.CarryOver = 0 Adder2.OR Adder2.Half2.CarryOver = 0", carryOver_0);
            Neuron adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_0 = net.CreateInterneuron($"{adderName}.Half1.CarryOver = 1 Adder2.OR Adder2.Half2.CarryOver = 0", carryOver_1);
            Neuron adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_1 = net.CreateInterneuron($"{adderName}.Half1.CarryOver = 0 Adder2.OR Adder2.Half2.CarryOver = 1", carryOver_1);
            Neuron adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_1 = net.CreateInterneuron($"{adderName}.Half1.CarryOver = 1 Adder2.OR Adder2.Half2.CarryOver = 1", carryOver_1);
            #endregion

            #region Input Neurons
            var adder2_Addend1_1 = net.CreateNeuron(
                $"{adderName}.Addend1 = 1"
            );

            var adder2_Addend1_0 = net.CreateNeuron(
                $"{adderName}.Addend1 = 0"
            );

            var adder2_Addend2_1 = net.CreateNeuron(
                $"{adderName}.Addend2 = 1"
            );

            var adder2_Addend2_0 = net.CreateNeuron(
                $"{adderName}.Addend2 = 0"
            );
            #endregion

            // Link Input Neurons to Half1 Interneurons
            net.LinkTruthTableInputNeuronsToInterneurons(
                adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_0,
                adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_0,
                adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_1,
                adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_1,
                adder2_Addend1_1,
                adder2_Addend1_0,
                adder2_Addend2_1,
                adder2_Addend2_0
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_0,
                adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_0,
                adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_1,
                adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_1,
                adder2_Addend1_1,
                adder2_Addend1_0,
                adder2_Addend2_1,
                adder2_Addend2_0
            );

            // Link Half1 interneurons and precedingCarryOver to Half2 interneurons
            net.LinkTruthTableInputNeuronsToInterneurons(
                adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0,
                adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0,
                adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1,
                adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1,
                precedingCarryOver_1,
                precedingCarryOver_0,
                adder2_Half1_Xor_Result_1,
                adder2_Half1_Xor_Result_0
            );

            net.LinkTruthTableInputNeuronsToInterneurons(
                adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0,
                adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0,
                adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1,
                adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1,
                precedingCarryOver_1,
                precedingCarryOver_0,
                adder2_Half1_Xor_Result_1,
                adder2_Half1_Xor_Result_0
            );

            // OR carryOvers
            net.LinkTruthTableInputNeuronsToInterneurons(
                adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_0,
                adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_0,
                adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_1,
                adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_1,
                adder2_Half1_CarryOver_1,
                adder2_Half1_CarryOver_0,
                adder2_Half2_CarryOver_1,
                adder2_Half2_CarryOver_0
            );

            if (clearOptionalTags)
            {
                adder2_Half1_Xor_Result_1.Tag = 
                adder2_Half1_Xor_Result_0.Tag =
                adder2_Half1_CarryOver_1.Tag =
                adder2_Half1_CarryOver_0.Tag =
                adder2_Half2_CarryOver_1.Tag =
                adder2_Half2_CarryOver_0.Tag =

                // half2
                adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0.Tag =
                adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0.Tag =
                adder2_Half2_Xor__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1.Tag =
                adder2_Half2_Xor__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1.Tag =

                adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_0.Tag =
                adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_0.Tag =
                adder2_Half2_And__PrecedingCarryOver_0__Adder2_Half1_Xor_Result_1.Tag =
                adder2_Half2_And__PrecedingCarryOver_1__Adder2_Half1_Xor_Result_1.Tag =

                // half1
                adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_0.Tag =
                adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_0.Tag =
                adder2_Half1_Xor__Adder2_Addend1_0__Adder2_Addend2_1.Tag =
                adder2_Half1_Xor__Adder2_Addend1_1__Adder2_Addend2_1.Tag =

                adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_0.Tag =
                adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_0.Tag =
                adder2_Half1_And__Adder2_Addend1_0__Adder2_Addend2_1.Tag =
                adder2_Half1_And__Adder2_Addend1_1__Adder2_Addend2_1.Tag =

                // OR carryOvers
                adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_0.Tag =
                adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_0.Tag =
                adder2_Or__Adder2_Half1_CarryOver_0__Adder2_Half2_CarryOver_1.Tag =
                adder2_Or__Adder2_Half1_CarryOver_1__Adder2_Half2_CarryOver_1.Tag =

                string.Empty;
            }
        }
    }
}
