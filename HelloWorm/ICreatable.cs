namespace HelloWorm
{
    public interface ICreatable : IPhysical
    {
        IPhysical Create(Size worldSize);
    }
}
