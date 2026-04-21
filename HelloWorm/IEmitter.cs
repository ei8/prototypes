using Timer = System.Threading.Timer;

namespace ei8.Prototypes.HelloWorm
{
    public interface IEmitter : IPhysical, ISectoral
    {
        public void Emit();

        event EventHandler<EmittedEventArgs> Emitted;
    }
}
