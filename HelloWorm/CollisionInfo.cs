namespace ei8.Prototypes.HelloWorm
{
    public class CollisionInfo(IPhysical receiver, IPhysical cause, int count)
    {
        public IPhysical Receiver { get; init; } = receiver;
        public IPhysical Cause { get; init; } = cause;
        public int Count { get; init; } = count;
    }
}
