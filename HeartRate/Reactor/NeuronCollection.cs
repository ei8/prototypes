using Reactor.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor
{
    public class NeuronCollection : ObservableKeyedCollection<string, Neuron>
    {
        protected override string GetKeyForItem(Neuron item) => item.Id;
    }
}
