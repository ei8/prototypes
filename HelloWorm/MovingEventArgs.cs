namespace HelloWorm
{
    internal class MovingEventArgs : EventArgs
    {
        public CollisionInfo? CollisionInfo { get; set; }
        public Point NewLocation { get; }

        public MovingEventArgs(Point newLocation)
        {
            this.CollisionInfo = null;
            this.NewLocation = newLocation;
        }
    }
}
