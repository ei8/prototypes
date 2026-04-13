using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class NeuronFireInfo(Neuron neuron, FireInfo fireInfo)
    {
        public Neuron Neuron { get; } = neuron;

        public FireInfo FireInfo { get; } = fireInfo;
    }
}
