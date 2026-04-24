using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Cortex.Diary.Nucleus.Client.In;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Http;
using System.ComponentModel;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmMain : Form
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISelectionService selectionService;
        private readonly ISettingsService settingsService;

        public frmMain(IServiceProvider serviceProvider, ISelectionService selectionService, ISettingsService settingsService)
        {
            InitializeComponent();

            this.dockPanel1.Theme = new VS2015LightTheme();
            this.dockPanel1.ActiveContentChanged += this.DockPanel1_ActiveContentChanged;
            this.serviceProvider = serviceProvider;
            this.selectionService = selectionService;
            this.selectionService.SelectionChanging += this.SelectionService_SelectionChanging;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            this.settingsService = settingsService;

            var fp = this.serviceProvider.GetRequiredService<frmProperties>();
            fp.Show(this.dockPanel1, DockState.DockRight);

            var tb = this.serviceProvider.GetRequiredService<frmToolbox>();
            tb.Show(this.dockPanel1, DockState.DockLeft);
        }

        private void DockPanel1_ActiveContentChanged(object? sender, EventArgs e)
        {
            if (this.dockPanel1.ActiveContent is frmDish fw)
                this.selectionService.SetSelectedComponents(new[] { fw.Dish });
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish w)
            {
                w.Added += this.Dish_Added;
                w.Removed += this.Dish_Removed;
            }

            if (this.selectionService.PrimarySelection is INotifyPropertyChanged t)
            {
                t.PropertyChanged += this.T_PropertyChanged;
            }
            this.UpdateTemporalToolbar();
        }

        private void T_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Dish.IsPlaying):
                    this.UpdateTemporalToolbar();
                    break;
            }
        }

        private void UpdateTemporalToolbar()
        {
            if (this.selectionService.PrimarySelection is ITemporal t)
            {
                this.tsbTemporalPlay.Enabled = !t.IsPlaying;
                this.tsbTemporalPause.Enabled = t.IsPlaying;
            }
            else
            {
                this.tsbTemporalPlay.Enabled =
                    this.tsbTemporalPause.Enabled =
                    false;
            }
        }

        private void SelectionService_SelectionChanging(object? sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is Dish w)
            {
                w.Added -= this.Dish_Added;
                w.Removed -= this.Dish_Removed;
            }

            if (this.selectionService.PrimarySelection is INotifyPropertyChanged t)
            {
                t.PropertyChanged -= this.T_PropertyChanged;
            }
        }

        private void UpdateObjectCount(Dish? dish)
        {
            if (dish != null)
                this.tslblCount.Text = $"Object(s): {dish.Components.Count()}";
        }

        private void Dish_Removed(object? sender, EventArgs e) => this.UpdateObjectCount((Dish?)sender);

        private void Dish_Added(object? sender, EventArgs e) => this.UpdateObjectCount((Dish?)sender);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsbTemporalPlay_Click(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is ITemporal t)
                t.Play();
        }

        private void tsbTemporalPause_Click(object sender, EventArgs e)
        {
            if (this.selectionService.PrimarySelection is ITemporal t)
                t.Pause();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedComponents(null);
        }

        private async void mnuToolsInitializeAvatar_Click(object sender, EventArgs e)
        {
            string avatarUrl = InputBox.ShowDialog(this, "Avatar URL", "http://fibona.cc/worm1/av8r/");

            if (
                this.settingsService.Mirrors != null &&
                this.settingsService.Mirrors.TryGetByKey(typeof(Worm).ToMethodKeyString("Rotate", typeof(Worm.RotationDirection), typeof(Worm.RotationDegrees)), out MirrorConfig? rotateConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDirection.Clockwise.ToEnumKeyString(), out MirrorConfig? clockwiseConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDirection.CounterClockwise.ToEnumKeyString(), out MirrorConfig? counterClockwiseConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDegrees.Small.ToEnumKeyString(), out MirrorConfig? smallConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDegrees.Medium.ToEnumKeyString(), out MirrorConfig? mediumConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDegrees.Large.ToEnumKeyString(), out MirrorConfig? largeConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.RotationDegrees.ExtraLarge.ToEnumKeyString(), out MirrorConfig? extraLargeConfig) &&
                this.settingsService.Mirrors.TryGetByKey(typeof(Odor).ToKeyString(), out MirrorConfig? odorConfig) &&
                this.settingsService.Mirrors.TryGetByKey(typeof(Dish).ToKeyString(), out MirrorConfig? dishConfig) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector1.ToEnumKeyString(), out MirrorConfig? sector1Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector2.ToEnumKeyString(), out MirrorConfig? sector2Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector3.ToEnumKeyString(), out MirrorConfig? sector3Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector4.ToEnumKeyString(), out MirrorConfig? sector4Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector5.ToEnumKeyString(), out MirrorConfig? sector5Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector6.ToEnumKeyString(), out MirrorConfig? sector6Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector7.ToEnumKeyString(), out MirrorConfig? sector7Config) &&
                this.settingsService.Mirrors.TryGetByKey(Worm.SectorValues.Sector8.ToEnumKeyString(), out MirrorConfig? sector8Config)
            )
            {
                var ns = new Network();

                // ... Output neurons
                var rotateNeuron = ns.CreateNeuron(rotateConfig);
                var clockwiseNeuron = ns.CreateNeuron(clockwiseConfig);
                var counterClockwiseNeuron = ns.CreateNeuron(counterClockwiseConfig);
                var smallNeuron = ns.CreateNeuron(smallConfig);
                var mediumNeuron = ns.CreateNeuron(mediumConfig);
                var largeNeuron = ns.CreateNeuron(largeConfig);
                var extraLargeNeuron = ns.CreateNeuron(extraLargeConfig);

                // ... Interneurons
                var dishSector1Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, mediumNeuron);
                var dishSector2Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, smallNeuron);
                var dishSector7Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, smallNeuron);
                var dishSector8Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, mediumNeuron);

                var odorSector1Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, smallNeuron);
                var odorSector2Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, mediumNeuron);
                var odorSector3Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, largeNeuron);
                var odorSector4Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, extraLargeNeuron);
                var odorSector5Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, extraLargeNeuron);
                var odorSector6Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, largeNeuron);
                var odorSector7Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, mediumNeuron);
                var odorSector8Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, smallNeuron);

                // ... Input Neurons
                ns.CreateInputNeuron(
                    odorConfig,
                    0.5f,
                    odorSector1Neuron,
                    odorSector2Neuron,
                    odorSector3Neuron,
                    odorSector4Neuron,
                    odorSector5Neuron,
                    odorSector6Neuron,
                    odorSector7Neuron,
                    odorSector8Neuron
                );

                ns.CreateInputNeuron(
                    dishConfig,
                    0.5f,
                    dishSector1Neuron,
                    dishSector2Neuron,
                    dishSector7Neuron,
                    dishSector8Neuron
                );

                ns.CreateInputNeuron(
                    sector1Config,
                    0.5f,
                    odorSector1Neuron,
                    dishSector1Neuron
                );

                ns.CreateInputNeuron(
                    sector2Config,
                    0.5f,
                    odorSector2Neuron,
                    dishSector2Neuron
                );

                ns.CreateInputNeuron(
                    sector3Config,
                    0.5f,
                    odorSector3Neuron
                );

                ns.CreateInputNeuron(
                    sector4Config,
                    0.5f,
                    odorSector4Neuron
                );

                ns.CreateInputNeuron(
                    sector5Config,
                    0.5f,
                    odorSector5Neuron
                );

                ns.CreateInputNeuron(
                    sector6Config,
                    0.5f,
                    odorSector6Neuron
                );

                ns.CreateInputNeuron(
                    sector7Config,
                    0.5f,
                    odorSector7Neuron,
                    dishSector7Neuron
                );

                ns.CreateInputNeuron(
                    sector8Config,
                    0.5f,
                    odorSector8Neuron,
                    dishSector8Neuron
                );

                // Create neurons
                var rp = new RequestProvider();
                rp.SetHttpClientHandler(new HttpClientHandler());
                var neuronClient = new HttpNeuronClient(rp);

                foreach (var n in ns.GetItems<Neuron>())
                    await neuronClient.CreateNeuron(
                        avatarUrl,
                        n.Id.ToString(),
                        n.Tag,
                        null,
                        n.MirrorUrl,
                        "bearerToken"
                    );

                var terminalClient = new HttpTerminalClient(rp);
                foreach (var t in ns.GetItems<Terminal>())
                    await terminalClient.CreateTerminal(
                        avatarUrl,
                        t.Id.ToString(),
                        t.PresynapticNeuronId.ToString(),
                        t.PostsynapticNeuronId.ToString(),
                        Enum.Parse<neurUL.Cortex.Common.NeurotransmitterEffect>(t.Effect.ToString()),
                        t.Strength,
                        null,
                        "bearerToken"
                    );
            }
        }

        private void mnuProjectSettings_Click(object sender, EventArgs e)
        {
            this.selectionService.SetSelectedComponents(new[] { this.settingsService });
        }

        private void mnuProjectAddDish_Click(object sender, EventArgs e)
        {
            var fp = this.serviceProvider.GetRequiredService<frmDish>();
            for (int i = 1; i < int.MaxValue; i++)
            {
                string name = $"Dish{i.ToString()}";
                if (!this.dockPanel1.Contents.OfType<frmDish>().Any(fd => fd.Dish.Name == name))
                {
                    fp.Dish.Name = name;
                    break;
                }
            }
            fp.Show(this.dockPanel1, DockState.Document);
        }
    }
}
