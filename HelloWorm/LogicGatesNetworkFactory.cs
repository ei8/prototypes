using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using ei8.Cortex.Coding.d23.Math.Logic;

namespace ei8.Prototypes.HelloWorm
{
    internal static class LogicGatesNetworkFactory
    {
        public static ReadOnlyNetwork CreateLogicGates()
        {
            Network net = new();

            var input1 = BinaryNeuronParameter.Create("Input1", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper());
            var input2 = BinaryNeuronParameter.Create("Input2", Boolean.TrueString.ToUpper(), Boolean.FalseString.ToUpper());

            if (
                BinaryNeuronParameter.TryCreate(out var result, trueString: Boolean.TrueString.ToUpper(), falseString: Boolean.FalseString.ToUpper()) &&
                NetworkHelper.TryCreateNeuron(out var NOT) &&
                NetworkHelper.TryCreateNeuron(out var AND) &&
                NetworkHelper.TryCreateNeuron(out var OR) &&
                NetworkHelper.TryCreateNeuron(out var NAND) &&
                NetworkHelper.TryCreateNeuron(out var NOR) &&
                NetworkHelper.TryCreateNeuron(out var XOR) &&
                NetworkHelper.TryCreateNeuron(out var XNOR) &&
                NetworkHelper.TryCreateNeuron(out var IMPLY) &&
                NetworkHelper.TryCreateNeuron(out var NIMPLY) &&
                NotGate.TryCreate(
                    out var NOT___Input1,
                    new(new(input1), new(result)),
                    additionalInputNeurons: new(NOT)
                ) &&
                NotGate.TryCreate(
                    out var NOT___Input2,
                    new(new(input2), new(result)),
                    additionalInputNeurons: new(NOT)
                ) &&
                AndGate.TryCreate(
                    out var AND___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(AND)
                ) &&
                OrGate.TryCreate(
                    out var OR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(OR)
                ) &&
                NandGate.TryCreate(
                    out var NAND___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(NAND)
                ) &&
                NorGate.TryCreate(
                    out var NOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(NOR)
                ) &&
                XorGate.TryCreate(
                    out var XOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(XOR)
                ) &&
                XnorGate.TryCreate(
                    out var XNOR___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(XNOR)
                ) &&
                ImplyGate.TryCreate(
                    out var IMPLY___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(IMPLY)
                ) &&
                NimplyGate.TryCreate(
                    out var NIMPLY___Input1__Input2,
                    new(new(input1, input2), new(result)),
                    additionalInputNeurons: new(NIMPLY)
                )
            )
            {
                net.AddReplaceItems(
                    [
                        result,
                        NOT___Input1,
                        NOT___Input2,
                        AND___Input1__Input2,
                        OR___Input1__Input2,
                        NAND___Input1__Input2,
                        NOR___Input1__Input2,
                        XOR___Input1__Input2,
                        XNOR___Input1__Input2,
                        IMPLY___Input1__Input2,
                        NIMPLY___Input1__Input2,
                        input1,
                        input2
                    ]
                );
                net.AddReplaceItems(
                    [
                        NOT,
                        AND,
                        OR,
                        NAND,
                        NOR,
                        XOR,
                        XNOR,
                        IMPLY,
                        NIMPLY
                    ]
                );
            }

            return net;
        }
    }
}
