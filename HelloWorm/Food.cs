using System.Runtime.CompilerServices;
using Timer = System.Threading.Timer;

namespace HelloWorm
{
    internal class Food : IRectangular, IEmitter<Odor>
    {
        private readonly Timer emissionTriggerTimer;

        public Food()
        {
            this.emissionTriggerTimer = new Timer(Food.Emit, this, 0, Constants.EmissionTriggerTimerPeriod);
        }

        public required Point Location { get; set; }

        public required Size Size { get; set; }
        public required float StartAngle { get; set; }
        public required float SweepAngle { get; set; }

        public event EventHandler<EmittedEventArgs<Odor>>? Emitted;

        private static void Emit(object? state)
        {
            if (state is Food currentFood) 
            {
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

                currentFood.OnEmitted(new EmittedEventArgs<Odor>(newOdors));
            }
        }

        private void OnEmitted(EmittedEventArgs<Odor> e) => this.Emitted?.Invoke(this, e);
    }
}
