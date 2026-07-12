using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm
{
    public class InverterInterneuronInfo(Neuron interneuron1, Neuron interneuron2)
    {
        public Neuron Interneuron1 { get; } = interneuron1;
        public Neuron Interneuron2 { get; } = interneuron2;
    }
}
