using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class TriggeredEventArgs
    (
        Neuron target,
        ChargeInfo charge, 
        IEnumerable<FireInfo> reflexArc
    ) : EventArgs
    {
        public Neuron Target { get; } = target;
        public ChargeInfo Charge { get; } = charge;
        public IEnumerable<FireInfo> ReflexArc { get; } = reflexArc;
    }
}
