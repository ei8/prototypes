using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class FiredEventArgs(ISpikable sender, FireInfo fireInfo, int charge) : EventArgs
    {
        public ISpikable Sender { get; } = sender;
        public FireInfo FireInfo { get; } = fireInfo;
        public int Charge { get; } = charge;
    }
}
