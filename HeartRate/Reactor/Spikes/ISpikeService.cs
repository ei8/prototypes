using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Spikes
{
    public interface ISpikeService
    {
        event EventHandler Spiking;

        void SetSpikeCount(int value);

        void Spike(IEnumerable<SpikeTarget> spikeTargets);
    }
}
