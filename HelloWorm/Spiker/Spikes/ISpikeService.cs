using ei8.Prototypes.HelloWorm.Spiker.Neurons;

namespace ei8.Prototypes.HelloWorm.Spiker.Spikes
{
    public interface ISpikeService
    {
        void SetSpikeCount(int value);

        void Spike(IEnumerable<Guid> spikeTargets);
    }
}
