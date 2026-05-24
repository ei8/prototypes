using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public class Dish : IRectangularComposite, ITemporal, INamed
    {
        public enum ModeValue
        {
            Forage,
            RotationTest
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        private IImmutableList<IComponent> components;
        private DateTime lastEmission;

        private bool isPlaying;
        private ModeValue mode;

        private string name;

        private int timerResolution;
        private readonly IServiceProvider serviceProvider;

        private object IndexLock { get; } = new();

        public Dish(IServiceProvider serviceProvider)
        {
            this.Location = new Point(0, 0);
            this.Size = Size.Empty;
            this.components = ImmutableList<IComponent>.Empty;
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
                                new Odor() { Parent = this },
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

        public void Add(IComponent component)
        {
            lock (this.IndexLock)
            {
                this.components = this.components.Add(component);
            }

            if (component is not Odor)
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.GetFullName()} - {component.GetFullName()} added."));

            if (component is IMovable movable)
            {
                movable.Moving += this.Movable_Moving;
                movable.Collided += this.Movable_Collided;
            }

            if (component is IEmitter emitter)
                emitter.Emitted += this.Emitter_Emitted;

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    component
                )
            );
        }

        public void Remove(IComponent component)
        {
            lock (this.IndexLock)
            {
                if (this.components.Contains(component))
                {
                    this.components = this.components.Remove(component);
                }
            }

            if (component is not Odor)
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.GetFullName()} - {component.GetFullName()} removed."));

            if (component is IMovable movable)
            {
                movable.Moving -= this.Movable_Moving;
                movable.Collided -= this.Movable_Collided;
            }

            if (component is IEmitter emitter)
                emitter.Emitted -= this.Emitter_Emitted;

            if (
                component is IRegenerative original &&
                this.Regenerate
            )
            {
                Dish.logger.Info(new LogMessageGenerator(() => $"{this.GetFullName()} - {component.GetFullName()} regenerating..."));
                var regen = (IRegenerative)this.serviceProvider.GetRequiredService(original.GetType());
                regen.Initialize(regen.Name, this);
                regen.Inherit(original);
                this.Add(regen);
            }

            this.NotifyCollectionChanged?.Invoke(
                this, 
                new NotifyCollectionChangedEventArgs(
                     NotifyCollectionChangedAction.Remove,
                     component
                )
            );
        }

        private void Movable_Collided(object? sender, CollidedEventArgs e)
        {
            if (e.Info.Cause is Odor odor && e.Info.Receiver is Dish)
            {
                this.Remove(odor);
            }
            else if (
                e.Info.Cause is ISectoral &&
                e.Info.Receiver is Odor odor2
            )
            {
                this.Remove(odor2);
            }
            else if (
                // TODO: create responseParser for Grow
                // e.Info.Cause is ISectoral &&
                sender is Worm worm &&
                e.Info.Receiver is Food food
            )
            {
                this.Remove(food);
                worm.Grow();
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

                        var sectorId = int.Parse(sp.Sector.Name.Substring(typeof(Sector).Name.Length));
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
        public IEnumerable<IComponent> Components => this.components; 
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

                    Dish.logger.Info(new LogMessageGenerator(() => $"{this.GetFullName()} - {(this.IsPlaying ? "played." : "paused.")}"));
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
        public required IComposite Parent { get; set; }
    }
}