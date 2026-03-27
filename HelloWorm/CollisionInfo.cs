namespace ei8.Prototypes.HelloWorm
{
    public class CollisionInfo(IPhysical target, IPhysical source, int count)
    {
        public IPhysical Target { get; init; } = target;
        public IPhysical Source { get; init; } = source;
        public int Count { get; init; } = count;
    }
}
