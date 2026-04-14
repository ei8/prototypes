using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class FiredEventArgs : EventArgs
    {
        public FiredEventArgs(Neuron source, FireInfo fireInfo, int charge)
        {
            this.Source = source;
            this.FireInfo = fireInfo;
            this.Charge = charge;
        }

        public Neuron Source { get; }
        public FireInfo FireInfo { get; private set; }
        public int Charge { get; private set; }
    }
}
