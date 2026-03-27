using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ei8.Prototypes.HelloWorm
{
    internal static class ExtensionMethods
    {
        #region Physical
        internal static CollisionInfo? GetCollisionSector(
            this IRectangularComposite rectangularComposite,
            Func<SectorPointInfo, CollisionInfo?> collisionEvaluator,
            Func<float, float> angleTranslator,
            Func<Point, Point> locationTranslator
        )
        {
            CollisionInfo? result = null;
            var swps = rectangularComposite.GetSectorsPoints(
                angleTranslator,
                locationTranslator
             );

            // get sectors with the most number of collision points
            IEnumerable<CollisionInfo?> collisionsPerSector = swps
                .Select(swp => collisionEvaluator(swp));

            var collidedSectors = collisionsPerSector.Where(t =>  t != null && t.Count > 0).Select(e => e!);
            if (collidedSectors.Any())
            {
                var max = collidedSectors.DefaultIfEmpty().Max(t => t?.Count);
                if (max.HasValue && max.Value > 0)
                {
                    var maxSectors = collidedSectors.Where(ct => ct.Count == max);

#if DEBUG
                    if (maxSectors.Count() > 1)
                        Debug.WriteLine("Multiple sectors collided equally: " + string.Join(";", maxSectors.Select(mt => ((Sector)mt.Source).StartAngle)));
#endif

                    result = maxSectors.FirstOrDefault();
                }
            }

            return result;
        }

        internal static CollisionInfo? GetCollisionInfo(this SectorPointInfo sp, IEnumerable<IRectangular> components)
        {
            CollisionInfo? result = null;

            foreach (var c in components)
            {
                if (sp.Path.IsVisible(c.GetRectangle().Location))
                {
                    result = new(
                        c,
                        sp.Sector,
                        1
                    );
                    break;
                }
            }

            return result;
        }

        internal static bool IsDirectionBound(this IMovable movable, Func<float, bool> positiveRemainderEvaluator, Func<float, bool> negativeRemainderEvaluator)
        {
            bool result = false;

                float directionRemainder = Math.Abs(movable.Direction % 360f) / 360f;
                result =
                    (movable.Direction > 0 && positiveRemainderEvaluator(directionRemainder)) ||
                    (movable.Direction < 0 && negativeRemainderEvaluator(directionRemainder)); ;

            return result;
        }

        #region Sample Usage of IsDirectionBound

        /*
        // is southBound?
        bool southBound = worm.IsDirectionBound(
            // ... all positive directions between 0 and 0.5 of 360 (eg. 1 to 180 etc)
            dr => dr > 0 && dr < 0.5,
            // ... all negative directions between 0.5 and 1 of 360 (eg. -181 to -360 etc.)
            dr => dr > 0.5 && dr < 1
        );

        // is northBound?
        bool northBound = worm.IsDirectionBound(
            // ... all positive directions between 0.5 and 1 of 360 (eg. 181 to 360 etc)
            dr => dr > 0.5 && dr < 1,
            // ... all negative directions between 0.0 and 0.5 of 360 (eg. -1 to -180 etc.)
            dr => dr > 0 && dr < 0.5
        );

        var eastEvaluator = new Func<float, bool>(dr => (dr > 0 && dr < 0.25) || (dr > 0.75 && dr < 1));
        // is eastBound?
        bool eastBound = worm.IsDirectionBound(
            // ... all positive directions between 0 and 0.25 or between 0.75 and 1 of 360
            eastEvaluator,
            // ... all negative directions between 0.0 and 0.25 or between 0.75 and 1 of 360
            eastEvaluator
        );

        var westEvaluator = new Func<float, bool>(dr => dr > 0.25 && dr < 0.75);
        // is westBound?
        bool westBound = worm.IsDirectionBound(
            // ... all positive directions between 0.25 and 0.75
            westEvaluator,
            // ... all negative directions between 0.25 and 0.75
            westEvaluator
        );

        // applicable only if left/right walls 
        // ----------------------
        // or if southeastbound and sector is 1 or 2
        exclude |= southBound && eastBound && sectorId < 3;

        // or if northeastbound and sector is 7 or 8
        exclude |= northBound && eastBound && sectorId > 6;

        // or if southwestbound and sector is 7 or 8
        exclude |= southBound && westBound && sectorId > 6;

        // or if northwestbound and sector is 1 or 2
        exclude |= northBound && westBound && sectorId < 3;
        // ----------------
        */

        #endregion

        internal static int GetSectorId(this IRectangularComposite rectangularComposite, ISectoral sectoral) =>
            rectangularComposite.Components.TakeWhile(s => s != sectoral).Count() + 1;

        internal static void Move(this IMovable movable, object? state)
        {
            ArgumentNullException.ThrowIfNull(movable);

            Point newLocation = movable.Location.GetLocationByHypotenuseAndAngle(movable.Direction, movable.Speed);

            var mea = new MovingEventArgs(newLocation);
            movable.OnMoving(mea);

            if (mea.CollisionInfo != null)
                movable.Collide(mea.CollisionInfo);
            else
                movable.Location = newLocation;
        }

        internal static Rectangle GetRectangle(this IRectangular rectangle) => new(rectangle.Location, rectangle.Size);

        internal static IEnumerable<SectorPointInfo> GetSectorsPoints(
            this IRectangularComposite parent,
            Func<float, float> angleTranslator,
            Func<Point, Point> locationTranslator
        )
        {
            var results = new List<SectorPointInfo>();
            var sectors = parent.Components.OfType<IRectangleBoundSectoral>();
            foreach (var s in sectors)
            {
                Rectangle parentRectangle = parent.GetRectangle();
                var sectorPath = new GraphicsPath();
                var sectorRectangle = new Rectangle(locationTranslator(parentRectangle.Location), parentRectangle.Size);
                sectorPath.AddPie(sectorRectangle, angleTranslator(s.StartAngle), s.SweepAngle);

                var ps = new List<Point>();
                for (int i = (int)s.StartAngle; i < s.StartAngle - 1 + s.SweepAngle; i++)
                {
                    var hp = parentRectangle.GetHypotenusePoint(angleTranslator(i));
                    hp = locationTranslator(hp);
                    ps.Add(hp);
                }

                results.Add(new(s, sectorPath, ps));
            }

            return results;
        }
        #endregion

        #region Drawing
        static Brush GetRandomBrushColor(int maxValue)
        {
            var random = new Random();
            var rv = random.Next(maxValue);
            var rb = Brushes.Green;
            switch (rv)
            {
                case 1:
                    rb = Brushes.Red;
                    break;
                case 2:
                    rb = Brushes.Blue;
                    break;
            }

            return rb;
        }

        internal static Point GetLocationByHypotenuseAndAngle(this Point original, float angle, double hypotenuse)
        {
            // 0 degrees is East
            var rad = angle * (Math.PI / 180d);

            var newLocation = new Point(
                original.X + Convert.ToInt32(Math.Cos(rad) * hypotenuse),
                original.Y + Convert.ToInt32(Math.Sin(rad) * hypotenuse)
            );

            return newLocation;
        }

        internal static Point GetHypotenusePoint(this Rectangle rectangle, float angle, int offset = 0)
        {
            var halfOfRectangle = rectangle.Size / 2;
            var centerOfRectangle = rectangle.Location.Add(new Point(halfOfRectangle.Width, halfOfRectangle.Height));
            Point hypoPoint = centerOfRectangle.GetLocationByHypotenuseAndAngle(
                angle,
                halfOfRectangle.Width + offset
            );
            return hypoPoint;
        }

        internal static void DrawCenteredStringAtPoint(this Graphics g, string text, Font font, Brush brush, PointF centerPoint)
        {
            // 1. Define a rectangle around the center point. 
            //    The size needs to be large enough to contain the string.
            //    A common approach is to use MeasureString to estimate the required size.
            SizeF stringSize = g.MeasureString(text, font);

            // Calculate the top-left corner of the rectangle to center it on the centerPoint
            float rectX = centerPoint.X - (stringSize.Width / 2);
            float rectY = centerPoint.Y - (stringSize.Height / 2);
            RectangleF layoutRect = new RectangleF(rectX, rectY, stringSize.Width, stringSize.Height);

            // 2. Create a StringFormat object and set the alignment properties
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;      // Horizontal center
            sf.LineAlignment = StringAlignment.Center;  // Vertical center

            // 3. Draw the string within the defined rectangle using the StringFormat
            g.DrawString(text, font, brush, layoutRect, sf);
        }

        internal static void DrawGrid(this Graphics g, Size worldSize, int gap)
        {
            using Pen gridPen = new Pen(Color.Gray);
            for (int i = gap; i <= worldSize.Width; i += gap)
                g.DrawLine(gridPen, new Point(i, 0), new Point(i, worldSize.Height));

            for (int i = gap; i <= worldSize.Height; i += gap)
                g.DrawLine(gridPen, new Point(0, i), new Point(worldSize.Width, i));
        }

        internal static void DrawRectangleBound(
            this Graphics g, 
            IRectangleBound rectangleBound, 
            IRectangularComposite parent, 
            Func<float, float> angleTranslator,
            params Func<Point, Point>[] locationTranslators
        )
        {
            if (rectangleBound is ISectoral sectoral)
            {
                using var boundedPath = new GraphicsPath();
                var parentRectangle = parent.GetRectangle();
                parentRectangle.Location = parentRectangle.Location.Translate(locationTranslators);

                Brush rb = ExtensionMethods.GetRandomBrushColor(2);
                g.FillPie(rb, parentRectangle, angleTranslator(sectoral.StartAngle), sectoral.SweepAngle);

                if (Constants.ShouldDrawSectorIds)
                {
                    Point hypoPoint = parentRectangle.GetHypotenusePoint(
                        angleTranslator(sectoral.StartAngle - 1 + (sectoral.SweepAngle / 2)),
                        10
                    );
                    g.DrawCenteredStringAtPoint(
                        parent.GetSectorId(sectoral).ToString(),
                        new Font("Arial", 8, FontStyle.Regular),
                        Brushes.Blue,
                        hypoPoint
                    );
                }
            }
        }

        internal static void DrawRectangular(
            this Graphics g, 
            IRectangular rectangular, 
            Pen pen,
            Func<float, float> angleTranslator,
            params Func<Point, Point>[] locationTranslators
        )
        {
            g.DrawRectangle(
                rectangular,
                pen,
                (irect) => new(
                    irect.Location.Translate(locationTranslators),
                    irect.Size
                )
            );

            if (rectangular is IRectangularComposite rectangularComposite)
                g.DrawRectangularComposite(rectangularComposite, pen, angleTranslator, locationTranslators);
        }

        internal static void DrawRectangularComposite(
            this Graphics g, 
            IRectangularComposite rectangularComposite, 
            Pen pen, 
            Func<float, float> angleTranslator,
            params Func<Point, Point>[] locationTranslators
        )
        {
            foreach (var component in rectangularComposite.Components)
            {
                if (component is IRectangular rectangularComponent)
                {
                    g.DrawRectangular(
                        rectangularComponent,
                        pen,
                        angleTranslator,
                        (pt) => pt.Translate(
                            locationTranslators.Prepend((point) => point.Add(rectangularComposite.Location))
                        )
                    );
                }
                else if (component is IRectangleBound rectangleBound)
                {
                    g.DrawRectangleBound(
                        rectangleBound,
                        rectangularComposite, 
                        angleTranslator,
                        locationTranslators
                    );
                }
            }
        }

        internal static void DrawRectangle(
            this Graphics g,
            IRectangular rectangular,
            Pen pen
        )
            => DrawRectangle(g, rectangular, pen, irect => irect.GetRectangle());

        internal static void DrawRectangle(
            this Graphics g,
            IRectangular rectangular,
            Pen pen,
            Func<IRectangular, Rectangle> rectangleRetriever
        )
        {
            var rect = rectangleRetriever(rectangular);
            if (rectangular is IElliptical elliptical)
                g.DrawEllipse(pen, rect);
            else
                g.DrawRectangle(pen, rect);
        }

        internal static Point Translate(
            this Point original,
            IEnumerable<Func<Point, Point>> translators
        ) => translators.Any() ? translators.Select(t => original = t(original)).Last() : original;

        internal static Point Add(this Point firstPoint, Point secondPoint) =>
            new Point(firstPoint.X + secondPoint.X, firstPoint.Y + secondPoint.Y);
        #endregion
    }
}
