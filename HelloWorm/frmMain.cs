using ei8.Cortex.Coding;
using ei8.Cortex.Library.Common;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Http;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmMain : Form
    {
        private readonly Settings settings;
        private readonly IServiceProvider serviceProvider;
        private readonly ISelectionService selectionService;

        public frmMain(IServiceProvider serviceProvider, ISelectionService selectionService)
        {
            InitializeComponent();

            this.settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText("customSettings.json"))!;

            this.dockPanel1.Theme = new VS2015DarkTheme();
            this.dockPanel1.ActiveContentChanged += this.DockPanel1_ActiveContentChanged;
            this.serviceProvider = serviceProvider;
            this.selectionService = selectionService;
            this.selectionService.SelectionChanging += this.SelectionService_SelectionChanging;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;

            var fp = this.serviceProvider.GetRequiredService<frmProperties>();
            fp.Show(this.dockPanel1, DockState.DockRight);
        }

        private void DockPanel1_ActiveContentChanged(object? sender, EventArgs e)
        {
            if (this.dockPanel1.ActiveContent is frmWorld fw)
                this.selectionService.SetSelectedComponents(new[] { fw.World });
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is World w)
            {
                w.Added += this.World_Added;
                w.Removed += this.World_Removed;
            }
        }

        private void SelectionService_SelectionChanging(object? sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is World w)
            {
                w.Added -= this.World_Added;
                w.Removed -= this.World_Removed;
            }
        }

        private void UpdateObjectCount(World? world)
        {
            if (!this.Disposing && world is World w)
                this.Invoke(() => this.tslblCount.Text = $"Object(s): {w.Components.Count()}");
        }

        private void World_Removed(object? sender, EventArgs e) => this.UpdateObjectCount((World?) sender);

        private void World_Added(object? sender, EventArgs e) => this.UpdateObjectCount((World?) sender);

        private void mnuFileOpenAvatar_Click(object sender, EventArgs e)
        {
            var fp = this.serviceProvider.GetRequiredService<frmWorld>();
            fp.Show(this.dockPanel1, DockState.Document);
        }

        private void mnuObjectsCreateFood_Click(object sender, EventArgs e)
        {
            var fw = this.dockPanel1.ActiveContent as frmWorld;
            if (fw != null && fw.World!= null)
                fw.World.Add(new Food().Create(fw.World.Size));
        }

        private async void mnuObjectsCreateWorm_Click(object sender, EventArgs e)
        {
            string avatarUrl = InputBox.ShowDialog(this, "Avatar URL", "http://fibona.cc/worm1/av8r/");

            var fw = this.dockPanel1.ActiveContent as frmWorld;
            if (
                fw != null &&
                fw.World != null &&
                settings.Mirrors != null &&
                !string.IsNullOrEmpty(avatarUrl)
            )
            {
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

                var worm = (Worm)new Worm().Create(fw.World.Size);
                worm.Initialize(queryResult.ToNetwork(), settings.Mirrors);
                fw.World.Add(worm);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
