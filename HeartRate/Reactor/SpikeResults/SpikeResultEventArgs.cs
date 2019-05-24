using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.SpikeResults
{
    public class SpikeResultEventArgs : EventArgs
    {
        public SpikeResultEventArgs(NeuronResult result)
        {
            this.Result = result;
        }

        public NeuronResult Result { get; private set; }
    }
}
