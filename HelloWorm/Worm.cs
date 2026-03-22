using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal class Worm : IMovable, IRectangularComposite, IElliptical
    {
        private readonly Timer movementTriggerTimer;

        public Worm()
        {
            this.movementTriggerTimer = new Timer(this.Move, null, 0, Constants.MovementTriggerTimerPeriod);
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public float Direction { get; set; }
        public int Speed { get; set; }
        public required IEnumerable<IPhysical> Components { get; set; }

        public event EventHandler<MovingEventArgs>? Moving;
        public event EventHandler<CollidedEventArgs>? Collided;

        public void Collide(CollisionInfo info)
        {
            if (info.Target is World && info.Source is ISectoral sector)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector);
                switch (sectorId)
                {
                    case 8:
                    case 1:
                        this.Direction += 45f * (sectorId == 8 ? 1 : -1);
                        break;
                    case 7:
                    case 2:
                        this.Direction += 22.5f * (sectorId == 7 ? 1 : -1);
                        break;
                }
            }
            else if (info.Target is Odor && info.Source is ISectoral sector2)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector2);
                switch (sectorId)
                {
                    case 1:
                    case 8:
                        this.Direction += 22.5f * (sectorId == 1 ? 1 : -1);
                        break;
                    case 2:
                    case 7:
                        this.Direction += 45f * (sectorId == 2 ? 1 : -1);
                        break;
                    case 3:
                    case 6:
                        this.Direction += 67.5f * (sectorId == 3 ? 1 : -1);
                        break;
                    case 4:
                    case 5:
                        this.Direction += 90f * (sectorId == 4 ? 1 : -1);
                        break;
                }
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info.Target));
        }

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);
    }
}
