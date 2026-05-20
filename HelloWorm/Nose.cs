
using System.Collections.Specialized;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    internal class Nose : IRectangularComposite, IElliptical, INamed
    {
        private readonly IList<IComponent> components;

        public Nose()
        {
            this.components = new List<IComponent>();
        }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public IEnumerable<IComponent> Components => components;

        public required IComposite Parent { get; set; }
        public string Name { get => typeof(Nose).Name; set { throw new NotSupportedException(); } }

        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Add(IComponent component)
        {
            this.components.Add(component);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, component));
        }

        public void Remove(IComponent component)
        {
            this.components.Remove(component);

            this.NotifyCollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, component));
        }
    }
}