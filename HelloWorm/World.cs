using neurUL.Common.Domain.Model;
using System.Collections.Immutable;

namespace ei8.Prototypes.HelloWorm
{
    public class World : IRectangularComposite
    {
        public event EventHandler Added;

        private IImmutableList<IPhysical> components;

        public World()
        {
            this.Location = Point.Empty;
            this.Size = Size.Empty;
            this.components = ImmutableList<IPhysical>.Empty;
            this.Regenerate = true;
        }

        public void Add(IPhysical @object)
        {
            this.AddCore(@object);
        }

        private void AddCore(IPhysical @object)
        {
            this.components = this.components.Add(@object);

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
            if (this.components.Contains(@object))
            {
                this.components = this.components.Remove(@object);

                if (@object is IMovable movable)
                {
                    movable.Moving -= this.Movable_Moving;
                    movable.Collided -= this.Movable_Collided;
                }
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
        }

        private void Movable_Collided(object? sender, CollidedEventArgs e)
        {
            if (
                sender is Odor odor &&
                e.Target is World
            )
            {
                odor.Stop();
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