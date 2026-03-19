
namespace HelloWorm
{
    internal class Sector : ISectoral
    {
        public required string Name { get; set; }

        public float StartAngle { get; set; }

        public float SweepAngle { get; set; }
    }
}
