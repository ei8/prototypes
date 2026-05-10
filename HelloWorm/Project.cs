using NLog;
using System.Collections.Specialized;

namespace ei8.Prototypes.HelloWorm
{
    public class Project : IAbstract, IComposite
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IList<IComponent> components;

        public Project()
        {
            this.components = new List<IComponent>();
        }

        public IEnumerable<IComponent> Components => components;

        public event NotifyCollectionChangedEventHandler? NotifyCollectionChanged;

        public void Add(IComponent component)
        {
            this.components.Add(component);

            Project.logger.Info(new LogMessageGenerator(() => $"{typeof(Project).Name} - {component.GetFullName()} added."));

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    component
                )
            );
        }

        public void Remove(IComponent component)
        {
            this.components.Remove(component);

            this.NotifyCollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    component
                )
            );
        }
    }
}
