namespace ei8.Prototypes.HelloWorm
{
    public class CollisionInfo(IComponent receiver, IComponent cause, int count)
    {
        public IComponent Receiver { get; init; } = receiver;
        public IComponent Cause { get; init; } = cause;
        public int Count { get; init; } = count;
    }
}
