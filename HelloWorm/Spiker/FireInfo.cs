using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public struct FireInfo(Neuron target, DateTime timestamp, IEnumerable<TriggerInfo> triggers)
    {
        public Neuron Target { get; } = target;

        public DateTime Timestamp { get; } = timestamp;

        public IEnumerable<TriggerInfo> Triggers { get; } = triggers;

        public override bool Equals(object? obj)
        {
            return obj is FireInfo && this == (FireInfo)obj;
        }
        public override int GetHashCode()
        {
            return Timestamp.GetHashCode();
        }
        public static bool operator ==(FireInfo x, FireInfo y)
        {
            return x.Timestamp == y.Timestamp;
        }
        public static bool operator !=(FireInfo x, FireInfo y)
        {
            return !(x == y);
        }
    }
}
