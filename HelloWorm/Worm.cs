using HelloWorld.Spiker.Spikes;
using HelloWorm.neurULization;
using HelloWorm.Spiker.Neurons;
using System.Collections.Immutable;
using System.Diagnostics;
using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal class Worm : IMovable, IRectangularComposite, IElliptical, IPerishable, IRegenerative, INeurULized
    {
        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }

        private readonly Timer movementTriggerTimer;
        private readonly ISpikeService spikeService;
        private NeuronCollection neurons;
        private ImmutableQueue<NeuronFireInfo> neuronFireInfos;

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
            this.spikeService = new SpikeService();
            this.neuronFireInfos = ImmutableQueue<NeuronFireInfo>.Empty;
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
        public NeuronCollection Neurons 
        { 
            get => neurons; 
            set
            {
                neurons = value;

                foreach (var n in this.neurons)
                    n.Fired += this.N_Fired;
            }
        }

        private void N_Fired(object? sender, FiredEventArgs e)
        {
            var neuron = (Neuron) sender!;
            this.neuronFireInfos = this.neuronFireInfos.Enqueue(new(neuron, e.FireInfo));
            if (this.neuronFireInfos.Count() > 10)
                this.neuronFireInfos = this.neuronFireInfos.Dequeue();

            if (Worm.TryFauxDeneurULizeInvoke(
                this.neuronFireInfos,
                Constants.NeuronId.Rotate,
                Constants.Spiker.RelatedFiresPeriod,
                Worm.Param1ValueMaps,
                Worm.Param2ValueMaps,
                out RotationDirection? parameter1,
                out float? parameter2
            ))
            {
                this.Rotate(parameter1!.Value, parameter2!.Value);
            }
        }

        /// <summary>
        /// As indicated, this is a temporary approach. 
        /// Ideally, the fired neurons for a method and its parameters
        /// should be retrieved via mirrors if necessary, deneurULized, cached and invoked accordingly. 
        /// eg. Rotate Method (granny), Clockwise Direction Parameter (granny), 22.5 Float Degrees Parameter (granny)
        /// Using Method (class), MethodParameter (class)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="neuronFireInfos"></param>
        /// <param name="methodNeuronId"></param>
        /// <param name="param1ValueMaps"></param>
        /// <param name="param2ValueMaps"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        private static bool TryFauxDeneurULizeInvoke<T1, T2>(
            IEnumerable<NeuronFireInfo> neuronFireInfos, 
            string methodNeuronId, 
            int relatedFiresPeriod,
            IEnumerable<FauxNeurULizationMap<T1>> param1ValueMaps,
            IEnumerable<FauxNeurULizationMap<T2>> param2ValueMaps,
            out T1? parameter1, 
            out T2? parameter2
        ) 
            where T1 : struct
            where T2 : struct
        {
            bool result = false;
            parameter1 = default;
            parameter2 = default;

            var latestFire = neuronFireInfos.Last();
            // if last fired is one of the anticipated neurons
            // TODO: anticipated neurons can include instantiates grannies (eg. instantiates^methodParameter) to optimize recognition,
            // ie. no need to recognize all possible values
            if (latestFire.Neuron.Id == methodNeuronId ||
                param1ValueMaps.Any(p1vm => p1vm.NeuronId == latestFire.Neuron.Id) ||
                param2ValueMaps.Any(p2vm => p2vm.NeuronId == latestFire.Neuron.Id)
            )
            {
                // Get all fires within period since latest fire
                var relatedFires = neuronFireInfos.Where(nf =>
                    latestFire.FireInfo.Timestamp.Subtract(nf.FireInfo.Timestamp).TotalNanoseconds <
                    relatedFiresPeriod
                );

                // if number of related fires equals method + 2 parameters
                if (relatedFires.Count() >= 3)
                {
#if DEBUG
                    Debug.WriteLine("Nanos: " + string.Join(",", relatedFires.Select(rf => latestFire.FireInfo.Timestamp.Subtract(rf.FireInfo.Timestamp).TotalNanoseconds)));
                    Debug.WriteLine($"Related Fires: {string.Join(",", relatedFires.Select(rf => rf.Neuron.Data))}");
#endif
                    if (
                        // and specified method was fired
                        relatedFires.Any(n => n.Neuron.Id == methodNeuronId) &&
                        // and any param1 was fired
                        (parameter1 = param1ValueMaps.SingleOrDefault(pvm => relatedFires.Any(rf => pvm.NeuronId == rf.Neuron.Id))?.Value) != null &&
                        // and any param2 was fired
                        (parameter2 = param2ValueMaps.SingleOrDefault(pvm => relatedFires.Any(rf => pvm.NeuronId == rf.Neuron.Id))?.Value) != null
                    )
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private static readonly IEnumerable<FauxNeurULizationMap<RotationDirection>> Param1ValueMaps = [
            new(Constants.NeuronId.Clockwise, RotationDirection.Clockwise),
            new(Constants.NeuronId.CounterClockwise, RotationDirection.CounterClockwise)
        ];

        private static readonly IEnumerable<FauxNeurULizationMap<float>> Param2ValueMaps = [
            new(Constants.NeuronId.Degrees22_5, 22.5f)
        ];

        private void Rotate(RotationDirection direction, float degrees)
        {
#if DEBUG
            Debug.WriteLine($"Rotating {direction}{degrees}! <<<<<<<<<<<<<<<<");
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
            if (info.Target is World && info.Source is ISectoral sector)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector);
                // TODO:
                switch (sectorId)
                {
                    case 1:
                    case 8:
                        this.Direction += 45f * (sectorId == 8 ? 1 : -1);
                        break;
                    case 2:
                    case 7:
                        this.Direction += 22.5f * (sectorId == 7 ? 1 : -1);
                        break;
                }
            }
            else if (info.Target is Odor && info.Source is ISectoral sector2)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector2);
                if (sectorId == 1)
                {
#if DEBUG
                    Debug.WriteLine("Odor spiking Sector " + sectorId + ">>>>>>>>>>>>>>>>>>>>>>");
#endif
                    this.spikeService.Spike(
                        [
                            new SpikeTarget(typeof(Constants.NeuronId).GetField("Sector" + sectorId)!.GetValue(null)!.ToString()!),
                            new SpikeTarget(Constants.NeuronId.Odor)
                        ],
                        this.Neurons
                    );
                }
                // TODO:
                switch (sectorId)
                {
                    case 8:
                        this.Direction += 22.5f * (sectorId == 1 ? 1 : -1);
                        break;
                    case 2:
                    case 7:
                        this.Direction += 45f * (sectorId == 2 ? 1 : -1);
                        break;
                    case 3:
                    case 6:
                        this.Direction += 60f * (sectorId == 3 ? 1 : -1);
                        break;
                    case 4:
                    case 5:
                        this.Direction += 70f * (sectorId == 4 ? 1 : -1);
                        break;
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
    }
}
