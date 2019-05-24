using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public struct SpikeOrigin
    {
        public SpikeOrigin(string id)
        {
            this.Id = id;
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return $"SpikeOrigin: '{this.Id}'";
        }
    }
}
