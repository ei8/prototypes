using ei8.Prototypes.HelloWorm.Spiker.Neurons;

namespace ei8.Prototypes.HelloWorm
{
    internal static class Constants
    {
        public static class NeuronId
        {
            // TODO:
            public static readonly string Odor = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector1 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector8 = NeuronHelper.GetNewShortGuid();
            public static readonly string Rotate = NeuronHelper.GetNewShortGuid();
            public static readonly string Clockwise = NeuronHelper.GetNewShortGuid();
            public static readonly string CounterClockwise = NeuronHelper.GetNewShortGuid();
            public static readonly string Degrees22_5 = NeuronHelper.GetNewShortGuid();

            public static readonly string OdorSector1 = NeuronHelper.GetNewShortGuid();
        }

        public const bool ShouldDrawGrid = false;
        public const bool ShouldDrawRectangularRectangles = false;
        public const bool ShouldDrawSectorIds = false;
        public const bool ShouldDrawDirection = false;
        public const bool ShouldDrawScore = true;

        public const int MovementTriggerTimerPeriod = 100;
        public const int EmissionTriggerTimerPeriod = 500;
        public const int CircleDegreesCount = 360;

        public static class Spiker
        {
            public const int RelatedFiresPeriod = 35000;
        }

        public static class Worm
        {
            public const int InitialLife = 1000;
            public const int WidthGrowthRate = 2;
            public const int MinSpeed = 3;
            public const int MaxSpeed = 6;
            public const int MinLength = 80;
            public const int MaxLength = 150;
            public const int MinWidth = 30;
            public const int MaxWidth = 50;
            public const int SectorCount = 8;
            public const int SectorRenderCount = 8;
            public static readonly int SectorSweepAngle = Constants.CircleDegreesCount / Constants.Worm.SectorCount;
        }

        public static class Food
        {
            public const int InitialLife = 100;
        }

        public static class Odor
        {
            public const int Size = 2;
            public const int Speed = 5;
            public const int DeployMin = 2;
            public const int DeployExtra = 2;
        }
    }
}
