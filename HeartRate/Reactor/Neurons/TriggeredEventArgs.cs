using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public class TriggeredEventArgs : EventArgs
    {
        public TriggeredEventArgs(SpikeOrigin spikeOrigin, TriggerInfo triggerInfo, int charge, IEnumerable<FireInfo> path)
        {
            this.SpikeOrigin = spikeOrigin;
            this.TriggerInfo = triggerInfo;
            this.Charge = charge;
            this.Path = path;
        }

        public SpikeOrigin SpikeOrigin { get; private set; }
        public TriggerInfo TriggerInfo { get; private set; }
        public int Charge { get; private set; }
        public IEnumerable<FireInfo> Path { get; private set; }
    }
}
