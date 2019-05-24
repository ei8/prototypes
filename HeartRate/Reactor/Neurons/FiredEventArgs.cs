using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.Neurons
{
    public class FiredEventArgs : EventArgs
    {
        public FiredEventArgs(FireInfo fireInfo, int charge)
        {
            this.FireInfo = fireInfo;
            this.Charge = charge;
        }

        public FireInfo FireInfo { get; private set; }
        public int Charge { get; private set; }
    }
}
