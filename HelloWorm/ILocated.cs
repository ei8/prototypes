namespace ei8.Prototypes.HelloWorm
{
    internal interface ILocated : IPhysical
    {
        public Point Location { get; set; }
    }
}
