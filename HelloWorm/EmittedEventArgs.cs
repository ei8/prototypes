namespace HelloWorm
{
    public class EmittedEventArgs : EventArgs 
    {
        public EmittedEventArgs(IEnumerable<IPhysical> emission)
        {
            this.Emission = emission ?? throw new ArgumentNullException(nameof(emission));
        }

        public IEnumerable<IPhysical> Emission { get; }
    }
}
