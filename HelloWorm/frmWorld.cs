using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using neurUL.Common.Http;
using Neuron = ei8.Cortex.Coding.Neuron;
using Terminal = ei8.Cortex.Coding.Terminal;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmWorld : Form
    {
        private readonly Settings settings;

        public frmWorld(World world)
        {
            InitializeComponent();

            this.worldPanel.World = world;
            this.mnuSettingsWorldRegenerate.Checked = this.worldPanel.World.Regenerate;

            this.worldPanel.World.Added += this.World_Added;
            this.worldPanel.World.Removed += this.World_Removed;

            this.settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText("customSettings.json"))!;
        }

        private void World_Removed(object? sender, EventArgs e)
        {
            this.UpdateCount();
        }

        private void World_Added(object? sender, EventArgs e)
        {
            this.UpdateCount();
        }

        private void UpdateCount()
        {
            if (this.worldPanel.World != null)
            {
                this.Invoke(() => this.tlblCount.Text = "Object(s): " + this.worldPanel.World.Components.Count());
            }
        }

        private void WorldPanel_DoubleClick(object? sender, EventArgs e)
        {
            if (this.worldPanel.World != null)
            {
                var f = this.worldPanel.World.Components.OfType<Food>().FirstOrDefault();
                if (f != null)
                    this.worldPanel.World.Remove(f);
                else
                    this.worldPanel.World.Add(new Food().Create(this.worldPanel.World.Size));
            }
        }

        private void frmWorld_Load(object sender, EventArgs e)
        {
            this.worldPanel.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.worldPanel.World != null)
                this.worldPanel.InvalidateRectangularComposite(this.worldPanel.World);
        }


        private void mnuObjectsCreateFood_Click(object sender, EventArgs e)
        {
            if (this.worldPanel.World != null)
                this.worldPanel.World.Add(new Food().Create(this.worldPanel.World.Size));
        }

        private void mnuSettingsWorldRegenerate_Click(object sender, EventArgs e)
        {
            this.mnuSettingsWorldRegenerate.Checked = !this.mnuSettingsWorldRegenerate.Checked;

            if (this.worldPanel.World != null)
                this.worldPanel.World.Regenerate = this.mnuSettingsWorldRegenerate.Checked;
        }

        private async void mnuObjectsCreateWorm_Click(object sender, EventArgs e)
        {
            string avatarUrl = InputBox.ShowDialog(this, "Avatar URL", "http://fibona.cc/worm1/av8r/");

            if (
                this.worldPanel.World != null &&
                settings.Mirrors != null &&
                !string.IsNullOrEmpty(avatarUrl)
            ) {
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
               
                var worm = (Worm)new Worm().Create(this.worldPanel.World.Size);
                worm.Initialize(queryResult.ToNetwork(), settings.Mirrors);
                this.worldPanel.World.Add(worm);
            }
        }

        private async void mnuToolsInitializeAvatar_Click(object sender, EventArgs e)
        {
            string avatarUrl = InputBox.ShowDialog(this, "Avatar URL", "http://fibona.cc/worm1/av8r/");

            var settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(System.IO.File.ReadAllText("customSettings.json"));
            AssertionConcern.AssertStateTrue(settings != null && settings.Mirrors != null, "Mirror Configs required.");

            if (
                this.worldPanel.World != null &&
                settings != null &&
                settings.Mirrors != null &&
                settings.Mirrors.TryGetByKey(typeof(Worm).ToMethodKeyString("Rotate", typeof(Worm.RotationDirection), typeof(Worm.RotationDegrees)), out MirrorConfig? rotateConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDirection.Clockwise.ToEnumKeyString(), out MirrorConfig? clockwiseConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDirection.CounterClockwise.ToEnumKeyString(), out MirrorConfig? counterClockwiseConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDegrees.Small.ToEnumKeyString(), out MirrorConfig? smallConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDegrees.Medium.ToEnumKeyString(), out MirrorConfig? mediumConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDegrees.Large.ToEnumKeyString(), out MirrorConfig? largeConfig) &&
                settings.Mirrors.TryGetByKey(Worm.RotationDegrees.ExtraLarge.ToEnumKeyString(), out MirrorConfig? extraLargeConfig) &&
                settings.Mirrors.TryGetByKey(typeof(Odor).ToKeyString(), out MirrorConfig? odorConfig) &&
                settings.Mirrors.TryGetByKey(typeof(World).ToKeyString(), out MirrorConfig? worldConfig) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector1.ToEnumKeyString(), out MirrorConfig? sector1Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector2.ToEnumKeyString(), out MirrorConfig? sector2Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector3.ToEnumKeyString(), out MirrorConfig? sector3Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector4.ToEnumKeyString(), out MirrorConfig? sector4Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector5.ToEnumKeyString(), out MirrorConfig? sector5Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector6.ToEnumKeyString(), out MirrorConfig? sector6Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector7.ToEnumKeyString(), out MirrorConfig? sector7Config) &&
                settings.Mirrors.TryGetByKey(Worm.SectorValues.Sector8.ToEnumKeyString(), out MirrorConfig? sector8Config)
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
                var worldSector1Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, mediumNeuron);
                var worldSector2Neuron = ns.CreateRotationInterneuron(rotateNeuron, counterClockwiseNeuron, smallNeuron);
                var worldSector7Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, smallNeuron);
                var worldSector8Neuron = ns.CreateRotationInterneuron(rotateNeuron, clockwiseNeuron, mediumNeuron);

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
                    worldConfig,
                    0.5f,
                    worldSector1Neuron,
                    worldSector2Neuron,
                    worldSector7Neuron,
                    worldSector8Neuron
                );

                ns.CreateInputNeuron(
                    sector1Config,
                    0.5f,
                    odorSector1Neuron,
                    worldSector1Neuron
                );

                ns.CreateInputNeuron(
                    sector2Config,
                    0.5f,
                    odorSector2Neuron,
                    worldSector2Neuron
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
                    worldSector7Neuron
                );

                ns.CreateInputNeuron(
                    sector8Config,
                    0.5f,
                    odorSector8Neuron,
                    worldSector8Neuron
                );

                // Create neurons
                var rp = new RequestProvider();
                rp.SetHttpClientHandler(new HttpClientHandler());
                var neuronClient = new HttpNeuronClient(rp);

                foreach(var n in ns.GetItems<Neuron>())
                    await neuronClient.CreateNeuron(
                        avatarUrl,
                        n.Id.ToString(),
                        n.Tag,
                        null,
                        n.MirrorUrl,
                        "bearerToken"
                    );

                var terminalClient = new HttpTerminalClient(rp);
                foreach(var t in ns.GetItems<Terminal>())
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
    }
}
