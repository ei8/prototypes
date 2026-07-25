using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public class XnorGate : DualInputLogicGateBase
    {
        protected override Neuron[] GetInterneuronOutputs(BinaryNeuronInfo output) =>
        [
            output.Neuron1,
            output.Neuron0,
            output.Neuron0,
            output.Neuron1
        ];
    }
}
