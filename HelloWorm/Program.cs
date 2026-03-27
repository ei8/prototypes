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
            world.Add(new Worm().Create(world.Size));
            // TODO: world.Add(new Worm().Create(world.Size));

            // TODO: NeuronCollection
            var ns = new NeuronCollection
            { 
                // Primitives
                new Neuron(
                    Constants.NeuronId.Odor,
                    nameof(Constants.NeuronId.Odor),
                    new Terminal(Constants.NeuronId.OdorSector1, NeurotransmitterEffect.Excite, 0.5f)
                    ),
                new Neuron(
                    Constants.NeuronId.Sector1,
                    nameof(Constants.NeuronId.Sector1),
                    new Terminal(Constants.NeuronId.OdorSector1, NeurotransmitterEffect.Excite, 0.5f)
                    ),
                new Neuron(Constants.NeuronId.Sector8, nameof(Constants.NeuronId.Sector8)),
                // Interneurons
                new Neuron(
                    Constants.NeuronId.OdorSector1,
                    nameof(Constants.NeuronId.OdorSector1),
                    new Terminal(Constants.NeuronId.Rotate, NeurotransmitterEffect.Excite, 1f),
                    new Terminal(Constants.NeuronId.Clockwise, NeurotransmitterEffect.Excite, 1f),
                    new Terminal(Constants.NeuronId.Degrees22_5, NeurotransmitterEffect.Excite, 1f)
                ),
                // Output neurons
                new Neuron(Constants.NeuronId.Rotate, nameof(Constants.NeuronId.Rotate)),
                new Neuron(Constants.NeuronId.Clockwise, nameof(Constants.NeuronId.Clockwise)),
                new Neuron(Constants.NeuronId.CounterClockwise, nameof(Constants.NeuronId.CounterClockwise)),
                new Neuron(Constants.NeuronId.Degrees22_5, nameof(Constants.NeuronId.Degrees22_5))
            };

            // assign to all worms
            foreach (var w in world.Components)
                if (w is INeurULized n)
                    n.Neurons = ns;

            Application.Run(new Form1(world));
        }
    }
}