using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Prototypes.HelloWorm.Spiker;
using neurUL.Common.Domain.Model;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm
{
    public class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, ISpikable
    {
        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }

        public enum RotationDegrees
        {
            Small = 23,
            Medium = 45,
            Large = 60,
            ExtraLarge = 70
        }

        public enum SectorValues
        {
            Sector1,
            Sector2,
            Sector3,
            Sector4,
            Sector5,
            Sector6,
            Sector7,
            Sector8
        }

        private Size size;
        private Network? network;
        private readonly ConcurrentDictionary<DateTime, FireInfo> fireHistory;
        private readonly ConcurrentDictionary<Guid, SpikeInfo> spikeHistory;
        private Neuron? rotationNeuron;
        private IDictionary<Guid, RotationDirection>? directionValueDictionary;
        private IDictionary<Guid, RotationDegrees>? degreesValueDictionary;
        private IDictionary<SectorValues, Guid>? sectorsValueDictionary;
        private IDictionary<string, Guid>? targetsValueDictionary;

        public Worm()
        {
            this.Direction = 0;
            this.Location = new Point(0, 0);
            this.Components = [
                new Nose()
                {
                    Location = new Point(0, 0),
                    Size = new Size(1, 1),
                    Components = Worm.InitializeSectors()
                }
            ];

            this.network = null;

            this.fireHistory = new ConcurrentDictionary<DateTime, FireInfo>();
            this.spikeHistory = new ConcurrentDictionary<Guid, SpikeInfo>();

            this.rotationNeuron = null;
            this.directionValueDictionary = null;
            this.degreesValueDictionary = null;
            this.sectorsValueDictionary = null;
            this.targetsValueDictionary = null;
        }

        private static IEnumerable<Sector> InitializeSectors()
        {
            var sects = new List<Sector>();

            for (int i = 0; i < Constants.Worm.SectorRenderCount; i++)
                sects.Add(new Sector()
                {
                    StartAngle = (i * Constants.Worm.SectorSweepAngle) + 1,
                    SweepAngle = Constants.Worm.SectorSweepAngle
                });

            return sects;
        }

        public Point Location { get; set; }
        public Size Size
        {
            get => this.size;
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    this.Components.OfType<Nose>().Single().Size = new Size(value.Width, value.Width);
                }
            }
        }
        public float Direction { get; set; }
        public int Speed { get; set; }
        public int Score { get; set; }
        public int Life { get; set; }
        public IEnumerable<IPhysical> Components { get; set; }

        public Network? Network => this.network;

        public IDictionary<Guid, SpikeInfo> SpikeHistory => this.spikeHistory;

        public void Rotate(RotationDirection direction, RotationDegrees degrees)
        {
#if DEBUG
            // Debug.WriteLine($"Rotating {direction}{degrees}! ////////////////////////////");
#endif

            this.Direction += ((int)degrees) * (direction == RotationDirection.Clockwise ? 1 : -1);
        }

        public event EventHandler<MovingEventArgs>? Moving;
        public event EventHandler<CollidedEventArgs>? Collided;
        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        public void Grow()
        {
            this.Score++;
            this.Life += Constants.Worm.InitialLife;
            if (this.Size.Width < Constants.Worm.MaxWidth)
            {
                int newWidth = this.Size.Width + 2;
                this.Size = Worm.GetSizeByWidth(newWidth);
                this.Speed = Worm.GetSpeedByWidth(newWidth);
            }
        }

        public static Size GetSizeByWidth(int width)
        {
            return new Size(
                width,
                (int)
                (
                    Constants.Worm.MinLength +
                    (
                        (
                            Constants.Worm.MaxLength -
                            Constants.Worm.MinLength
                        ) *
                        Worm.GetGrowthPercentageByWidth(width)
                    )
                )
            );
        }

        public static int GetSpeedByWidth(int width)
        {
            return (int)
            (
                Constants.Worm.MinSpeed +
                (
                    Constants.Worm.MaxSpeed -
                    Constants.Worm.MinSpeed -
                    (
                        (
                            Constants.Worm.MaxSpeed -
                            Constants.Worm.MinSpeed
                        ) *
                        Worm.GetGrowthPercentageByWidth(width)
                    )
                )
            );
        }

        private static float GetGrowthPercentageByWidth(int width) =>
            ((float)(width - Constants.Worm.MinWidth)) / (Constants.Worm.MaxWidth - Constants.Worm.MinWidth);

        public void Collide(CollisionInfo info) => this.Collided?.Invoke(this, new CollidedEventArgs(info));

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);

        public void Inherit(IRegenerative original)
        {
            var originalWorm = (Worm) original;
            this.Initialize(originalWorm.Network);
            Worm.UpdateCaches(
                this,
                originalWorm.rotationNeuron,
                originalWorm.directionValueDictionary,
                originalWorm.degreesValueDictionary,
                originalWorm.sectorsValueDictionary,
                originalWorm.targetsValueDictionary
            );
        }

        public void Initialize(Size dishSize)
        {
            var r = new Random();
            var wormWidth = Constants.Worm.MinWidth;
            var center = dishSize / 2;
            this.Life = Constants.Worm.InitialLife;
            this.Direction = r.Next(Constants.CircleDegreesCount);
            this.Location = new Point(center.Width, center.Height);
            this.Size = Worm.GetSizeByWidth(wormWidth);
            this.Speed = Worm.GetSpeedByWidth(wormWidth);
        }

        public bool TryGetSpikeTargets<TS, TT>(TS source, TT target, [NotNullWhen(true)] out IEnumerable<Guid>? result)
            where TS : notnull
            where TT : notnull
        {
            var bResult = false;
            result = null;

            if (source is ISectoral sector)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector);

                AssertionConcern.AssertArgumentValid(
                    si => si > 0 && si <= Constants.Worm.SectorCount,
                    sectorId,
                    "Sector ID must in the range 1 to 8.",
                    nameof(sectorId)
                );

                if (
                    this.sectorsValueDictionary != null &&
                    this.targetsValueDictionary != null &&
                    target is not Food
                )
                {
                    result = [
                        this.sectorsValueDictionary[Enum.Parse<SectorValues>("Sector" + sectorId)],
                        this.targetsValueDictionary[target.GetType().ToKeyString()]
                    ];
                    bResult = true;
                }
            }

            return bResult;
        }

        public bool ProcessFire(FireInfo fireInfo)
        {
            bool result = false;

            this.fireHistory.Clean(
                (nfi) => nfi.Timestamp,
                fireInfo.Timestamp.Subtract(Constants.Spiker.RelativeSpikesPeriod)
            );

            this.fireHistory.TryAdd(fireInfo.Timestamp, fireInfo);

            if (
                this.rotationNeuron != null &&
                this.fireHistory.Values.TryFauxDeneurULizeInvoke(
                    this.rotationNeuron.Id,
                    this.directionValueDictionary!,
                    this.degreesValueDictionary!,
                    out RotationDirection? parameter1,
                    out RotationDegrees? parameter2
                )
            )
            {
                this.Rotate(parameter1.Value, parameter2.Value);
                result = true;
            }

            return result;
        }

        public void Initialize(Network? network, IEnumerable<MirrorConfig>? mirrorConfigs)
        {
            AssertionConcern.AssertArgumentNotNull(mirrorConfigs, nameof(mirrorConfigs));
            this.Initialize(network);

            if (this.network != null && mirrorConfigs != null)
            {
                var directionValueDictionary = Enum.GetValues<RotationDirection>().ConvertToNeuronValueMap(mirrorConfigs, this.network).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                var degreesValueDictionary = Enum.GetValues<RotationDegrees>().ConvertToNeuronValueMap(mirrorConfigs, this.network).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                var sectorsValueDictionary = Enum.GetValues<SectorValues>().ConvertToNeuronValueMap(mirrorConfigs, this.network).ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                if (
                    mirrorConfigs.TryGetMirrorNeuron(
                        typeof(Worm).ToMethodKeyString(
                            nameof(Worm.Rotate),
                            typeof(RotationDirection),
                            typeof(RotationDegrees)
                        ),
                        this.network,
                        out Neuron? rotationNeuron
                    ) &&
                    mirrorConfigs.TryGetMirrorNeuron(
                        typeof(Dish).ToKeyString(),
                        this.network,
                        out Neuron? dishNeuron
                    ) &&
                    mirrorConfigs.TryGetMirrorNeuron(
                        typeof(Odor).ToKeyString(),
                        this.network,
                        out Neuron? odorNeuron
                    )
                )
                {
                    var targetsValueDictionary = new Dictionary<string, Guid>{
                    { typeof(Dish).ToKeyString(), dishNeuron.Id },
                    { typeof(Odor).ToKeyString(), odorNeuron.Id },
                };

                    Worm.UpdateCaches(
                        this,
                        rotationNeuron,
                        directionValueDictionary,
                        degreesValueDictionary,
                        sectorsValueDictionary,
                        targetsValueDictionary
                    );
                }
            }
        }

        private static void UpdateCaches(
            Worm worm,
            Neuron? rotationNeuron,
            IDictionary<Guid, RotationDirection>? directionValueDictionary,
            IDictionary<Guid, RotationDegrees>? degreesValueDictionary,
            IDictionary<SectorValues, Guid>? sectorsValueDictionary,
            IDictionary<string, Guid>? targetsValueDictionary
        )
        {
            worm.rotationNeuron = rotationNeuron;
            worm.directionValueDictionary = directionValueDictionary;
            worm.degreesValueDictionary = degreesValueDictionary;
            worm.sectorsValueDictionary = sectorsValueDictionary;
            worm.targetsValueDictionary = targetsValueDictionary;
        }

        public void Initialize(Network? network)
        {
            if (this.network != network)
            {
                AssertionConcern.AssertArgumentNotNull(network, nameof(network));

                this.network = network;
            }
        }
    }
}
