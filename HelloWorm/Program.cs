using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using HelloWorm;
using neurUL.Common.Domain.Model;

namespace ei8.Prototypes.HelloWorm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            #region Network retrieval
            var settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText("customSettings.json"));
            //var rp = new RequestProvider();
            //rp.SetHttpClientHandler(new HttpClientHandler());
            //var client = new ei8.Cortex.Library.Client.Out.HttpNeuronQueryClient(rp);
            //var queryResult = client.GetNeurons(
            //    "http://fibona.cc/worm1/av8r/",
            //    new NeuronQuery()
            //    {
            //        Depth = 5,
            //        DirectionValues = DirectionValues.Outbound
            //    },
            //    "Guest"
            //).Result;
            //var net = queryResult.ToNetwork();
            #endregion


            // TODO: Prepare data
            // Create neurons
            //var rp = new RequestProvider();
            //rp.SetHttpClientHandler(new HttpClientHandler());
            //var client = new HttpNeuronClient(rp);
            //client.CreateNeuron(
            //    "http://fibona.cc/worm1/av8r/",
            //    Guid.NewGuid().ToString(),
            //    "Hello worm",
            //    null,
            //    "http://fibona.cc/worm1/gogogo/",
            //    "bearerToken"
            //).Wait();

            // var ks = typeof(Worm).ToMethodKeyString("Rotate", typeof(Worm.RotationDirection), typeof(Worm.RotationDegrees));


            var world = new World();
            world.Location = new Point(0, 0);
            world.Size = new Size(600, 400);
            world.Add(new Food().Create(world.Size));
            world.Add(new Food().Create(world.Size));
            world.Add(new Worm().Create(world.Size));
            // TODO: world.Add(new Worm().Create(world.Size));

            var ns = new Network();

            AssertionConcern.AssertStateTrue(settings != null && settings.Mirrors != null, "Mirror Configs required.");

            if (
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

                // assign to all worms
                foreach (var w in world.Components)
                    if (w is INeurULized n)
                        n.Initialize(ns, settings.Mirrors);
            }

            Application.Run(new Form1(world));
        }
    }
}