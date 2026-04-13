using ei8.Cortex.Coding;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public struct TriggerInfo
    {
        public TriggerInfo(DateTime timestamp, NeurotransmitterEffect effect, float strength, Guid presynapticId)
        {
            this.Timestamp = timestamp;
            this.Effect = effect;
            this.Strength = strength;
            this.PresynapticId = presynapticId;
        }

        public readonly DateTime Timestamp;
        public readonly NeurotransmitterEffect Effect;
        public readonly float Strength;
        public readonly Guid PresynapticId;

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            bool result = false;
            if (obj != null && obj is TriggerInfo triggerInfo)
            {
                result =
                    this.Timestamp == triggerInfo.Timestamp &&
                    this.Effect == triggerInfo.Effect &&
                    this.Strength == triggerInfo.Strength &&
                    this.PresynapticId == triggerInfo.PresynapticId;
            }
            return result;
        }
    }
}
