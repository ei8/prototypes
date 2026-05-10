using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    public interface IComposite : IObject
    {
        void Add(IComponent component);

        void Remove(IComponent component);

        IEnumerable<IComponent> Components { get; }

        event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;
    }
}
