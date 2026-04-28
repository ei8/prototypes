namespace ei8.Prototypes.HelloWorm
{
    internal class DishPanel : Panel
    {
        private const float AngleOffset = 90;
        
        private Dish? dish;

        public DishPanel()
        {
            this.DoubleBuffered = true;

            this.SizeChanged += this.DishPanel_SizeChanged;
        }

        private void DishPanel_SizeChanged(object? sender, EventArgs e)
        {
            this.UpdateSize();
        }

        private void UpdateSize()
        {
            if (this.dish != null)
                this.dish.Size = new Size(this.Size.Width, this.Size.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using Pen dishPen = new Pen(Color.Blue);

            if (this.dish != null)
            { 
                if (this.dish.ShowGrid)
                    g.DrawGrid(this.dish.Size, 50);

                foreach (var p in this.dish.Components)
                {
                    using SolidBrush b = new(Color.Black);
                    using Pen pen = new(b);

                    if (p is IRectangular rc && (p is not Odor || this.dish.ShowOdor))
                    {
                        if (this.dish.ShowRectangularRectangles)
                        {
                            using Pen rectPen = new Pen(Color.Red);
                            g.DrawRectangle(rectPen, rc.GetRectangle());
                        }

                        var angleTranslator = new Func<float, float>((a) => a - DishPanel.AngleOffset);
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
                            g.RotateTransform(pm.Direction + DishPanel.AngleOffset);
                            // Translate back to the original top-left corner position relative to the new origin
                            g.TranslateTransform(-ellipseCenterX, -ellipseCenterY);

                            if (this.dish.ShowDirection)
                            {
                                Point hypoPoint = rc.GetRectangle().GetHypotenusePoint(180 - DishPanel.AngleOffset, 10);
                                g.DrawCenteredStringAtPoint(
                                    pm.Direction.ToString(),
                                    new Font("Arial", 8, FontStyle.Regular),
                                    Brushes.Green,
                                    hypoPoint
                                );
                            }

                            if (p is Worm pw)
                            {
                                if (this.dish.ShowScore)
                                {
                                    Point hypoPoint = rc.GetRectangle().GetHypotenusePoint(60 - DishPanel.AngleOffset, 18);
                                    g.DrawCenteredStringAtPoint(
                                        pw.Score.ToString(),
                                        new Font("Arial", 10, FontStyle.Regular),
                                        Brushes.Green,
                                        hypoPoint
                                    );
                                }

                                if (this.dish.ShowLife)
                                {
                                    Point hypoPoint2 = rc.GetRectangle().GetHypotenusePoint(45 - DishPanel.AngleOffset, 25);
                                    g.DrawCenteredStringAtPoint(
                                        pw.Life.ToString(),
                                        new Font("Arial", 7, FontStyle.Regular),
                                        Brushes.Red,
                                        hypoPoint2
                                    );
                                }
                            }
                            
                            // Draw the ellipse
                            g.DrawRectangular(rc, pen, angleTranslator, this.dish.ShowSectorIds);

                            // Reset the graphics transform to the original state for other drawings
                            g.Transform = originalMatrix;
                        }
                        else
                        {
                            g.DrawRectangular(rc, pen, angleTranslator, this.dish.ShowSectorIds);

                            if (rc is Food food && this.dish.ShowLife)
                            {
                                var foodLoc = rc.GetRectangle().Location;
                                Point hypoPoint2 = new Point(foodLoc.X + food.Size.Width + 10, foodLoc.Y + 4);
                                g.DrawCenteredStringAtPoint(
                                    food.Life.ToString(),
                                    new Font("Arial", 7, FontStyle.Regular),
                                    Brushes.Red,
                                    hypoPoint2
                                );
                            }
                        }
                    }
                }
            }

            base.OnPaint(e);
        }

        public Dish? Dish
        {
            get => this.dish;
            set
            {
                this.dish = value;

                if (this.dish != null)
                {
                    this.dish.NotifyCollectionChanged += this.Dish_NotifyCollectionChanged;
                }

                this.UpdateSize();
            }
        }

        private void Dish_NotifyCollectionChanged(object? sender, EventArgs e)
        {
            if (!((Dish)sender!).IsPlaying)
                this.InvalidateDish();
        }
    }
}
