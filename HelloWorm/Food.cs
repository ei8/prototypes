using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    internal class Food : IRectangular, IEmitter, IPerishable, IRegenerative
    {
        private readonly Timer emissionTriggerTimer;

        public Food()
        {
            this.emissionTriggerTimer = new Timer(Food.Emit, this, 0, Constants.EmissionTriggerTimerPeriod);
        }

        public Point Location { get; set; }

        public Size Size { get; set; }
        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }

        public event EventHandler<EmittedEventArgs>? Emitted;

        public int Life { get; set; }

        private static void Emit(object? state)
        {
            if (state is Food currentFood) 
            {
                currentFood.Life--;

                var r = new Random();
                var deployCount = Constants.Odor.DeployMin + r.Next(Constants.Odor.DeployExtra);
                var newOdors = new List<Odor>();
                for (int i = 0; i < deployCount; i++)
                {
                    var sweep = r.Next((int) currentFood.SweepAngle);
                    newOdors.Add(new Odor()
                    {
                        Location = currentFood.Location,
                        Direction = currentFood.StartAngle + sweep,
                        Size = new Size(Constants.Odor.Size, Constants.Odor.Size),
                        Speed = Constants.Odor.Speed
                    });
                }

                currentFood.OnEmitted(new EmittedEventArgs(newOdors));
            }
        }

        private void OnEmitted(EmittedEventArgs e) => this.Emitted?.Invoke(this, e);

        public IPhysical Create(Size worldSize)
        {
            var r = new Random();
            return new Food()
            {
                Location = new Point(r.Next(worldSize.Width), r.Next(worldSize.Height)),
                Size = new Size(5, 5),
                StartAngle = r.Next(Constants.CircleDegreesCount),
                SweepAngle = 90 + r.Next(135),
                Life = Constants.Food.InitialLife
            };
        }
    }
}
