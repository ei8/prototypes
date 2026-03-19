using Timer = System.Threading.Timer;

namespace HelloWorm
{
    public interface IEmitter<T> : IPhysical where T : IPhysical
    {
        event EventHandler<EmittedEventArgs<T>> Emitted;
    }
}
