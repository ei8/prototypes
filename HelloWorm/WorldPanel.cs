using System.Drawing.Drawing2D;

namespace HelloWorm
{
    internal class WorldPanel : Panel
    {
        internal const float AngleOffset = 90;
        
        private readonly World world;

        public bool ShouldDrawGrid { get; set; } = false;
        public bool ShouldDrawRectangularRectangles { get; set; } = false;
        public bool ShouldDrawRectangularDirectedRectangles { get; set; } = false;

        public WorldPanel(World world)
        {
            this.DoubleBuffered = true;
            this.world = world;

            this.ShouldDrawGrid = true;
            this.ShouldDrawRectangularRectangles = true;
            this.ShouldDrawRectangularDirectedRectangles = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using Pen worldPen = new Pen(Color.Blue);

            WorldPanel.DrawRectangleCore(g, this.world, worldPen);

            if (this.ShouldDrawGrid)
                WorldPanel.DrawGrid(this.world.Size, g);

            var pos = this.world.Components.ToArray();
            foreach (var p in pos)
            {
                using SolidBrush b = new(Color.Black);
                using Pen pen = new(b);

                if (p is IRectangular rc)
                {
                    if (this.ShouldDrawRectangularRectangles)
                    {
                        using Pen rectPen = new Pen(Color.Red);
                        g.DrawRectangle(rectPen, rc.GetRectangle());
                    }

                    if (p is IMovable pm)
                    {
                        float ellipseCenterX = 0;
                        float ellipseCenterY = 0;

                        if (pm is Worm wm && wm.Components.OfType<Nose>().Any())
                        {
                            ellipseCenterX = wm.Location.X + (rc.Size.Width / 2);
                            ellipseCenterY = wm.Location.Y + (wm.Components.OfType<Nose>().Single().Size.Height / 2);
                        }
                        else
                        {
                            ellipseCenterX = rc.Location.X + (rc.Size.Width / 2);
                            ellipseCenterY = rc.Location.Y + (rc.Size.Height / 2);
                        }

                        // Save the current graphics state
                        System.Drawing.Drawing2D.Matrix originalMatrix = e.Graphics.Transform;

                        // Translate the origin to the center of the ellipse, then rotate
                        g.TranslateTransform(ellipseCenterX, ellipseCenterY);
                        g.RotateTransform(pm.Direction + WorldPanel.AngleOffset);
                        // Translate back to the original top-left corner position relative to the new origin
                        g.TranslateTransform(-ellipseCenterX, -ellipseCenterY);

                        // Draw the ellipse
                        WorldPanel.DrawRectangular(g, rc, pen);

                        // Reset the graphics transform to the original state for other drawings
                        g.Transform = originalMatrix;
                    }
                    else
                        WorldPanel.DrawRectangular(g, rc, pen);
                }
            }

            base.OnPaint(e);
        }

        private static void DrawGrid(Size worldSize, Graphics g)
        {
            using Pen gridPen = new Pen(Color.Gray);
            for (int i = 50; i <= worldSize.Width; i+=50)
                g.DrawLine(gridPen, new Point(i, 0), new Point(i, worldSize.Height));

            for (int i = 50; i <= worldSize.Height; i+=50)
                g.DrawLine(gridPen, new Point(0, i), new Point(worldSize.Width, i));
        }

        private static void DrawRectangleBounded(Graphics g, IRectangleBounded rectangleBounded, Pen pen, Rectangle parentRectangle)
        {
            var random = new Random();
            if (rectangleBounded is ISectoral sectoral)
            {
                using var boundedPath = new GraphicsPath();

                var rv = random.Next(2);
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
                // boundedPath.AddPie(parentRectangle, sectoral.StartAngle, sectoral.EndAngle - 90);
                g.FillPie(rb, parentRectangle, sectoral.StartAngle - WorldPanel.AngleOffset, sectoral.SweepAngle);

                if (rectangleBounded is Sector se)
                {
                    var halfOfParent = parentRectangle.Size / 2;
                    var centerOfParent = parentRectangle.Location.Add(new Point(halfOfParent.Width, halfOfParent.Height));
                    g.DrawString(
                        se.Name,
                        new Font("Arial", 12, FontStyle.Regular),
                        Brushes.Blue,
                        centerOfParent.GetLocationByHypotenuseAndAngle(
                            sectoral.StartAngle /* - 1 + (sectoral.SweepAngle / 2) */ - WorldPanel.AngleOffset,
                            halfOfParent.Width + 15
                        )
                    );
                }
            }

            // TODO:
            // WorldPanel.DrawCompositeSectoral(g, rectangleBounded, pen);
        }

        private static void DrawRectangular(Graphics g, IRectangular rectangular, Pen pen, Func<Point, Point>? locationUpdater = null)
        {
            WorldPanel.DrawRectangleCore(
                g,
                rectangular,
                pen,
                (irect) => new(
                    irect.Location.UseUpdaterIfExists(locationUpdater),
                    irect.Size
                )
            );

            WorldPanel.DrawRectangularComposite(g, rectangular, pen, locationUpdater);
        }

        private static void DrawRectangularComposite(Graphics g, IRectangular rectangular, Pen pen, Func<Point, Point>? locationUpdater = null)
        {
            if (rectangular is IComposite rectangularComposite)
            {
                foreach (var component in rectangularComposite.Components)
                {
                    if (component is IRectangular rectangularComponent)
                    {
                        WorldPanel.DrawRectangular(
                            g,
                            rectangularComponent,
                            pen,
                            (pt) => pt.UseUpdaterIfExists(
                                (point) => point.Add(rectangular.Location), 
                                locationUpdater
                            )
                        );
                    }
                    else if (component is IRectangleBounded rectangleBounded)
                    {
                        var bounds = rectangular.GetRectangle();
                        bounds.Location = bounds.Location.UseUpdaterIfExists(locationUpdater);
                        WorldPanel.DrawRectangleBounded(
                            g, 
                            rectangleBounded, 
                            pen, 
                            bounds
                        );
                    }
                }
            }
        }

        private static void DrawRectangleCore(
            Graphics g,
            IRectangular rectangular,
            Pen pen
        )
            => DrawRectangleCore(g, rectangular, pen, irect => irect.GetRectangle());

        private static void DrawRectangleCore(
            Graphics g, 
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

        public World World => this.world;
    }
}
