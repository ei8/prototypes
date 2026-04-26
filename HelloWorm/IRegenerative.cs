namespace ei8.Prototypes.HelloWorm
{
    public interface IRegenerative : ICreatable
    {
        void Inherit(IRegenerative original);
    }
}
