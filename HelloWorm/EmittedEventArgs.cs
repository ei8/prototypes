namespace HelloWorm
{
    public class EmittedEventArgs<T> : EventArgs where T : IPhysical
    {
        public EmittedEventArgs(IEnumerable<T> emission)
        {
            this.Emission = emission ?? throw new ArgumentNullException(nameof(emission));
        }

        public IEnumerable<T> Emission { get; }
    }
}
