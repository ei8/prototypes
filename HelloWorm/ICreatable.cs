namespace ei8.Prototypes.HelloWorm
{
    public interface ICreatable : IPhysical, IComponent, INamed
    {
        void Initialize(string name, IRectangularComposite parent);
    }
}
