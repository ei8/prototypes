using ei8.Cortex.Coding;

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
            // TODO: bool fe = File.Exists("customSettings.json");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var world = new World();
            world.Location = new Point(0, 0);
            world.Size = new Size(600, 400);
            world.Add(new Food().Create(world.Size));
            world.Add(new Food().Create(world.Size));
            world.Add(new Worm().Create(world.Size));
            // TODO: world.Add(new Worm().Create(world.Size));

            var ns = new Network();
            var nis = new INetworkItem[]
            { 
                // Primitives
                Helpers.CreateNeuron(Constants.NeuronId.Odor, nameof(Constants.NeuronId.Odor)),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector1, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector2, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector3, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector4, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector5, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector6, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector7, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Odor, Constants.NeuronId.OdorSector8, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.World, nameof(Constants.NeuronId.World)),
                Helpers.CreateTerminal(Constants.NeuronId.World,Constants.NeuronId.WorldSector1, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.World, Constants.NeuronId.WorldSector2, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.World, Constants.NeuronId.WorldSector7, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.World, Constants.NeuronId.WorldSector8, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector1, nameof(Constants.NeuronId.Sector1)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector1, Constants.NeuronId.OdorSector1, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Sector1, Constants.NeuronId.WorldSector1, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector2, nameof(Constants.NeuronId.Sector2)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector2, Constants.NeuronId.OdorSector2, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Sector2, Constants.NeuronId.WorldSector2, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector3, nameof(Constants.NeuronId.Sector3)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector3, Constants.NeuronId.OdorSector3, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector4, nameof(Constants.NeuronId.Sector4)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector4, Constants.NeuronId.OdorSector4, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector5, nameof(Constants.NeuronId.Sector5)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector5, Constants.NeuronId.OdorSector5, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector6, nameof(Constants.NeuronId.Sector6)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector6, Constants.NeuronId.OdorSector6, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector7, nameof(Constants.NeuronId.Sector7)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector7, Constants.NeuronId.OdorSector7, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Sector7, Constants.NeuronId.WorldSector7, NeurotransmitterEffect.Excite, 0.5f),

                Helpers.CreateNeuron(Constants.NeuronId.Sector8, nameof(Constants.NeuronId.Sector8)),
                Helpers.CreateTerminal(Constants.NeuronId.Sector8, Constants.NeuronId.OdorSector8, NeurotransmitterEffect.Excite, 0.5f),
                Helpers.CreateTerminal(Constants.NeuronId.Sector8, Constants.NeuronId.WorldSector8, NeurotransmitterEffect.Excite, 0.5f),
            
                // Interneurons
                Helpers.CreateNeuron(Constants.NeuronId.WorldSector1, nameof(Constants.NeuronId.WorldSector1)),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector1, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector1, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector1, Constants.NeuronId.Degrees45),

                Helpers.CreateNeuron(Constants.NeuronId.WorldSector2, nameof(Constants.NeuronId.WorldSector2)),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector2, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector2, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector2, Constants.NeuronId.Degrees22_5),

                Helpers.CreateNeuron(Constants.NeuronId.WorldSector7, nameof(Constants.NeuronId.WorldSector7)),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector7, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector7, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector7, Constants.NeuronId.Degrees22_5),

                Helpers.CreateNeuron(Constants.NeuronId.WorldSector8, nameof(Constants.NeuronId.WorldSector8)),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector8, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector8, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.WorldSector8, Constants.NeuronId.Degrees45),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector1, nameof(Constants.NeuronId.OdorSector1)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector1, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector1, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector1, Constants.NeuronId.Degrees22_5),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector2, nameof(Constants.NeuronId.OdorSector2)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector2, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector2, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector2, Constants.NeuronId.Degrees45),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector3, nameof(Constants.NeuronId.OdorSector3)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector3, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector3, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector3, Constants.NeuronId.Degrees60),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector4, nameof(Constants.NeuronId.OdorSector4)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector4, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector4, Constants.NeuronId.Clockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector4, Constants.NeuronId.Degrees70),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector5, nameof(Constants.NeuronId.OdorSector5)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector5, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector5, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector5, Constants.NeuronId.Degrees70),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector6, nameof(Constants.NeuronId.OdorSector6)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector6, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector6, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector6, Constants.NeuronId.Degrees60),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector7, nameof(Constants.NeuronId.OdorSector7)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector7, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector7, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector7, Constants.NeuronId.Degrees45),

                Helpers.CreateNeuron(Constants.NeuronId.OdorSector8, nameof(Constants.NeuronId.OdorSector8)),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector8, Constants.NeuronId.Rotate),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector8, Constants.NeuronId.CounterClockwise),
                Helpers.CreateTerminal(Constants.NeuronId.OdorSector8, Constants.NeuronId.Degrees22_5),

                // Output neurons
                Helpers.CreateNeuron(Constants.NeuronId.Rotate, nameof(Constants.NeuronId.Rotate)),
                Helpers.CreateNeuron(Constants.NeuronId.Clockwise, nameof(Constants.NeuronId.Clockwise)),
                Helpers.CreateNeuron(Constants.NeuronId.CounterClockwise, nameof(Constants.NeuronId.CounterClockwise)),
                Helpers.CreateNeuron(Constants.NeuronId.Degrees22_5, nameof(Constants.NeuronId.Degrees22_5)),
                Helpers.CreateNeuron(Constants.NeuronId.Degrees45, nameof(Constants.NeuronId.Degrees45)),
                Helpers.CreateNeuron(Constants.NeuronId.Degrees60, nameof(Constants.NeuronId.Degrees60)),
                Helpers.CreateNeuron(Constants.NeuronId.Degrees70, nameof(Constants.NeuronId.Degrees70))
            };
            nis.ToList().ForEach(ns.AddReplace);

            // assign to all worms
            foreach (var w in world.Components)
                if (w is INeurULized n)
                    n.Network = ns;

            Application.Run(new Form1(world));
        }
    }
}