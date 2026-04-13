namespace ei8.Prototypes.HelloWorm
{
    public struct Constants
    {
        public struct NeuronId
        {
            // Inputs
            public static readonly Guid World = Guid.NewGuid();
            public static readonly Guid Odor = Guid.NewGuid();
            public static readonly Guid Sector1 = Guid.NewGuid();
            public static readonly Guid Sector2 = Guid.NewGuid();
            public static readonly Guid Sector3 = Guid.NewGuid();
            public static readonly Guid Sector4 = Guid.NewGuid();
            public static readonly Guid Sector5 = Guid.NewGuid();
            public static readonly Guid Sector6 = Guid.NewGuid();
            public static readonly Guid Sector7 = Guid.NewGuid();
            public static readonly Guid Sector8 = Guid.NewGuid();

            // Interneurons
            public static readonly Guid OdorSector1 = Guid.NewGuid();
            public static readonly Guid OdorSector2 = Guid.NewGuid();
            public static readonly Guid OdorSector3 = Guid.NewGuid();
            public static readonly Guid OdorSector4 = Guid.NewGuid();
            public static readonly Guid OdorSector5 = Guid.NewGuid();
            public static readonly Guid OdorSector6 = Guid.NewGuid();
            public static readonly Guid OdorSector7 = Guid.NewGuid();
            public static readonly Guid OdorSector8 = Guid.NewGuid();

            public static readonly Guid WorldSector1 = Guid.NewGuid();
            public static readonly Guid WorldSector2 = Guid.NewGuid();
            public static readonly Guid WorldSector7 = Guid.NewGuid();
            public static readonly Guid WorldSector8 = Guid.NewGuid();

            // Outputs
            public static readonly Guid Rotate = Guid.NewGuid();
            public static readonly Guid Clockwise = Guid.NewGuid();
            public static readonly Guid CounterClockwise = Guid.NewGuid();
            public static readonly Guid Degrees22_5 = Guid.NewGuid();
            public static readonly Guid Degrees45 = Guid.NewGuid();
            public static readonly Guid Degrees60 = Guid.NewGuid();
            public static readonly Guid Degrees70 = Guid.NewGuid();
        }

        public const bool ShouldDrawGrid = false;
        public const bool ShouldDrawRectangularRectangles = false;
        public const bool ShouldDrawSectorIds = false;
        public const bool ShouldDrawDirection = false;
        public const bool ShouldDrawScore = true;

        public const int MovementTriggerTimerPeriod = 100;
        public const int EmissionTriggerTimerPeriod = 500;
        public const int CircleDegreesCount = 360;

        public struct Spiker
        {
            public const int DefaultThreshold = -55;
            public const int RestingPotential = -70;
            public const int SpikeDepolarizationAmount = 15;
            public static readonly TimeSpan RefractoryPeriod = new TimeSpan(0, 0, 0, 0, 50, 0);
            public static readonly TimeSpan RelativeSpikesPeriod = new TimeSpan(0, 0, 0, 0, 0, 20);
        }

        public struct Worm
        {
            public const int InitialLife = 1000;
            public const int WidthGrowthRate = 2;
            public const int MinSpeed = 4;
            public const int MaxSpeed = 5;
            public const int MinLength = 80;
            public const int MaxLength = 150;
            public const int MinWidth = 30;
            public const int MaxWidth = 50;
            public const int SectorCount = 8;
            public const int SectorRenderCount = 8;
            public static readonly int SectorSweepAngle = Constants.CircleDegreesCount / Constants.Worm.SectorCount;
        }

        public struct Food
        {
            public const int InitialLife = 200;
        }

        public struct Odor
        {
            public const int Size = 2;
            public const int Speed = 5;
            public const int DeployMin = 3;
            public const int DeployExtra = 2;
        }

        public struct Render
        {
            public const int RegularOffset = 15;
        }
    }
}
