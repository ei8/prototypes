using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Prototypes.HelloWorm.Spiker;
using Microsoft.Extensions.Logging;
using neurUL.Common.Domain.Model;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    public class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, INeurULized
    {
        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }

        public enum RotationDegrees
        {
            Small       = 23,
            Medium      = 45,
            Large       = 60,
            ExtraLarge  = 70
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

        // TODO: Extricate from Worm
        private ISpikeService spikeService;
        private Network network;
        private ConcurrentDictionary<DateTime, NeuronFireInfo> neuronFireInfos;
        private IDictionary<Guid, RotationDirection>? directionValueDictionary;
        private IDictionary<Guid, RotationDegrees>? degreesValueDictionary;
        private IDictionary<SectorValues, Guid>? sectorsValueDictionary;
        private IDictionary<string, Guid>? targetsValueDictionary;
        private Neuron? rotationNeuron;
        private readonly ILogger<Worm> logger;

        // TODO: Extricate from Worm
        private readonly ISettingsService settingsService;

        public Worm(ILogger<Worm> logger, ISpikeService spikeService, ISettingsService settingsService)
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

            this.neuronFireInfos = new ConcurrentDictionary<DateTime, NeuronFireInfo>();
            this.directionValueDictionary = null;
            this.degreesValueDictionary = null;
            this.sectorsValueDictionary = null;
            this.targetsValueDictionary = null;
            this.rotationNeuron = null;
            
            this.logger = logger;
            this.spikeService = spikeService;
            this.settingsService = settingsService;

            this.spikeService.Triggered += this.SpikeService_Triggered;
            this.spikeService.Fired += this.SpikeService_Fired;
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
        
        public Network Network 
        {
            get => this.network;
            set
            {
                if (this.network != value)
                {
                    AssertionConcern.AssertArgumentNotNull(value, nameof(value));

                    this.network = value;

                    this.directionValueDictionary = Enum.GetValues<RotationDirection>().ConvertToNeuronValueMap(this.settingsService.Mirrors, this.network).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    this.degreesValueDictionary = Enum.GetValues<RotationDegrees>().ConvertToNeuronValueMap(this.settingsService.Mirrors, this.network).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    this.sectorsValueDictionary = Enum.GetValues<SectorValues>().ConvertToNeuronValueMap(this.settingsService.Mirrors, this.network).ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                    if (
                        this.settingsService.Mirrors.TryGetMirrorNeuron(
                            typeof(Worm).ToMethodKeyString("Rotate", typeof(RotationDirection), typeof(RotationDegrees)),
                            this.network,
                            out Neuron? rotationNeuron
                        )
                    )
                        this.rotationNeuron = rotationNeuron;

                    if (
                        this.settingsService.Mirrors.TryGetMirrorNeuron(
                            typeof(Dish).ToKeyString(),
                            this.network,
                            out Neuron? dishNeuron
                        ) &&
                        this.settingsService.Mirrors.TryGetMirrorNeuron(
                            typeof(Odor).ToKeyString(),
                            this.network,
                            out Neuron? odorNeuron
                        )
                    )
                    {
                        this.targetsValueDictionary = new Dictionary<string, Guid>{
                            { typeof(Dish).ToKeyString(), dishNeuron.Id },
                            { typeof(Odor).ToKeyString(), odorNeuron.Id },
                        };
                    }
                }
            }
        }
        
        private void SpikeService_Triggered(object? sender, TriggeredEventArgs e)
        {
#region DEBUG
            //Debug.WriteLine("Triggered: " + ((Neuron)sender!).ToString());
#endregion
        }

        private void SpikeService_Fired(object? sender, FiredEventArgs e)
        {
            #region DEBUG
            //            Debug.WriteLine("Fired: " + ((Neuron)sender!).ToString());
            #endregion
            var neuron = e.Source;
            this.neuronFireInfos.Clean(
                (nfi) => nfi.FireInfo.Timestamp, 
                e.FireInfo.Timestamp.Subtract(Constants.Spiker.RelativeSpikesPeriod)
            );

            var nnfi = new NeuronFireInfo(neuron, e.FireInfo);
            this.neuronFireInfos.TryAdd(nnfi.FireInfo.Timestamp, nnfi);

            if (
                this.rotationNeuron != null &&
                this.neuronFireInfos.Values.TryFauxDeneurULizeInvoke(
                    this.rotationNeuron.Id,
                    this.directionValueDictionary!,
                    this.degreesValueDictionary!,
                    out RotationDirection? parameter1,
                    out RotationDegrees? parameter2
                )
            )
                this.Rotate(parameter1.Value, parameter2.Value);
        }

        private void Rotate(RotationDirection direction, RotationDegrees degrees)
        {
#if DEBUG
            // Debug.WriteLine($"Rotating {direction}{degrees}! ////////////////////////////");
#endif

            this.Direction += ((int) degrees) * (direction == RotationDirection.Clockwise ? 1 : -1);
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
            if (info.Source is ISectoral sector)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector);
#if DEBUG
                // Debug.WriteLine($"{info.Target.GetType()} spiking Sector {sectorId}" + @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
#endif
                if (
                    this.network != null &&
                    this.spikeService != null &&
                    this.sectorsValueDictionary != null &&
                    this.targetsValueDictionary != null &&
                    info.Target is not Food
                )
                {
                    var sectorMatch = this.sectorsValueDictionary[Enum.Parse<SectorValues>("Sector" + sectorId)];
                    var targetMatch = this.targetsValueDictionary[info.Target.GetType().ToKeyString()];

                    this.spikeService.Spike(
                        [
                            sectorMatch,
                            targetMatch
                        ],
                        this.network
                    );
                }
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info.Target));
        }
        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);

        public void Inherit(IRegenerative original)
        {
            this.Network = ((Worm)original).Network;
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
    }
}
