namespace ei8.Prototypes.HelloWorm
{
    internal class Food : IRectangular, IEmitter, IPerishable, IRegenerative
    {
        public Food()
        {
        }

        public Point Location { get; set; }

        public Size Size { get; set; }
        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }

        public event EventHandler<EmittedEventArgs>? Emitted;

        public int Life { get; set; }

        public void Emit()
        {
            var r = new Random();
            var deployCount = Constants.Odor.DeployMin + r.Next(Constants.Odor.DeployExtra);
            var newOdors = new List<Odor>();
            for (int i = 0; i < deployCount; i++)
            {
                var sweep = r.Next((int)this.SweepAngle);
                newOdors.Add(new Odor()
                {
                    Location = this.Location,
                    Direction = this.StartAngle + sweep,
                    Size = new Size(Constants.Odor.Size, Constants.Odor.Size),
                    Speed = Constants.Odor.Speed
                });
            }

            this.Emitted?.Invoke(this, new EmittedEventArgs(newOdors));
        }
        
        public IPhysical Create(Size dishSize)
        {
            var r = new Random();
            return new Food()
            {
                Location = new Point(r.Next(dishSize.Width), r.Next(dishSize.Height)),
                Size = new Size(5, 5),
                StartAngle = r.Next(Constants.CircleDegreesCount),
                SweepAngle = 90 + r.Next(135),
                Life = Constants.Food.InitialLife
            };
        }
    }
}
