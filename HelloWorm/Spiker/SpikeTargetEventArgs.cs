namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeTargetEventArgs : EventArgs
    {
        public SpikeTargetEventArgs(Guid target)
        {
            Target = target;
        }

        public Guid Target { get; private set; }
    }
}
