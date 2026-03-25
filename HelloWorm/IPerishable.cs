namespace HelloWorm
{
    public interface IPerishable : IPhysical
    {
        public int Life { get; set; }
    }
}
