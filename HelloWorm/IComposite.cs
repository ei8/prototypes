using System.Collections.Specialized;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public interface IComposite : IPhysical
    { 
        IEnumerable<IPhysical> Components { get; set; }

        event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;
    }
}
