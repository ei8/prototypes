using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm
{
    public class ParameterInfo : IneurULized
    {
        public ParameterInfo(BinaryNeuronInfo[] inputs, BinaryNeuronInfo[] outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;

            this.Network = new();
            this.Network.AddReplaceItems(
                [
                    ..this.Inputs,
                    ..this.Outputs
                ]
            );
        }

        public BinaryNeuronInfo[] Inputs { get; } 
        public BinaryNeuronInfo[] Outputs { get; }

        public Network Network { get; }
    }
}
