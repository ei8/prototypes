namespace ei8.Prototypes.HelloWorm
{
    public class CollidedEventArgs(CollisionInfo info) : EventArgs
    {
        public CollisionInfo Info { get; } = info;
    }
}
