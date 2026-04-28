using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    public class Project : IAbstract, IComposite
    {
        private readonly IList<IObject> components;

        public Project()
        {
            this.components = new List<IObject>();
        }

        public IEnumerable<IObject> Components => components;

        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        public void Add(IObject @object)
        {
            this.components.Add(@object);

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    @object
                )
            );
        }

        public void Remove(IObject @object)
        {
            this.components.Remove(@object);

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    @object
                )
            );
        }
    }
}
