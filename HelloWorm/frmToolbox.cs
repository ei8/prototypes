using ei8.Cortex.Coding;
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                net.LinkTruthTableInputNeuronsToInterneurons(
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

                sheet.Initialize(net, this.settingsService.Mirrors);
                dish.Add(sheet);
            }
        }
    }
}
