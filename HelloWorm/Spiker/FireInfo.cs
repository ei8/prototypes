namespace ei8.Prototypes.HelloWorm.Spiker
{
    public struct FireInfo
    {
        public static readonly FireInfo Empty = new FireInfo(DateTime.MinValue, new TriggerInfo[0]);

        public FireInfo(DateTime timestamp, TriggerInfo[] triggers)
        {
            Timestamp = timestamp;
            Triggers = triggers;
        }

        public DateTime Timestamp { get; private set; }

        public TriggerInfo[] Triggers { get; private set; }

        public override bool Equals(object obj)
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
