namespace HelloWorm
{
    internal class WorldPanel : Panel
    {
        private const float AngleOffset = 90;
        
        private readonly World world;

        

        public WorldPanel(World world)
        {
            this.DoubleBuffered = true;
            this.world = world;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using Pen worldPen = new Pen(Color.Blue);

            g.DrawRectangle(this.world, worldPen);

            if (Constants.ShouldDrawGrid)
                g.DrawGrid(this.world.Size, 50);

            foreach (var p in this.world.Components)
            {
                using SolidBrush b = new(Color.Black);
                using Pen pen = new(b);

                if (p is IRectangular rc)
                {
                    if (Constants.ShouldDrawRectangularRectangles)
                    {
                        using Pen rectPen = new Pen(Color.Red);
                        g.DrawRectangle(rectPen, rc.GetRectangle());
                    }

                    var angleTranslator = new Func<float, float>((a) => a - WorldPanel.AngleOffset);
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

                        if (Constants.ShouldDrawDirection)
                        {
                            Point hypoPoint = rc.GetRectangle().GetHypotenusePoint(180 - WorldPanel.AngleOffset, 10);
                            g.DrawCenteredStringAtPoint(
                                pm.Direction.ToString(),
                                new Font("Arial", 8, FontStyle.Regular),
                                Brushes.Green,
                                hypoPoint
                            );
                        }

                        // Draw the ellipse
                        g.DrawRectangular(rc, pen, angleTranslator);

                        // Reset the graphics transform to the original state for other drawings
                        g.Transform = originalMatrix;
                    }
                    else
                        g.DrawRectangular(rc, pen, angleTranslator);
                }
            }

            base.OnPaint(e);
        }

        public World World => this.world;
    }
}
