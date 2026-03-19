
namespace HelloWorm
{
    internal class Nose : IComposite, IElliptical
    {
        public Nose()
        {
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public required IEnumerable<IPhysical> Components { get; set; }
    }
}
