namespace HelloWorm
{
    internal static class Constants
    {
        public const bool ShouldDrawGrid = false;
        public const bool ShouldDrawRectangularRectangles = false;
        public const bool ShouldDrawSectorIds = false;
        public const bool ShouldDrawDirection = false;

        public const int MovementTriggerTimerPeriod = 100;
        public const int EmissionTriggerTimerPeriod = 300;
        public const int CircleDegreesCount = 360;

        public static class Worm
        {
            public const int Speed = 3;
            public const int Width = 20;
            public const int SectorCount = 8;
            public const int SectorRenderCount = 8;
            public static readonly int SectorSweepAngle = Constants.CircleDegreesCount / Constants.Worm.SectorCount;
        }

        public static class Food
        {
            public const int Size = 3;
            public const int Speed = 6;
            public const int OdorDeployMax = 5;
        }
    }
}
