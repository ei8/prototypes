using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public interface ISpikeService
    {
        event EventHandler<TriggeredEventArgs>? Triggered;

        event EventHandler<FiredEventArgs>? Fired;

        void SetSpikeCount(int value);

        void Spike(IEnumerable<Neuron> targets, Network network, TimeSpan refractoryPeriod);
    }
}
