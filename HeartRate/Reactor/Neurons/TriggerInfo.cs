using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public struct TriggerInfo
    {
        public TriggerInfo(DateTime timestamp, NeurotransmitterEffect effect, float strength, string presynapticId)
        {
            this.Timestamp = timestamp;
            this.Effect = effect;
            this.Strength = strength;
            this.PresynapticId = presynapticId;
        }

        public readonly DateTime Timestamp;
        public readonly NeurotransmitterEffect Effect;
        public readonly float Strength;
        public readonly string PresynapticId;
    }
}
