using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal class Worm : IMovable, IRectangularComposite, IElliptical
    {
        private readonly Timer movementTriggerTimer;

        public Worm(int direction, int x, int y, int width)
        {
            this.Direction = direction;
            this.Location = new Point(x, y);
            this.Life = Constants.Worm.InitialLife;
            this.Components = [
                new Nose()
                {
                    Location = new Point(0, 0),
                    Size = new Size(1, 1),
                    Components = Worm.InitializeSectors()
                }
            ];
            this.UpdateSize(Constants.Worm.MinWidth);

            this.movementTriggerTimer = new Timer(this.WrapMove, null, 0, Constants.MovementTriggerTimerPeriod);
        }

        private static IEnumerable<Sector> InitializeSectors()
        {
            var sects = new List<Sector>();

            for (int i = 0; i < Constants.Worm.SectorRenderCount; i++)
                sects.Add(new Sector()
                {
                    StartAngle = (i * Constants.Worm.SectorSweepAngle) + 1,
                    SweepAngle = Constants.Worm.SectorSweepAngle
                });

            return sects;
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
        public float Direction { get; set; }
        public int Speed { get; set; }
        public int Score { get; set; }
        public int Life { get; set; }
        public IEnumerable<IPhysical> Components { get; set; }

        public event EventHandler<MovingEventArgs>? Moving;
        public event EventHandler<CollidedEventArgs>? Collided;

        public void Grow()
        {
            this.Score++;
            this.Life += Constants.Worm.InitialLife;
            if (this.Size.Width < Constants.Worm.MaxWidth)
                this.UpdateSize(this.Size.Width + 2);
        }

        private void UpdateSize(int width)
        {
            float pctMax = 
                ((float)(width - Constants.Worm.MinWidth)) / 
                (Constants.Worm.MaxWidth - Constants.Worm.MinWidth);
            this.Size = new Size(
                width, 
                (int)
                (
                    Constants.Worm.MinLength + 
                    (
                        (
                            Constants.Worm.MaxLength - 
                            Constants.Worm.MinLength
                        ) * 
                        pctMax
                    )
                )
            );
            this.Speed = 
                (int)
                (
                    Constants.Worm.MinSpeed + 
                    (
                        Constants.Worm.MaxSpeed - 
                        Constants.Worm.MinSpeed - 
                        (
                            (
                                Constants.Worm.MaxSpeed - 
                                Constants.Worm.MinSpeed
                            ) * 
                            pctMax
                        )
                    )
                );
            this.Components.OfType<Nose>().Single().Size = new Size(width, width);
        }

        private void WrapMove(object? state)
        {
            this.Life--;
            this.Move(state);
        }

        public void Collide(CollisionInfo info)
        {
            if (info.Target is World && info.Source is ISectoral sector)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector);
                switch (sectorId)
                {
                    case 8:
                    case 1:
                        this.Direction += 45f * (sectorId == 8 ? 1 : -1);
                        break;
                    case 7:
                    case 2:
                        this.Direction += 22.5f * (sectorId == 7 ? 1 : -1);
                        break;
                }
            }
            else if (info.Target is Odor && info.Source is ISectoral sector2)
            {
                var sectorId = this.Components.OfType<Nose>().Single().GetSectorId(sector2);
                switch (sectorId)
                {
                    case 1:
                    case 8:
                        this.Direction += 22.5f * (sectorId == 1 ? 1 : -1);
                        break;
                    case 2:
                    case 7:
                        this.Direction += 45f * (sectorId == 2 ? 1 : -1);
                        break;
                    case 3:
                    case 6:
                        this.Direction += 60f * (sectorId == 3 ? 1 : -1);
                        break;
                    case 4:
                    case 5:
                        this.Direction += 70f * (sectorId == 4 ? 1 : -1);
                        break;
                }
            }

            this.Collided?.Invoke(this, new CollidedEventArgs(info.Target));
        }

        public void OnMoving(MovingEventArgs e) => this.Moving?.Invoke(this, e);
    }
}
