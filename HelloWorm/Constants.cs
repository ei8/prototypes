namespace HelloWorm
{
    internal static class Constants
    {
        public const bool ShouldDrawGrid = true;
        public const bool ShouldDrawRectangularRectangles = false;
        public const bool ShouldDrawSectorIds = false;

        public const int MovementTriggerTimerPeriod = 200;
        public const int EmissionTriggerTimerPeriod = 2000;
        public const int CircleDegreesCount = 360;

        public static class Worm
        {
            public const int Speed = 5;
            public const int Width = 10;
            public const int SectorCount = 8;
            public const int SectorRenderCount = 8;
            public static readonly int SectorSweepAngle = Constants.CircleDegreesCount / Constants.Worm.SectorCount;
        }

        public static class Food
        {
            public const int Size = 3;
            public const int Speed = 6;
        }
    }
}
