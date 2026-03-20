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
            this.components.Remove(@object);

            if (@object is IMovable movable)
            {
                movable.Moving -= this.Movable_Moving;
                movable.Collided -= this.Movable_Collided;
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

                var swps = nose.GetSectorsWithPoints(
                    (angle) => angle + worm.Direction,
                    (location) => location.Add(worm.Location)
                 );

                ISectoral? firstSector = null;
                if ((firstSector = swps.FirstOrDefault(swp => swp.Points.Any(p => !this.GetRectangle().Contains(p))).Sector) != null)
                {
                    var sector = nose.GetSectorId(firstSector);
                    if (sector > 6 || sector < 3)
                    {
                        e.CollisionInfo = new()
                        {
                            CollisionTarget = this,
                            CollisionSource = firstSector
                        };
                    }
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