namespace ei8.Prototypes.HelloWorm
{
    public struct Constants
    {
        public enum PropertyCategory
        {
            Appearance,
            Time, 
            Visualization
        }

        public const int CircleDegreesCount = 360;

        public struct Worm
        {
            // NOTE: 94% after 200 collisions for Worm
            // public static readonly TimeSpan InitialRefractoryPeriod = new TimeSpan(0, 0, 0, 0, 1, 0);
            // NOTE: allowance for addition
            public static readonly TimeSpan InitialRefractoryPeriod = new TimeSpan(0, 0, 0, 2, 0, 0);
            public static readonly TimeSpan InitialRelatedSpikesPeriod = new TimeSpan(0, 0, 0, 0, 0, 100);

            public const int InitialLife = 1000;
            public const int WidthGrowthRate = 2;
            public const int MinSpeed = 4;
            public const int MaxSpeed = 5;
            public const int MinLength = 80;
            public const int MaxLength = 150;
            public const int MinWidth = 35;
            public const int MaxWidth = 50;
            public const int SectorCount = 8;
            public const int SectorRenderCount = 8;
            public static readonly int SectorSweepAngle = Constants.CircleDegreesCount / Constants.Worm.SectorCount;
        }

        public struct Food
        {
            public const int InitialLife = 500;
        }

        public struct Odor
        {
            public const int Size = 2;
            public const int MinSpeed = 3;
            public const int MaxSpeed = 5;
            public const int DeployMin = 3;
            public const int DeployExtra = 2;
        }

        public struct Render
        {
            public const int RegularOffset = 15;
        }
    }
}
