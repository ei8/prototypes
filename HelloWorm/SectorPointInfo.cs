namespace HelloWorm
{
    public class SectorPointInfo(IRectangleBoundSectoral sector, IEnumerable<Point> points)
    {
        public IRectangleBoundSectoral Sector { get; init; } = sector;
        public IEnumerable<Point> Points { get; init; } = points;
    }
}