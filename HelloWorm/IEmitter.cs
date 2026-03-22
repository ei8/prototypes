using Timer = System.Threading.Timer;

namespace HelloWorm
{
    public interface IEmitter<T> : IPhysical, ISectoral where T : IPhysical
    {
        event EventHandler<EmittedEventArgs<T>> Emitted;
    }
}
