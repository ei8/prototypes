using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public class AndGate : DualInputLogicGateBase
    {
        protected override Neuron[] GetInterneuronOutputs(BinaryNeuronInfo output) =>
        [
            output.Neuron0, 
            output.Neuron0, 
            output.Neuron0, 
            output.Neuron1
        ];
    }
}
