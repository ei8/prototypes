using ei8.Cortex.Coding;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Client.Out;
using ei8.Cortex.Library.Common;
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

        // TODO: necessary?
        //internal void FocusFirstItem()
        //{
        //    this.toolStrip1.Focus();
        //    if (this.toolStrip1.Items.Count > 0)
        //        this.toolStrip1.Items[0].Select();
        //}

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            bool isDish = this.selectionService.PrimarySelection is Dish;
            this.tsbFood.Visible =
                this.tsbWorm.Visible =
                isDish;

            this.tslblNoUsableControls.Visible = !isDish;
        }

        private void tsbFood_DoubleClick(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish d)
            {
                var newFood = this.serviceProvider.GetRequiredService<Food>();
                newFood.Initialize(d.Size);
                d.Add(newFood);
            }
        }

        private async void tsbWorm_DoubleClick(object sender, EventArgs e)
        {
            if (
                this.selectionService.PrimarySelection is Dish d &&
                this.settingsService.Mirrors != null
            )
            {
                string neuronQuery = InputBox.ShowDialog(this, "neurUL Query", "http://fibona.cc/worm1/av8r/cortex/neurons?sortorder=1&sortby=1&pagesize=29&depth=5&direction=1");
                if (!string.IsNullOrEmpty(neuronQuery) &&
                    neuronQuery.Contains('?') &&
                    QueryUrl.TryParse(neuronQuery, out QueryUrl queryUrl) &&
                    NeuronQuery.TryParse(queryUrl.QueryString, out NeuronQuery query)
                )
                {
                    var queryResult = await this.neuronQueryClient.GetNeurons(
                        queryUrl.AvatarUrl,
                        query,
                        "Guest"
                    );

                    var newWorm = this.serviceProvider.GetRequiredService<Worm>();
                    newWorm.Initialize(d.Size);
                    newWorm.Initialize(queryResult.ToNetwork(), this.settingsService.Mirrors);
                    d.Add(newWorm);
                }
            }
        }
    }
}
