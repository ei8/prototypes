using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal interface IMovable : ILocated
    {
        float Direction { get; set; }

        int Speed { get; set; }

        void OnMoving(MovingEventArgs e);

        event EventHandler<MovingEventArgs> Moving;

        void Collide(CollisionInfo info);

        event EventHandler<CollidedEventArgs> Collided;
    }
}
