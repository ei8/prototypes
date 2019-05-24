using Reactor.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.SpikeResults
{
    public struct NeuronResult
    {
        public NeuronResult(DateTime timestamp, string id, string data, NeurotransmitterEffect effect, int charge)
        {
            this.Timestamp = timestamp;
            this.Id = id;
            this.Data = data;
            this.Effect = effect;
            this.Charge = charge;
        }

        public readonly DateTime Timestamp;
        public readonly string Id;
        public readonly string Data;
        public NeurotransmitterEffect Effect { get; private set; }
        public int Charge { get; private set; }
    }
}
