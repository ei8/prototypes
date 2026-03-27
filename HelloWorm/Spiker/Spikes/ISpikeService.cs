using ei8.Prototypes.HelloWorm.Spiker.Neurons;

namespace HelloWorld.Spiker.Spikes
{
    public interface ISpikeService
    {
        event EventHandler Spiking;

        void SetSpikeCount(int value);

        void Spike(IEnumerable<SpikeTarget> spikeTargets, NeuronCollection neurons);
    }
}
