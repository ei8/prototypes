using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Cortex.Coding.Spiker;
using neurUL.Common.Domain.Model;
using NLog;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm
{
    public class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, ISpikableReporting, INamed
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
        private Neuron? rotationNeuron;
        private IDictionary<Guid, RotationDirection>? directionValueDictionary;
        private IDictionary<Guid, RotationDegrees>? degreesValueDictionary;
        private IDictionary<SectorValues, Guid>? sectorsValueDictionary;
        private IDictionary<string, Guid>? targetsValueDictionary;
        private TimeSpan refractoryPeriod;
        private TimeSpan relatedSpikesPeriod;
        private readonly IList<IComponent> components;
        private readonly ISpikeService spikeService;

        public Worm(ISpikeService spikeService)
        {
            this.invokeFailureCount = 0;
            this.rotationCount = 0;
            this.collisionCount = 0;
            this.Direction = 0;
            this.Location = new Point(0, 0);
            this.refractoryPeriod = Constants.Worm.InitialRefractoryPeriod;
            this.relatedSpikesPeriod = Constants.Worm.InitialRelatedSpikesPeriod;
            this.components = new List<IComponent>();

            var nose = new Nose()
            {
                Location = new Point(0, 0),
                Size = new Size(1, 1),
                Parent = this
            };
            var sectors = Worm.InitializeSectors(nose);
            foreach (var s in sectors)
                nose.Add(s);

            this.Add(nose);

            this.network = null;

            this.fireHistory = new ConcurrentDictionary<DateTime, FireInfo>();

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
            Worm.logger.Debug(
                new LogMessageGenerator(
                    () => {
                        var origin = e.ReflexArc.LastOrDefault();
                        var result = $"{this.GetFullName()} - " +
                        $"Triggered: {e.Target.ToReadableString()}; " +
                        $"By: {(origin != null ? origin.Target.ToReadableString() : "[Stimulus]")}; " +
                        $"Count: {e.Charge.Excitations.Count() + e.Charge.Inhibitions.Count()}; " +
                        $"Charge: ({ChargeInfo.RestingPotential}) + {e.Charge.Excitations.Sum(ChargeInfo.GetCharge)} + ({e.Charge.Inhibitions.Sum(ChargeInfo.GetCharge)}) = {e.Charge.Result} mV";

                        return result;
                    }
                )
            );
        }

        private void SpikeService_Fired(object? sender, FiredEventArgs e)
        {
            Worm.logger.Debug(new LogMessageGenerator(() => $"{this.GetFullName()} - Fired: {e.FireInfo.Target.ToReadableString()}"));

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
                    out IEnumerable<ParseMotorResult>? parseResult
                )
            )
            {
                Worm.logger.Debug(
                    new LogMessageGenerator(() =>
                        $"{this.GetFullName()} - Related Fires (microseconds): {string.Join(
                            ", ",
                            parseResult.Select(rf =>
                                rf.Neuron.ToReadableString() +
                                " (" +
                                    e.FireInfo.Timestamp.Subtract(rf.FireInfo.Timestamp).TotalMicroseconds +
                                ")"
                                )
                        )}"
                    )
                );
                var parseValues = parseResult.Select(pr => pr.Value);
                this.Rotate(parseValues.OfType<RotationDirection>().Single(), parseValues.OfType<RotationDegrees>().Single());
            }
            else
                Worm.logger.Trace(new LogMessageGenerator(() => $"Invoke failed: {this.invokeFailureCount++}"));
        }

        private static IEnumerable<Sector> InitializeSectors(Nose nose)
        {
            var sects = new List<Sector>();

            for (int i = 0; i < Constants.Worm.SectorRenderCount; i++)
                sects.Add(
                    new Sector()
                    {
                        StartAngle = (i * Constants.Worm.SectorSweepAngle) + 1,
                        SweepAngle = Constants.Worm.SectorSweepAngle,
                        Parent = nose,
                        Name = ExtensionMethods.CreateUnusedName(
                            (i) => $"{typeof(Sector).Name}{i.ToString()}",
                            (s) => sects.Any(fd => fd.Name == s)
                        )
                    }
                );

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
        public IEnumerable<IComponent> Components => this.components;

        public Network? Network => this.network;

        public IDictionary<DateTime, FireInfo> FireHistory => this.fireHistory;

        public float ProcessingRatio => ((float)this.rotationCount / (float)this.collisionCount) * 100f;

        public TimeSpan RefractoryPeriod { get => this.refractoryPeriod; set => this.refractoryPeriod = value; }

        public TimeSpan RelatedSpikesPeriod { get => this.relatedSpikesPeriod; set => this.relatedSpikesPeriod = value; }

        public required IComposite Parent { get; set; }
        
        public required string Name { get; set; }

        public void Rotate(RotationDirection direction, RotationDegrees degrees)
        {
            Worm.logger.Info(
                new LogMessageGenerator(
                    () => 
                    $"{this.GetFullName()} - Invoking {nameof(Worm.Rotate)}({direction}, {degrees}). [Total: {++this.rotationCount}; " +
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
                Worm.logger.Info(new LogMessageGenerator(() => $"{info.Cause.GetFullName()} collided with {info.Receiver.GetFullName()}, rotation-required. [Total: {++this.collisionCount}]"));

            if (
                this.network != null &&
                this.sectorsValueDictionary != null &&
                this.targetsValueDictionary != null &&
                this.TryParseSensoryNeurons(
                    [
                        new StimulusParser(
                            StimulusType.Internal,
                            (o) => o is ISector,
                            (o) => {
                                var sector = (ISector) o;
                                return this.network.ValidateGet(this.sectorsValueDictionary[Enum.Parse<SectorValues>(sector.Name)]);
                            }
                        ),
                        new StimulusParser(
                            StimulusType.External,
                            (o) => o is not Food,
                            (o) => this.network.ValidateGet(this.targetsValueDictionary[o.GetType().ToKeyString()])
                        )
                    ],
                    out IEnumerable<ParseSensorResult>? result,
                    // Collision cause is internal eg. Nose Sector
                    new StimulusInfo(StimulusType.Internal, info.Cause),
                    // Collision receiver is external eg. Dish, Food
                    new StimulusInfo(StimulusType.External, info.Receiver)
                )
            )
            {
                Worm.logger.Debug(
                    new LogMessageGenerator(
                        () => $"{this.GetFullName()} - {info.Receiver.GetFullName()} stimulating {info.Cause.GetFullName()}."
                    )
                );
                this.spikeService.Spike(result.Select(r => r.Value), this.network, this.refractoryPeriod);
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info));
        }

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);

        public void Inherit(IRegenerative original)
        {
            var originalWorm = (Worm) original;
            this.Initialize(originalWorm.Network);
            this.Parent = originalWorm.Parent;
            this.Name = originalWorm.Name;
            Worm.UpdateCaches(
                this,
                originalWorm.rotationNeuron,
                originalWorm.directionValueDictionary,
                originalWorm.degreesValueDictionary,
                originalWorm.sectorsValueDictionary,
                originalWorm.targetsValueDictionary
            );
        }

        public void Initialize(string name, IRectangularComposite parent)
        {
            var r = new Random();
            var wormWidth = Constants.Worm.MinWidth;
            var center = parent.Size / 2;
            this.Life = Constants.Worm.InitialLife;
            this.Direction = r.Next(Constants.CircleDegreesCount);
            this.Location = new Point(center.Width, center.Height);
            this.Size = Worm.GetSizeByWidth(wormWidth);
            this.Speed = Worm.GetSpeedByWidth(wormWidth);
            this.Name = name;
            this.Parent = parent;
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

        public void Add(IComponent component)
        {
            this.components.Add(component);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, component));
        }

        public void Remove(IComponent component)
        {
            this.components.Remove(component);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, component));
        }
    }
}
