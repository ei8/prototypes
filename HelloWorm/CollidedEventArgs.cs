namespace ei8.Prototypes.HelloWorm
{
    public class CollidedEventArgs : EventArgs
    {
        public CollidedEventArgs(IPhysical target) => this.Target = target;

        public IPhysical Target { get; private set; }
    }
}
