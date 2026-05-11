
namespace ei8.Prototypes.HelloWorm
{
    public class Sector : ISector
    {
        public float StartAngle { get; set; }

        public float SweepAngle { get; set; }

        public required IComposite Parent { get; set; }

        public required string Name { get; set; }

        public void Initialize(string name, IRectangularComposite parent)
        {
            this.Name = name;
            this.Parent = parent;
        }
    }
}
