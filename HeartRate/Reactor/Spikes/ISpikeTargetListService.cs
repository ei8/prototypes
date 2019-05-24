using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Spikes
{
    public interface ISpikeTargetListService
    {
        void Add(SpikeTarget value);

        void Remove(SpikeTarget value);

        event EventHandler<SpikeTargetEventArgs> Added;
        event EventHandler<SpikeTargetEventArgs> Removed;
        
        IEnumerable<SpikeTarget> Targets
        {
            get;
        }
    }
}
