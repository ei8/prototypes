using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public struct FireInfo
    {
        public static readonly FireInfo Empty = new FireInfo(DateTime.MinValue, new TriggerInfo[0]);

        public FireInfo(DateTime timestamp, TriggerInfo[] triggers)
        {
            this.Timestamp = timestamp;
            this.Triggers = triggers;
        }

        public DateTime Timestamp { get; private set; }

        public TriggerInfo[] Triggers { get; private set; }

        public override bool Equals(Object obj)
        {
            return obj is FireInfo && this == (FireInfo)obj;
        }
        public override int GetHashCode()
        {
            return this.Timestamp.GetHashCode();
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
