using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm
{
    public class TruthTableInterneuronInfo(Neuron interneuron1, Neuron interneuron2, Neuron interneuron3, Neuron interneuron4)
    {
        public Neuron Interneuron1 { get; } = interneuron1;
        public Neuron Interneuron2 { get; } = interneuron2;
        public Neuron Interneuron3 { get; } = interneuron3;
        public Neuron Interneuron4 { get; } = interneuron4;
    }
}
