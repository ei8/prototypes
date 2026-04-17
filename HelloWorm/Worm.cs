using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Prototypes.HelloWorm.neurULization;
using ei8.Prototypes.HelloWorm.Spiker;
using neurUL.Common.Domain.Model;
using System.Collections.Concurrent;
using System.Diagnostics;
using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    internal class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, INeurULized
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

        private readonly Timer movementTriggerTimer;
        private ISpikeService? spikeService;
        private Network? network;
        private ConcurrentDictionary<DateTime, NeuronFireInfo> neuronFireInfos;
        private IEnumerable<MirrorConfig>? mirrorConfigs;
        private IEnumerable<NeuronValueMap<RotationDirection>>? directionValueMap;
        private IEnumerable<NeuronValueMap<RotationDegrees>>? degreesValueMap;
        private IEnumerable<NeuronValueMap<SectorValues>>? sectorsValueMap;
        private IEnumerable<NeuronValueMap<string>>? targetsValueMap;
        private Neuron? rotationNeuron;

        public Worm() : this(0, 0, 0, 0)
        {
        }

        public Worm(int direction, int x, int y, int width)
        {
            this.Direction = direction;
            this.Location = new Point(x, y);
            this.Life = Constants.Worm.InitialLife;
            this.Components = [
                new Nose()
                {
                    Location = new Point(0, 0),
                    Size = new Size(1, 1),
                    Components = Worm.InitializeSectors()
                }
            ];
            this.UpdateSize(Constants.Worm.MinWidth);

            this.movementTriggerTimer = new Timer(this.WrapMove, null, 0, Constants.MovementTriggerTimerPeriod);
            this.neuronFireInfos = new ConcurrentDictionary<DateTime, NeuronFireInfo>();
            this.mirrorConfigs = null;
            this.directionValueMap = null;
            this.degreesValueMap = null;
            this.sectorsValueMap = null;
            this.targetsValueMap = null;
            this.rotationNeuron = null;
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
        public Size Size { get; set; }
        public float Direction { get; set; }
        public int Speed { get; set; }
        public int Score { get; set; }
        public int Life { get; set; }
        public IEnumerable<IPhysical> Components { get; set; }
        
        public Network? Network => this.network;
        
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
                    this.directionValueMap!,
                    this.degreesValueMap!,
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

        public void Grow()
        {
            this.Score++;
            this.Life += Constants.Worm.InitialLife;
            if (this.Size.Width < Constants.Worm.MaxWidth)
                this.UpdateSize(this.Size.Width + 2);
        }

        private void UpdateSize(int width)
        {
            float pctMax = 
                ((float)(width - Constants.Worm.MinWidth)) / 
                (Constants.Worm.MaxWidth - Constants.Worm.MinWidth);
            this.Size = new Size(
                width, 
                (int)
                (
                    Constants.Worm.MinLength + 
                    (
                        (
                            Constants.Worm.MaxLength - 
                            Constants.Worm.MinLength
                        ) * 
                        pctMax
                    )
                )
            );
            this.Speed = 
                (int)
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
                            pctMax
                        )
                    )
                );
            this.Components.OfType<Nose>().Single().Size = new Size(width, width);
        }

        private void WrapMove(object? state)
        {
            this.Life--;
            this.Move(state);
        }

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
                    this.sectorsValueMap != null &&
                    this.targetsValueMap != null
                )
                {
                    var sectorMatch = this.sectorsValueMap.SingleOrDefault(svm => svm.Value == Enum.Parse<SectorValues>("Sector" + sectorId));
                    var targetMatch = this.targetsValueMap.SingleOrDefault(svm => svm.Value == info.Target.GetType().ToKeyString());
                    if (sectorMatch != null && targetMatch != null)
                    {
                        this.spikeService.Spike(
                            [
                                sectorMatch.NeuronId,
                                targetMatch.NeuronId
                            ]
                        );
                    }
                }
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info.Target));
        }
        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);

        public IPhysical Create(Size worldSize)
        {
            var r = new Random();
            var size = Constants.Worm.MinWidth;
            var center = worldSize / 2;
            return new Worm(r.Next(Constants.CircleDegreesCount), center.Width, center.Height, size);
        }

        public void Initialize(Network network, IEnumerable<MirrorConfig> mirrorConfigs)
        {
            AssertionConcern.AssertArgumentNotNull(network, nameof(network));
            AssertionConcern.AssertArgumentNotNull(mirrorConfigs, nameof(mirrorConfigs));

            this.network = network;
            this.spikeService = new SpikeService(this.network);
            this.spikeService.Triggered += this.SpikeService_Triggered;
            this.spikeService.Fired += this.SpikeService_Fired;

            this.mirrorConfigs = mirrorConfigs;

            this.directionValueMap = Enum.GetValues<RotationDirection>().ConvertToNeuronValueMap(this.mirrorConfigs, this.network);
            this.degreesValueMap = Enum.GetValues<RotationDegrees>().ConvertToNeuronValueMap(this.mirrorConfigs, this.network);
            this.sectorsValueMap = Enum.GetValues<SectorValues>().ConvertToNeuronValueMap(this.mirrorConfigs, this.network);

            if (this.MirrorConfigs != null && this.network != null)
            {
                if (
                    this.mirrorConfigs.TryGetMirrorNeuron(
                        typeof(Worm).ToMethodKeyString("Rotate", typeof(RotationDirection), typeof(RotationDegrees)),
                        this.network,
                        out Neuron? rotationNeuron
                    )
                )
                    this.rotationNeuron = rotationNeuron;

                if (
                    this.mirrorConfigs.TryGetMirrorNeuron(
                        typeof(World).ToKeyString(),
                        this.network,
                        out Neuron? worldNeuron
                    ) &&
                    this.mirrorConfigs.TryGetMirrorNeuron(
                        typeof(Odor).ToKeyString(),
                        this.network,
                        out Neuron? odorNeuron
                    )
                )
                {
                    this.targetsValueMap = [
                        new NeuronValueMap<string>(worldNeuron.Id, typeof(World).ToKeyString()),
                        new NeuronValueMap<string>(odorNeuron.Id, typeof(Odor).ToKeyString()),
                    ];
                }
            }
        }

        public ISpikeService? SpikeService => this.spikeService;

        public IEnumerable<MirrorConfig>? MirrorConfigs => this.mirrorConfigs;
    }
}
