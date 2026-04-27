using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public interface ISpikeService
    {
        event EventHandler<TriggeredEventArgs>? Triggered;

        event EventHandler<FiredEventArgs>? Fired;

        void SetSpikeCount(int value);

        void Spike(
            IEnumerable<Guid> spikeTargets, 
            ISpikable spikable
        );
    }
}
