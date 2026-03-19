namespace HelloWorm
{
    internal interface ILocated : IPhysical
    {
        public Point Location { get; set; }
    }
}
