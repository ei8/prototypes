namespace ei8.Prototypes.HelloWorm
{
    public class DocumentActivationRequestedEventArgs(IObject @object) : EventArgs
    {
        public IObject Object { get; } = @object;
    }
}
