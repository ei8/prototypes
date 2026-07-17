namespace ei8.Prototypes.HelloWorm
{
    public class EmittedEventArgs : EventArgs 
    {
        public EmittedEventArgs(IEnumerable<IComponent> emission)
        {
            ArgumentNullException.ThrowIfNull(emission);

            this.Emission = emission;
        }

        public IEnumerable<IComponent> Emission { get; }
    }
}
