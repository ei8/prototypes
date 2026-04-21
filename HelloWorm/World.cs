using neurUL.Common.Domain.Model;
using System.Collections.Immutable;
using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    public class World : IRectangularComposite
    {
        public event EventHandler Added;
        public event EventHandler Removed;

        private IImmutableList<IPhysical> components;
        private readonly Timer movementTriggerTimer;
        private readonly Timer emissionTriggerTimer;

        private object IndexLock { get; } = new();

        public World()
        {
            this.Location = new Point(0, 0);
            this.Size = Size.Empty;
            this.components = ImmutableList<IPhysical>.Empty;
            this.Regenerate = true;

            this.movementTriggerTimer = new Timer(this.WrapMove, null, 0, Constants.MovementTriggerTimerPeriod);
            this.emissionTriggerTimer = new Timer(this.Emit, this, 0, Constants.EmissionTriggerTimerPeriod);
        }

        private void Emit(object? state)
        {
            foreach (var e in this.components.OfType<IEmitter>())
            {
                if (e is IPerishable perishable)
                    perishable.Life--;

                e.Emit();
            }
        }

        private void WrapMove(object? state)
        {
            foreach (var m in this.components.OfType<IMovable>())
            {
                if (m is IPerishable perishable)
                    perishable.Life--;

                m.Move(state);
            }
        }

        public void Add(IPhysical @object)
        {
            lock (this.IndexLock)
            {
                this.components = this.components.Add(@object);
            }

            if (@object is IMovable movable)
            {
                movable.Moving += this.Movable_Moving;
                movable.Collided += this.Movable_Collided;
            }

            if (@object is IEmitter emitter)
                emitter.Emitted += this.Emitter_Emitted;

            this.Added?.Invoke(this, EventArgs.Empty);
        }

        public void Remove(IPhysical @object)
        {
            lock (this.IndexLock)
            {
                if (this.components.Contains(@object))
                {
                    this.components = this.components.Remove(@object);
                }
            }

            if (@object is IMovable movable)
            {
                movable.Moving -= this.Movable_Moving;
                movable.Collided -= this.Movable_Collided;
            }

            if (@object is IRegenerative regenerative && this.Regenerate)
            {
                var regen = regenerative.Create(this.Size);
                this.Add(regen);

                if (regenerative is INeurULized neurULized)
                {
                    AssertionConcern.AssertArgumentValid(n => n.Network != null && n.MirrorConfigs != null, neurULized, "neurULized object should be initialized adequately.", nameof(@object));

                    ((INeurULized)regen).Initialize(neurULized.Network!, neurULized.MirrorConfigs!);
                }
            }

            if (@object is IEmitter emitter)
                emitter.Emitted -= this.Emitter_Emitted;

            this.Removed?.Invoke(this, EventArgs.Empty);
        }

        private void Movable_Collided(object? sender, CollidedEventArgs e)
        {
            if (sender is Odor odor && e.Target is World)
            {
                this.Remove(odor);
            }
            else if (
                sender is Worm worm &&
                e.Target is Odor odor2
            )
            {
                this.Remove(odor2);
            }
            else if (
                sender is Worm worm2 &&
                e.Target is Food food
            )
            {
                this.Remove(food);
                worm2.Grow();
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
                        //  ...world
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

            if (sender is IPerishable perishable && perishable.Life < 0)
            {
                this.Remove(perishable);
            }
        }

        private void Emitter_Emitted(object? sender, EmittedEventArgs e)
        {
            foreach (var em in e.Emission)
                this.Add(em);

            if (sender is IPerishable perishable && perishable.Life < 0)
            {
                this.Remove(perishable);
            }
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public IEnumerable<IPhysical> Components { get => this.components; set => throw new NotSupportedException(); }
        public bool Regenerate { get; set; }
    }
}