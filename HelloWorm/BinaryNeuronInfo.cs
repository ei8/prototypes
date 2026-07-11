using ei8.Cortex.Coding;
using System.Windows.Navigation;

namespace ei8.Prototypes.HelloWorm
{
    public class BinaryNeuronInfo
    {
        public BinaryNeuronInfo(Neuron neuron1, Neuron neuron0)
        {
            this.Neuron1 = neuron1;
            this.Neuron0 = neuron0;
        }
        
        public Neuron Neuron1 { get; }
        public Neuron Neuron0 { get; }
    }
}
