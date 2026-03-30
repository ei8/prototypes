using ei8.Prototypes.HelloWorm.Spiker.Neurons;

namespace ei8.Prototypes.HelloWorm
{
    public struct Constants
    {
        public struct NeuronId
        {
            // Inputs
            public static readonly string World = NeuronHelper.GetNewShortGuid();
            public static readonly string Odor = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector1 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector2 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector3 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector4 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector5 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector6 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector7 = NeuronHelper.GetNewShortGuid();
            public static readonly string Sector8 = NeuronHelper.GetNewShortGuid();

            // Interneurons
            public static readonly string OdorSector1 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector2 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector3 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector4 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector5 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector6 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector7 = NeuronHelper.GetNewShortGuid();
            public static readonly string OdorSector8 = NeuronHelper.GetNewShortGuid();

            public static readonly string WorldSector1 = NeuronHelper.GetNewShortGuid();
            public static readonly string WorldSector2 = NeuronHelper.GetNewShortGuid();
            public static readonly string WorldSector7 = NeuronHelper.GetNewShortGuid();
            public static readonly string WorldSector8 = NeuronHelper.GetNewShortGuid();

            // Outputs
            public static readonly string Rotate = NeuronHelper.GetNewShortGuid();
            public static readonly string Clockwise = NeuronHelper.GetNewShortGuid();
            public static readonly string CounterClockwise = NeuronHelper.GetNewShortGuid();
            public static readonly string Degrees22_5 = NeuronHelper.GetNewShortGuid();
            public static readonly string Degrees45 = NeuronHelper.GetNewShortGuid();
            public static readonly string Degrees60 = NeuronHelper.GetNewShortGuid();
            public static readonly string Degrees70 = NeuronHelper.GetNewShortGuid();
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

            public enum NeurotransmitterEffect
            {
                Inhibit = -1,
                NotSet = 0,
                Excite = 1
            }
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
