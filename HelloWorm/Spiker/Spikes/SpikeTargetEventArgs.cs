namespace ei8.Prototypes.HelloWorm.Spiker.Spikes
{
    public class SpikeTargetEventArgs : EventArgs
    {
        public SpikeTargetEventArgs(Guid target)
        {
            this.Target = target;
        }

        public Guid Target { get; private set; }
    }
}
