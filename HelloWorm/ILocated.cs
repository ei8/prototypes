namespace ei8.Prototypes.HelloWorm
{
    public interface ILocated : IPhysical, IComponent
    {
        public Point Location { get; set; }
    }
}
