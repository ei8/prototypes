using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal class Worm : IMovable, IComposite, IElliptical
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

        public void Collide(IPhysical target) => this.Collided?.Invoke(this, new CollidedEventArgs(target));

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);
    }
}
