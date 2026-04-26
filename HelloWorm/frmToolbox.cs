using ei8.Cortex.Coding;
using ei8.Cortex.Library.Common;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Http;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmToolbox : DockContent
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISelectionService selectionService;
        private readonly ISettingsService settingsService;

        public frmToolbox(
            IServiceProvider serviceProvider, 
            ISelectionService selectionService, 
            ISettingsService settingsService
        )
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            this.settingsService = settingsService;
        }

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
                string avatarUrl = InputBox.ShowDialog(this, "Avatar URL", "http://fibona.cc/worm1/av8r/");

                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    // TODO: use serviceProvider to instantiate these
                    var rp = new RequestProvider();
                    rp.SetHttpClientHandler(new HttpClientHandler());
                    var client = new ei8.Cortex.Library.Client.Out.HttpNeuronQueryClient(rp);
                    var queryResult = await client.GetNeurons(
                        avatarUrl,
                        new NeuronQuery()
                        {
                            SortOrder = SortOrderValue.Descending,
                            SortBy = SortByValue.NeuronCreationTimestamp,
                            PageSize = 29,
                            Depth = 5,
                            DirectionValues = DirectionValues.Outbound
                        },
                        "Guest"
                    );

                    var newWorm = this.serviceProvider.GetRequiredService<Worm>();
                    newWorm.Initialize(d.Size);
                    newWorm.Network = queryResult.ToNetwork();
                    d.Add(newWorm);
                }
            }
        }
    }
}
