using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public interface IGraph : INotifyPropertyChanged, ITemporal
    {
        event EventHandler<ActivityLoggedEventArgs> ActivityLogged;
        event EventHandler Reloaded;

        void Reload();

        ISpikableReporting Spikable { get; }

        GraphSettings Settings { get; }
    }
}
