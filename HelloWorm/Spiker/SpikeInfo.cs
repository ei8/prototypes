using ei8.Prototypes.HelloWorm.Spiker;
using System.Collections.Concurrent;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeInfo(Guid neuronId)
    {
        public Guid NeuronId { get; set; } = neuronId;
        public ConcurrentDictionary<DateTime, TriggerInfo> Triggers { get; } = new ConcurrentDictionary<DateTime, TriggerInfo>();
        public FireInfo? LastFire { get; set; }
    }
}
