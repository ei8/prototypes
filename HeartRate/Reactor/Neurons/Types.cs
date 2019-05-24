using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public enum NeurotransmitterEffect
    {
        Inhibit = -1,
        NotSet = 0,
        Excite = 1        
    }

    public struct Constants
    {
        public const int DefaultThreshold = -55;
        public const int RestingPotential = -70;
        public const int SpikeDepolarizationAmount = 15;
        public static readonly TimeSpan RefractoryPeriod = new TimeSpan(0, 0, 0, 0, 500);
    }
}
