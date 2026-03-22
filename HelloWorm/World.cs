using System.Diagnostics;

namespace HelloWorm
{
    public class World : IRectangular, IComposite
    {
        private IList<IPhysical> components;

        public World()
        {
            this.Location = Point.Empty;
            this.Size = Size.Empty;
            this.components = [];
        }

        public void Add(IPhysical @object)
        {
            this.AddCore(@object);
        }

        private void AddCore(IPhysical @object)
        {
            this.components.Add(@object);

            if (@object is IMovable movable)
            {
                movable.Moving += this.Movable_Moving;
                movable.Collided += this.Movable_Collided;
            }
        }

        public void Remove(IPhysical @object)
        {
            this.RemoveCore(@object);
        }

        private void RemoveCore(IPhysical @object)
        {
            if (this.components.Contains(@object))
            {
                try
                {
                    this.components.Remove(@object);

                    if (@object is IMovable movable)
                    {
                        movable.Moving -= this.Movable_Moving;
                        movable.Collided -= this.Movable_Collided;
                    }
                }
                catch (IndexOutOfRangeException ioex)
                {
                    Debug.WriteLine("Tried re-removing item.");
                }
            }
        }

        public void Remove<T>(IEmitter<T> emitter) where T : IPhysical
        {
            this.RemoveCore(emitter);

            emitter.Emitted -= this.Emitter_Emitted;
        }

        private void Movable_Collided(object? sender, CollidedEventArgs e)
        {
            if (
                sender is Odor odor &&
                e.Target is World
            )
            {
                this.Remove(odor);
            }
        }

        private void Movable_Moving(object? sender, MovingEventArgs e)
        {
            if (
                sender is Odor odor &&
                !this.GetRectangle().Contains(e.NewLocation)
            )
            {
                e.CollisionInfo = new()
                {
                    CollisionTarget = this,
                    CollisionSource = odor
                };
            }
            else if (sender is Worm worm)
            {
                var nose = worm.Components.OfType<Nose>().Single();
                var firstSector = nose.GetCollisionSector(
                    (p) => !this.GetRectangle().Contains(p),
                    (sector) =>
                    {
                        int sectorId = nose.GetSectorId(sector);
                        // if one of the rear sectors 
                        bool exclude = sectorId > 2 && sectorId < 7;

                        /*
                        // is southBound?
                        bool southBound = worm.IsDirectionBound(
                            // ... all positive directions between 0 and 0.5 of 360 (eg. 1 to 180 etc)
                            dr => dr > 0 && dr < 0.5,
                            // ... all negative directions between 0.5 and 1 of 360 (eg. -181 to -360 etc.)
                            dr => dr > 0.5 && dr < 1
                        );

                        // is northBound?
                        bool northBound = worm.IsDirectionBound(
                            // ... all positive directions between 0.5 and 1 of 360 (eg. 181 to 360 etc)
                            dr => dr > 0.5 && dr < 1,
                            // ... all negative directions between 0.0 and 0.5 of 360 (eg. -1 to -180 etc.)
                            dr => dr > 0 && dr < 0.5
                        );

                        var eastEvaluator = new Func<float, bool>(dr => (dr > 0 && dr < 0.25) || (dr > 0.75 && dr < 1));
                        // is eastBound?
                        bool eastBound = worm.IsDirectionBound(
                            // ... all positive directions between 0 and 0.25 or between 0.75 and 1 of 360
                            eastEvaluator,
                            // ... all negative directions between 0.0 and 0.25 or between 0.75 and 1 of 360
                            eastEvaluator
                        );

                        var westEvaluator = new Func<float, bool>(dr => dr > 0.25 && dr < 0.75);
                        // is westBound?
                        bool westBound = worm.IsDirectionBound(
                            // ... all positive directions between 0.25 and 0.75
                            westEvaluator,
                            // ... all negative directions between 0.25 and 0.75
                            westEvaluator
                        );

                        // TODO: applicable only if left/right walls 
                        // ----------------------
                        // or if southeastbound and sector is 1 or 2
                        exclude |= southBound && eastBound && sectorId < 3;

                        // or if northeastbound and sector is 7 or 8
                        exclude |= northBound && eastBound && sectorId > 6;

                        // or if southwestbound and sector is 7 or 8
                        exclude |= southBound && westBound && sectorId > 6;

                        // or if northwestbound and sector is 1 or 2
                        exclude |= northBound && westBound && sectorId < 3;
                        // TODO: ----------------
                        */

                        return exclude;
                    },
                    (angle) => angle + worm.Direction,
                    (location) => location.Add(worm.Location)
                );
                if (firstSector != null)
                {
                    e.CollisionInfo = new()
                    {
                        CollisionTarget = this,
                        CollisionSource = firstSector
                    };
                }
            }
        }

        public void Add<T>(IEmitter<T> emitter) where T : IPhysical
        {
            this.AddCore(emitter);

            emitter.Emitted += this.Emitter_Emitted;
        }

        private void Emitter_Emitted<T>(object? sender, EmittedEventArgs<T> e) where T: IPhysical
        {
            foreach (var em in e.Emission)
                this.Add(em);
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public IEnumerable<IPhysical> Components { get => this.components; set => throw new NotSupportedException(); }
    }
}