namespace ei8.Prototypes.HelloWorm
{
    public class EmittedEventArgs : EventArgs 
    {
        public EmittedEventArgs(IEnumerable<IComponent> emission)
        {
            this.Emission = emission ?? throw new ArgumentNullException(nameof(emission));
        }

        public IEnumerable<IComponent> Emission { get; }
    }
}
