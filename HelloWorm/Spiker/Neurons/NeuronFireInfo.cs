namespace HelloWorm.Spiker.Neurons
{
    public class NeuronFireInfo(Neuron neuron, FireInfo fireInfo)
    {
        public Neuron Neuron { get; } = neuron;

        public FireInfo FireInfo { get; } = fireInfo;
    }
}
