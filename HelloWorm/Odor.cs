using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    internal class Odor : IMovable, IElliptical
    {
        private readonly Timer movementTriggerTimer;

        public Odor()
        {
            this.movementTriggerTimer = new Timer(this.Move, null, 0, Constants.MovementTriggerTimerPeriod);
        }

        public float Direction { get; set; }
        public int Speed { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }

        public event EventHandler<MovingEventArgs>? Moving;
        public event EventHandler<CollidedEventArgs>? Collided;

        public void Collide(CollisionInfo info) => this.Collided?.Invoke(this, new CollidedEventArgs(info.Target));

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);

        public void Stop()
        {
            this.movementTriggerTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }
}
