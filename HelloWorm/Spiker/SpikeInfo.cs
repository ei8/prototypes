using System.Collections.Concurrent;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeInfo()
    {
        public IDictionary<DateTime, TriggerInfo> Triggers { get; } = new ConcurrentDictionary<DateTime, TriggerInfo>();
        public FireInfo? LastFire { get; set; }
    }
}
