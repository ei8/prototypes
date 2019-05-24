using Reactor.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.SpikeResults
{
    public interface ISpikeResultsService
    {
        event EventHandler Cleared;
        event EventHandler<SpikeResultEventArgs> FiredAdded;
        event EventHandler<SpikeResultEventArgs> TriggeredAdded;

        void AddFired(Neuron value, FiredEventArgs firedEventArgs);

        void AddTriggered(Neuron result, TriggeredEventArgs triggeredEventArgs);

        void Clear();

        void Enable(bool value);
    }
}
