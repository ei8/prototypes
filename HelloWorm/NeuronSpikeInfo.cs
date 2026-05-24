using static ei8.Prototypes.HelloWorm.NeuronSpikeInfo;

namespace ei8.Prototypes.HelloWorm
{
    public enum NeuronStatusValue
    {
        NotSet,
        Triggered,
        TriggeredPreviously,
        Fired,
        FiredPreviously
    }

    public class NeuronSpikeInfo(Guid id, NeuronStatusValue status, DateTime timestamp)
    {
        public Guid Id { get; } = id;
        public NeuronStatusValue Status { get; set; } = status;
        public DateTime Timestamp { get; set; } = timestamp;
    }
}
