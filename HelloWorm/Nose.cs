
namespace ei8.Prototypes.HelloWorm
{
    internal class Nose : IRectangularComposite, IElliptical
    {
        public Nose()
        {
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public required IEnumerable<IPhysical> Components { get; set; }
    }
}
