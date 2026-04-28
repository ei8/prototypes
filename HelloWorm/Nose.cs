
using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    internal class Nose : IRectangularComposite, IElliptical
    {
        private readonly IList<IObject> components;

        public Nose()
        {
            this.components = new List<IObject>();
        }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public IEnumerable<IObject> Components => components;

        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        public void Add(IObject @object)
        {
            this.components.Add(@object);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, @object));
        }

        public void Remove(IObject @object)
        {
            this.components.Remove(@object);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, @object));
        }
    }
}