namespace ei8.Prototypes.HelloWorm
{
    public interface IComponent : IObject
    {
        IComposite Parent { get; set; }
    }
}
