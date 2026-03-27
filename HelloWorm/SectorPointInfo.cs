using System.Drawing.Drawing2D;

namespace ei8.Prototypes.HelloWorm
{
    public class SectorPointInfo(IRectangleBoundSectoral sector, GraphicsPath path, IEnumerable<Point> circumferencePoints)
    {
        public IRectangleBoundSectoral Sector { get; init; } = sector;
        public GraphicsPath Path { get; init; } = path;
        public IEnumerable<Point> CircumferencePoints { get; init; } = circumferencePoints;
    }
}