using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public class ImplyGate : DualInputLogicGateBase
    {
        protected override Neuron[] GetInterneuronOutputs(BinaryNeuronInfo output) =>
        [
            output.Neuron1,
            output.Neuron1,
            output.Neuron0,
            output.Neuron1
        ];
    }
}
