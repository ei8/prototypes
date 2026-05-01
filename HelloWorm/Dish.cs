using ei8.Prototypes.HelloWorm.Spiker;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public class Dish : IRectangularComposite, ITemporal<Dish.ModeValue>, INamed, INotifyPropertyChanged
    {
        public enum ModeValue
        {
            Forage,
            RotationTest
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        private IImmutableList<IObject> components;
        private DateTime lastEmission;

        private bool isPlaying;
        private ModeValue mode;

        private string name;

        private int timerResolution;
        private readonly IServiceProvider serviceProvider;
        private readonly ISpikeService spikeService;

        private object IndexLock { get; } = new();

        public Dish(IServiceProvider serviceProvider, ISpikeService spikeService)
        {
            this.Location = new Point(0, 0);
            this.Size = Size.Empty;
            this.components = ImmutableList<IObject>.Empty;
            this.Regenerate = true;
            this.lastEmission = DateTime.MinValue;

            this.isPlaying = false;
            this.mode = ModeValue.Forage;

            this.name = string.Empty;

            this.ShowGrid = false;
            this.ShowRectangularRectangles = false;
            this.ShowSectorIds = false;
            this.ShowDirection = false;
            this.ShowScore = true;
            this.ShowLife = true;
            this.ShowOdor = true;
            this.ShowFocus = true;

            this.TimerResolution = 50;
            this.EmissionInterval = 500;

            this.serviceProvider = serviceProvider;
            this.spikeService = spikeService;
            this.spikeService.Triggered += this.SpikeService_Triggered;
            this.spikeService.Fired += this.SpikeService_Fired;
        }

        private void SpikeService_Triggered(object? sender, TriggeredEventArgs e)
        {
            Dish.logger.Debug(new LogMessageGenerator(() => $"{this.Name} - Triggered: {e.Source.Id}:'{e.Source.Tag}'"));
        }

        private void SpikeService_Fired(object? sender, FiredEventArgs e)
        {
            if (e.Sender is ISpikable spikable)
                spikable.ProcessFire(e.FireInfo);

            Dish.logger.Debug(new LogMessageGenerator(() => $"{this.Name} - Fired: {e.FireInfo.Target.Id}:'{e.FireInfo.Target.Tag}'"));
        }

        public void ProcessTick()
        {
            switch (this.mode)
            {
                case ModeValue.RotationTest:
                    foreach (var worm in this.Components.OfType<Worm>())
                    {
                        worm.Collide(
                            new CollisionInfo(
                                new Odor(),
                                worm.Components.OfType<Nose>().Single().Components.OfType<Sector>().First(),
                                1
                            )
                        );
                    }
                    break;
                case ModeValue.Forage:
                    var emissionIntervalTimestamp = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 0, this.EmissionInterval));
                    bool emit = this.lastEmission < emissionIntervalTimestamp;
                    if (emit) this.lastEmission = DateTime.Now;

                    foreach (var ph in this.components.OfType<IPhysical>())
                    {
                        if (ph is IPerishable perishable)
                        {
                            perishable.Life--;
                            if (perishable.Life < 0)
                                this.Remove(perishable);
                        }

                        if (ph is IMovable m)
                            m.Move();

                        if (ph is IEmitter e && emit)
                        {
                            this.lastEmission = DateTime.Now;
                            e.Emit();
                        }
                    }
                    break;
            }
        }

        public void Add(IObject @object)
        {
            lock (this.IndexLock)
            {
                this.components = this.components.Add(@object);
            }

            if (@object is not Odor)
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.name} - {@object.GetType()} added."));

            if (@object is IMovable movable)
            {
                movable.Moving += this.Movable_Moving;
                movable.Collided += this.Movable_Collided;
            }

            if (@object is IEmitter emitter)
                emitter.Emitted += this.Emitter_Emitted;

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    @object
                )
            );
        }

        public void Remove(IObject @object)
        {
            lock (this.IndexLock)
            {
                if (this.components.Contains(@object))
                {
                    this.components = this.components.Remove(@object);
                }
            }

            if (@object is not Odor)
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.name} - {@object.GetType()} removed."));

            if (@object is IMovable movable)
            {
                movable.Moving -= this.Movable_Moving;
                movable.Collided -= this.Movable_Collided;
            }

            if (@object is IEmitter emitter)
                emitter.Emitted -= this.Emitter_Emitted;

            if (
                @object is IRegenerative original &&
                this.Regenerate
            )
            {
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.name} - {@object.GetType()} regenerating..."));
                var regen = (IRegenerative)this.serviceProvider.GetRequiredService(original.GetType());
                regen.Initialize(this.Size);
                regen.Inherit(original);
                this.Add(regen);
            }

            this.NotifyCollectionChanged?.Invoke(
                this, 
                new NotifyCollectionChangedEventArgs(
                     NotifyCollectionChangedAction.Remove,
                     @object
                )
            );
        }

        private void Movable_Collided(object? sender, CollidedEventArgs e)
        {
            if (sender is Odor odor && e.Info.Target is Dish)
            {
                this.Remove(odor);
            }
            else if (
                sender is Worm worm &&
                e.Info.Target is Odor odor2
            )
            {
                this.Remove(odor2);
            }
            else if (
                sender is Worm worm2 &&
                e.Info.Target is Food food
            )
            {
                this.Remove(food);
                worm2.Grow();
            }

            if (
                sender is ISpikable spikable &&
                spikable.TryGetSpikeTargets(
                    e.Info.Source,
                    e.Info.Target,
                    out IEnumerable<Guid>? spikeTargets
                )
            )
            {
#if DEBUG
                // Debug.WriteLine($"{info.Target.GetType()} spiking Sector {sectorId}" + @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
#endif
                this.spikeService.Spike(
                    spikeTargets,
                    spikable
                );
            }
        }

        private void Movable_Moving(object? sender, MovingEventArgs e)
        {
            if (
                sender is Odor odor &&
                !this.GetRectangle().Contains(e.NewLocation)
            )
            {
                e.CollisionInfo = new(this, odor, 1);
            }
            // if worm...
            else if (sender is Worm worm)
            {
                var nose = worm.Components.OfType<Nose>().Single();

                // ...collides with...
                var collisionSector = nose.GetCollisionSector(
                    (sp) =>
                    {
                        CollisionInfo? result = null;

                        var sectorId = nose.GetSectorId(sp.Sector);
                        var spCount = 0;
                        //  ...dish
                        if ((sectorId < 3 || sectorId > 6) && (spCount = sp.CircumferencePoints.Count(spp => !this.GetRectangle().Contains(spp))) > 0)
                            result = new(this, sp.Sector, spCount);

                        // ...odor
                        if (result == null)
                            result = sp.GetCollisionInfo(this.components.OfType<Odor>());

                        // ...food
                        if (result == null)
                            result = sp.GetCollisionInfo(this.components.OfType<Food>());

                        return result;
                    },
                    (angle) => angle + worm.Direction,
                    (location) => location.Add(worm.Location)
                );
                if (collisionSector != null)
                    e.CollisionInfo = collisionSector;
            }
        }

        private void Emitter_Emitted(object? sender, EmittedEventArgs e)
        {
            foreach (var em in e.Emission)
                this.Add(em);
        }

        public void Play()
        {
            this.IsPlaying = true;
        }

        public void Pause()
        {
            this.IsPlaying = false;
        }

        public ModeValue Mode
        {
            get => this.mode;
            set
            {
                this.mode = value;
            }
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public IEnumerable<IObject> Components => this.components; 
        public bool Regenerate { get; set; }
        public bool IsPlaying 
        {
            get => this.isPlaying;
            set
            {
                if (this.isPlaying != value)
                {
                    this.isPlaying = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPlaying)));

                    Dish.logger.Info(new LogMessageGenerator(() => $"{this.name} - {(this.IsPlaying ? "played." : "paused.")}"));
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowGrid { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowRectangularRectangles { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowSectorIds { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowDirection { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowScore { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowLife { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowOdor { get; set; }
        
        [Category(nameof(Constants.PropertyCategory.Appearance))]
        public bool ShowFocus { get; set; }

        [Category(nameof(Constants.PropertyCategory.Time))]
        public int TimerResolution
        {
            get => this.timerResolution;
            set
            {
                if (this.timerResolution != value)
                {
                    this.timerResolution = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimerResolution)));
                }
            }
        }

        [Category(nameof(Constants.PropertyCategory.Time))]
        public int EmissionInterval { get; set; }
    }
}