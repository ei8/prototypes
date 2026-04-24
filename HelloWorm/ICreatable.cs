namespace ei8.Prototypes.HelloWorm
{
    public interface ICreatable : IPhysical
    {
        IPhysical Create(Size dishSize);
    }
}
