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

        public Point Location { get; set; }

        public Size Size { get; set; }

        public event EventHandler<EmittedEventArgs<Odor>>? Emitted;

        private static void Emit(object? state)
        {
            if (state is Food currentFood) 
            {
                var r = new Random();
                var direction = r.Next(Constants.CircleDegreesCount);
                var newOdor = new Odor()
                {
                    Location = currentFood.Location,
                    Direction = direction,
                    Size = new Size(Constants.Food.Size, Constants.Food.Size),
                    Speed = Constants.Food.Speed
                };

                currentFood.OnEmitted(new EmittedEventArgs<Odor>([newOdor]));
            }
        }

        private void OnEmitted(EmittedEventArgs<Odor> e) => this.Emitted?.Invoke(this, e);
    }
}
