using ei8.Cortex.Coding;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public interface IGraph : INotifyPropertyChanged, ITemporal
    {
        event EventHandler<ActivityLoggedEventArgs> ActivityLogged;
        event EventHandler Reloaded;

        void Reload();

        ISpikableReporting2 Spikable { get; }

        GraphSettings Settings { get; }
    }
}
