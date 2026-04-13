using ei8.Cortex.Coding;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public struct TriggerInfo
    {
        public TriggerInfo(DateTime timestamp, NeurotransmitterEffect effect, float strength, Guid presynapticId)
        {
            Timestamp = timestamp;
            Effect = effect;
            Strength = strength;
            PresynapticId = presynapticId;
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
                    Timestamp == triggerInfo.Timestamp &&
                    Effect == triggerInfo.Effect &&
                    Strength == triggerInfo.Strength &&
                    PresynapticId == triggerInfo.PresynapticId;
            }
            return result;
        }
    }
}
