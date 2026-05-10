
namespace ei8.Prototypes.HelloWorm
{
    public class Sector : IRectangleBoundSectoral
    {
        public float StartAngle { get; set; }

        public float SweepAngle { get; set; }

        public required IComposite Parent { get; set; }
    }
}
