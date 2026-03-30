using ei8.Prototypes.HelloWorm.Spiker.Neurons;
using System.Diagnostics;

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

            var world = new World();
            world.Location = new Point(0, 0);
            world.Size = new Size(600, 400);
            world.Add(new Food().Create(world.Size));
            world.Add(new Food().Create(world.Size));
            world.Add(new Worm().Create(world.Size));
            // TODO: world.Add(new Worm().Create(world.Size));

            // TODO: NeuronCollection
            var ns = new NeuronCollection
            { 
                // Primitives
                new Neuron(
                    Constants.NeuronId.Odor,
                    nameof(Constants.NeuronId.Odor),
                    new Terminal(Constants.NeuronId.OdorSector1, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector2, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector3, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector4, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector5, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector6, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector7, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.OdorSector8, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.World,
                    nameof(Constants.NeuronId.World),
                    new Terminal(Constants.NeuronId.WorldSector1, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector2, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector7, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector8, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector1,
                    nameof(Constants.NeuronId.Sector1),
                    new Terminal(Constants.NeuronId.OdorSector1, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector1, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector2,
                    nameof(Constants.NeuronId.Sector2),
                    new Terminal(Constants.NeuronId.OdorSector2, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector2, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector3,
                    nameof(Constants.NeuronId.Sector3),
                    new Terminal(Constants.NeuronId.OdorSector3, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector4,
                    nameof(Constants.NeuronId.Sector4),
                    new Terminal(Constants.NeuronId.OdorSector4, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector5,
                    nameof(Constants.NeuronId.Sector5),
                    new Terminal(Constants.NeuronId.OdorSector5, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector6,
                    nameof(Constants.NeuronId.Sector6),
                    new Terminal(Constants.NeuronId.OdorSector6, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector7,
                    nameof(Constants.NeuronId.Sector7),
                    new Terminal(Constants.NeuronId.OdorSector7, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector7, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                new Neuron(
                    Constants.NeuronId.Sector8, 
                    nameof(Constants.NeuronId.Sector8),
                    new Terminal(Constants.NeuronId.OdorSector8, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f),
                    new Terminal(Constants.NeuronId.WorldSector8, Constants.Spiker.NeurotransmitterEffect.Excite, 0.5f)
                ),
                // Interneurons
                new Neuron(
                    Constants.NeuronId.WorldSector1,
                    nameof(Constants.NeuronId.WorldSector1),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees45)
                ),
                new Neuron(
                    Constants.NeuronId.WorldSector2,
                    nameof(Constants.NeuronId.WorldSector2),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees22_5)
                ),
                new Neuron(
                    Constants.NeuronId.WorldSector7,
                    nameof(Constants.NeuronId.WorldSector7),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees22_5)
                ),
                new Neuron(
                    Constants.NeuronId.WorldSector8,
                    nameof(Constants.NeuronId.WorldSector8),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees45)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector1,
                    nameof(Constants.NeuronId.OdorSector1),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees22_5)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector2,
                    nameof(Constants.NeuronId.OdorSector2),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees45)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector3,
                    nameof(Constants.NeuronId.OdorSector3),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees60)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector4,
                    nameof(Constants.NeuronId.OdorSector4),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.Clockwise),
                    new Terminal(Constants.NeuronId.Degrees70)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector5,
                    nameof(Constants.NeuronId.OdorSector5),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees70)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector6,
                    nameof(Constants.NeuronId.OdorSector6),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees60)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector7,
                    nameof(Constants.NeuronId.OdorSector7),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees45)
                ),
                new Neuron(
                    Constants.NeuronId.OdorSector8,
                    nameof(Constants.NeuronId.OdorSector8),
                    new Terminal(Constants.NeuronId.Rotate),
                    new Terminal(Constants.NeuronId.CounterClockwise),
                    new Terminal(Constants.NeuronId.Degrees22_5)
                ),
                // Output neurons
                new Neuron(Constants.NeuronId.Rotate, nameof(Constants.NeuronId.Rotate)),
                new Neuron(Constants.NeuronId.Clockwise, nameof(Constants.NeuronId.Clockwise)),
                new Neuron(Constants.NeuronId.CounterClockwise, nameof(Constants.NeuronId.CounterClockwise)),
                new Neuron(Constants.NeuronId.Degrees22_5, nameof(Constants.NeuronId.Degrees22_5)),
                new Neuron(Constants.NeuronId.Degrees45, nameof(Constants.NeuronId.Degrees45)),
                new Neuron(Constants.NeuronId.Degrees60, nameof(Constants.NeuronId.Degrees60)),
                new Neuron(Constants.NeuronId.Degrees70, nameof(Constants.NeuronId.Degrees70))
            };

            // assign to all worms
            foreach (var w in world.Components)
                if (w is INeurULized n)
                    n.Neurons = ns;

            Application.Run(new Form1(world));
        }
    }
}