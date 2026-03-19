namespace HelloWorm
{
    internal interface IComposite : IPhysical
    { 
        IEnumerable<IPhysical> Components { get; set; }
    }
}
