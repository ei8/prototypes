using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public interface INamed : INotifyPropertyChanged
    {
        public string Name { get; set; }
    }
}
