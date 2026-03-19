namespace HelloWorm
{
    internal class MovingEventArgs : EventArgs
    {
        public bool CollisionDetected { get; set; }
        public IPhysical? CollisionTarget { get; set; }
        public Point NewLocation { get; }

        public MovingEventArgs(Point newLocation)
        {
            this.CollisionDetected = false;
            this.CollisionTarget = null;
            this.NewLocation = newLocation;
        }
    }
}
