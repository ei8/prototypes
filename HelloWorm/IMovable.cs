using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    internal interface IMovable : ILocated
    {
        float Direction { get; set; }

        int Speed { get; set; }

        void Stop();

        void OnMoving(MovingEventArgs e);

        event EventHandler<MovingEventArgs> Moving;

        void Collide(CollisionInfo info);

        event EventHandler<CollidedEventArgs> Collided;
    }
}
