namespace ei8.Prototypes.HelloWorm
{
    internal interface IComposite : IPhysical
    { 
        IEnumerable<IPhysical> Components { get; set; }
    }
}
