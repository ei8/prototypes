using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class TriggeredEventArgs : EventArgs
    {
        public TriggeredEventArgs(Neuron source, SpikeOrigin spikeOrigin, TriggerInfo triggerInfo, int charge, IEnumerable<FireInfo> path)
        {
            Source = source;
            SpikeOrigin = spikeOrigin;
            TriggerInfo = triggerInfo;
            Charge = charge;
            Path = path;
        }

        public Neuron Source { get; }
        public SpikeOrigin SpikeOrigin { get; private set; }
        public TriggerInfo TriggerInfo { get; private set; }
        public int Charge { get; private set; }
        public IEnumerable<FireInfo> Path { get; private set; }
    }
}
