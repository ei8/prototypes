using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Prototypes.HelloWorm.Spiker;
using neurUL.Common.Domain.Model;
using NLog;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm
{
    public class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, ISpikableReporting
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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

        private int invokeFailureCount;
        private int rotationCount;
        private int collisionCount;
        private Size size;
        private Network? network;
        private readonly ConcurrentDictionary<DateTime, FireInfo> fireHistory;
        private readonly ConcurrentDictionary<Guid, SpikeInfo> spikeHistory;
        private Neuron? rotationNeuron;
        private IDictionary<Guid, RotationDirection>? directionValueDictionary;
        private IDictionary<Guid, RotationDegrees>? degreesValueDictionary;
        private IDictionary<SectorValues, Guid>? sectorsValueDictionary;
        private IDictionary<string, Guid>? targetsValueDictionary;
        private TimeSpan refractoryPeriod;
        private TimeSpan relativeSpikesPeriod;
        private readonly IList<IObject> components;
        private readonly ISpikeService spikeService;

        public Worm(ISpikeService spikeService)
        {
            this.invokeFailureCount = 0;
            this.rotationCount = 0;
            this.collisionCount = 0;
            this.Direction = 0;
            this.Location = new Point(0, 0);
            this.refractoryPeriod = Constants.Spiker.InitialRefractoryPeriod;
            this.relativeSpikesPeriod = Constants.Spiker.InitialRelativeSpikesPeriod;
            this.components = new List<IObject>();

            var nose = new Nose()
            {
                Location = new Point(0, 0),
                Size = new Size(1, 1)
            };
            var sectors = Worm.InitializeSectors();
            foreach (var s in sectors)
                nose.Add(s);

            this.Add(nose);

            this.network = null;

            this.fireHistory = new ConcurrentDictionary<DateTime, FireInfo>();
            this.spikeHistory = new ConcurrentDictionary<Guid, SpikeInfo>();

            this.rotationNeuron = null;
            this.directionValueDictionary = null;
            this.degreesValueDictionary = null;
            this.sectorsValueDictionary = null;
            this.targetsValueDictionary = null;
            this.spikeService = spikeService;
            this.spikeService.Triggered += this.SpikeService_Triggered;
            this.spikeService.Fired += this.SpikeService_Fired;
        }

        private void SpikeService_Triggered(object? sender, TriggeredEventArgs e)
        {
            Worm.logger.Debug(new LogMessageGenerator(() => $"Triggered: {e.Source.ToReadableString()}"));
        }

        private void SpikeService_Fired(object? sender, FiredEventArgs e)
        {
            if (
                this.rotationNeuron != null &&
                this.directionValueDictionary != null &&
                this.degreesValueDictionary != null &&
                this.TryParseMotorNeurons(
                    e.FireInfo,
                    [
                        new ResponseParser(
                            new Predicate<FireInfo>(
                                fi =>
                                    this.directionValueDictionary.ContainsKey(fi.Target.Id) ||
                                    this.degreesValueDictionary.ContainsKey(fi.Target.Id)
                            ),
                            this.rotationNeuron.Id,
                            [
                                // and any parameter1 was fired
                                new ParameterConverter(
                                    (fi, [NotNullWhen(true)] out or) =>
                                        fi.TryGetFiredParameter(this.directionValueDictionary, out or)
                                ),
                                // and any parameter2 was fired
                                new ParameterConverter(
                                    (fi, [NotNullWhen(true)] out or2) =>
                                        fi.TryGetFiredParameter(this.degreesValueDictionary, out or2)
                                )
                            ]
                        )
                    ],
                    out IEnumerable<object>? parseResult
                )
            )
                this.Rotate((RotationDirection)parseResult.ElementAt(0), (RotationDegrees)parseResult.ElementAt(1));
            else
                Worm.logger.Trace(new LogMessageGenerator(() => $"Invoke failed: {this.invokeFailureCount++}"));

            Worm.logger.Debug(new LogMessageGenerator(() => $"Fired: {e.FireInfo.Target.ToReadableString()}"));
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
        public IEnumerable<IObject> Components => this.components;

        public Network? Network => this.network;

        public IDictionary<DateTime, FireInfo> FireHistory => this.fireHistory;

        public IDictionary<Guid, SpikeInfo> NeuronSpikeHistory => this.spikeHistory;

        public float ProcessingRatio => ((float)this.rotationCount / (float)this.collisionCount) * 100f;

        public TimeSpan RefractoryPeriod { get => this.refractoryPeriod; set => this.refractoryPeriod = value; }

        public TimeSpan RelativeSpikesPeriod { get => this.relativeSpikesPeriod; set => this.relativeSpikesPeriod = value; }

        public void Rotate(RotationDirection direction, RotationDegrees degrees)
        {
            Worm.logger.Info(
                new LogMessageGenerator(
                    () => 
                    $"Rotating {direction} {degrees}. [Total: {++this.rotationCount}; " +
                    $"Processing ratio: { Math.Round(this.ProcessingRatio)}%]"
                )
            );

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

        public void Collide(CollisionInfo info)
        {
            if (info.Receiver is not Food)
                Worm.logger.Info(new LogMessageGenerator(() => $"Collided with {info.Receiver.GetType()}, rotation-required. [Total: {++this.collisionCount}]"));

            if (
                this.sectorsValueDictionary != null &&
                this.targetsValueDictionary != null &&
                this.TryParseSensoryNeurons(
                    [
                        new StimulusParser(
                            StimulusType.Internal,
                            (o) => o is ISectoral,
                            (o) => {
                                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId((ISectoral) o);
                                return this.sectorsValueDictionary[Enum.Parse<SectorValues>("Sector" + sectorId)];
                            }
                        ),
                        new StimulusParser(
                            StimulusType.External,
                            (o) => o is not Food,
                            (o) => this.targetsValueDictionary[o.GetType().ToKeyString()]
                        )
                    ],
                    out IEnumerable<Guid>? result,
                    // Collision cause is internal eg. Nose Sector
                    new StimulusInfo(StimulusType.Internal, info.Cause),
                    // Collision receiver is external eg. Dish, Food
                    new StimulusInfo(StimulusType.External, info.Receiver)
                )
            )
            {
                Worm.logger.Debug(
                    new LogMessageGenerator(
                        () =>
                            $"{info.Receiver.GetType()} stimulating Sector " +
                            $"{this.Components.OfType<Nose>().Single().GetSectorId((ISectoral)info.Cause)}"
                    )
                );
                this.spikeService.Spike(result, this);
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info));
        }

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

        public void Add(IObject @object)
        {
            this.components.Add(@object);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, @object));
        }

        public void Remove(IObject @object)
        {
            this.components.Remove(@object);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, @object));
        }
    }
}
