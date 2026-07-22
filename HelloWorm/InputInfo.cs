using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Rename IneurULized to IneurUL, can generate Network based on contained IneurULs (InputInfo)
    // TODO: Create ICircuit or something else to accommodate voluntary actions, with Inputs, Outputs, Interneurons (AndGate)
    public class InputInfo : IneurULized
    {
        public InputInfo(BinaryNeuronInfo input1, BinaryNeuronInfo input2)
        {
            this.Input1 = input1;
            this.Input2 = input2;

            this.Network = new();
            this.Network.AddReplaceItems(
                this.Input1.Network,
                this.Input2.Network
            );
        }

        public static InputInfo Create(
            string input1TagPrefix,
            string input2TagPrefix,
            string trueString = "1",
            string falseString = "0"
        ) =>
            new(
                BinaryNeuronInfo.Create(
                    input1TagPrefix,
                    trueString,
                    falseString
                ),
                BinaryNeuronInfo.Create(
                    input2TagPrefix,
                    trueString,
                    falseString
                )
            );


        public BinaryNeuronInfo Input1 { get; }
        public BinaryNeuronInfo Input2 { get; }

        public Network Network { get; }
    }
}
