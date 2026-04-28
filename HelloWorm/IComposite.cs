using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    public interface IComposite : IObject
    {
        void Add(IObject @object);

        void Remove(IObject @object);

        IEnumerable<IObject> Components { get; }

        event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;
    }
}
