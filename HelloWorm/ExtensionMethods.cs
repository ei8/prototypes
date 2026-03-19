namespace HelloWorm
{
    internal static class ExtensionMethods
    {
        internal static void Move(this IMovable movable, object? state)
        {
            ArgumentNullException.ThrowIfNull(movable);

            Point newLocation = movable.Location.GetLocationByHypotenuseAndAngle(movable.Direction, movable.Speed);

            var mea = new MovingEventArgs(newLocation);
            movable.OnMoving(mea);

            if (mea.CollisionDetected && mea.CollisionTarget != null)
                movable.Collide(mea.CollisionTarget);

            movable.Location = newLocation;
        }

        internal static Point GetLocationByHypotenuseAndAngle(this Point original, float angle, int hypotenuse)
        {
            // 0 degrees is East
            var rad = angle * (Math.PI / 180d);

            var newLocation = new Point(
                original.X + Convert.ToInt32(Math.Cos(rad) * Convert.ToDouble(hypotenuse)),
                original.Y + Convert.ToInt32(Math.Sin(rad) * Convert.ToDouble(hypotenuse))
            );

            return newLocation;
        }

        internal static Point UseUpdaterIfExists(
            this Point original,
            Func<Point, Point>? locationUpdater
        ) => original.UseUpdaterIfExists(
            (orig) => orig,
            locationUpdater
        );

        internal static Point UseUpdaterIfExists(
            this Point original, 
            Func<Point, Point> originalProcessor, 
            Func<Point, Point>? locationUpdater
        ) => locationUpdater != null ?
            locationUpdater(originalProcessor(original)) :
            originalProcessor(original);

        internal static Point Add(this Point firstPoint, Point secondPoint) =>
            new Point(firstPoint.X + secondPoint.X, firstPoint.Y + secondPoint.Y);

        internal static Rectangle GetRectangle(this IRectangular rectangle) => new(rectangle.Location, rectangle.Size);

        internal static Rectangle GetRectangle(this IElliptical rectangle) => new(rectangle.Location, rectangle.Size);
    }
}
