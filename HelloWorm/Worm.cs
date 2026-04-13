using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm.neurULization;
using ei8.Prototypes.HelloWorm.Spiker;
using System.Collections.Concurrent;
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

        private static readonly IEnumerable<FauxNeurULizationMap<RotationDirection>> Param1ValueMaps = [
            new(Constants.NeuronId.Clockwise, RotationDirection.Clockwise),
            new(Constants.NeuronId.CounterClockwise, RotationDirection.CounterClockwise)
        ];

        private static readonly IEnumerable<FauxNeurULizationMap<float>> Param2ValueMaps = [
            new(Constants.NeuronId.Degrees22_5, 22.5f),
            new(Constants.NeuronId.Degrees45, 45f),
            new(Constants.NeuronId.Degrees60, 60f),
            new(Constants.NeuronId.Degrees70, 70f)
        ];

        private readonly Timer movementTriggerTimer;
        private ISpikeService spikeService;
        private Network network;
        private ConcurrentDictionary<DateTime, NeuronFireInfo> neuronFireInfos;

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
        public Network Network 
        { 
            get => network; 
            set
            {
                this.network = value;
                this.spikeService = new SpikeService(this.network);
                this.spikeService.Triggered += this.SpikeService_Triggered;
                this.spikeService.Fired += this.SpikeService_Fired;
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

            if (this.neuronFireInfos.Values.TryFauxDeneurULizeInvoke(
                Constants.NeuronId.Rotate,
                Worm.Param1ValueMaps,
                Worm.Param2ValueMaps,
                out RotationDirection? parameter1,
                out float? parameter2
            ))
                this.Rotate(parameter1!.Value, parameter2!.Value);
        }

        private void Rotate(RotationDirection direction, float degrees)
        {
#if DEBUG
            // Debug.WriteLine($"Rotating {direction}{degrees}! ////////////////////////////");
#endif

            this.Direction += degrees * (direction == RotationDirection.Clockwise ? 1 : -1);
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
                // Debug.WriteLine("Odor spiking Sector " + sectorId + @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
#endif
                var target2Id = Guid.Empty;

                if (info.Target is Odor)
                    target2Id = Constants.NeuronId.Odor;
                else if (info.Target is World)
                    target2Id = Constants.NeuronId.World;

                if (target2Id != Guid.Empty)
                {
                    this.spikeService.Spike(
                        [
                            (Guid) typeof(Constants.NeuronId).GetField("Sector" + sectorId)!.GetValue(null)!,
                            target2Id
                        ]
                    );
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

        public ISpikeService SpikeService => this.spikeService;
    }
}
