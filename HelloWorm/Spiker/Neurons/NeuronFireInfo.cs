namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public class NeuronFireInfo(SpikingNeuron neuron, FireInfo fireInfo)
    {
        public SpikingNeuron Neuron { get; } = neuron;

        public FireInfo FireInfo { get; } = fireInfo;
    }
}
