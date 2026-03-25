using Timer = System.Threading.Timer;

namespace HelloWorm
{
    public interface IEmitter : IPhysical, ISectoral
    {
        event EventHandler<EmittedEventArgs> Emitted;
    }
}
