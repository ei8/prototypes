namespace ei8.Prototypes.HelloWorm
{
    public interface IPerishable : IPhysical, IComponent
    {
        public int Life { get; set; }
    }
}
