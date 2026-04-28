namespace ei8.Prototypes.HelloWorm
{
    internal class Odor : IMovable, IElliptical
    {
        public Odor()
        {
        }

        public float Direction { get; set; }
        public int Speed { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }

        public event EventHandler<MovingEventArgs>? Moving;
        public event EventHandler<CollidedEventArgs>? Collided;

        public void Collide(CollisionInfo info) => this.Collided?.Invoke(this, new CollidedEventArgs(info));

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);
    }
}
