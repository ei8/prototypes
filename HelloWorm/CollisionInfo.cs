namespace HelloWorm
{
    internal class CollisionInfo
    {
        public required IPhysical CollisionTarget { get; set; }
        public required IPhysical CollisionSource { get; set; }
    }
}
