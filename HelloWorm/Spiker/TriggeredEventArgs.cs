using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class TriggeredEventArgs
    (
        Neuron source,
        SpikeOrigin spikeOrigin, 
        TriggerInfo triggerInfo, 
        int charge, 
        IEnumerable<FireInfo> reflexArc
    ) : EventArgs
    {
        public Neuron Source { get; } = source;
        public SpikeOrigin SpikeOrigin { get; } = spikeOrigin;
        public TriggerInfo TriggerInfo { get; } = triggerInfo;
        public int Charge { get; } = charge;
        public IEnumerable<FireInfo> ReflexArc { get; } = reflexArc;
    }
}
