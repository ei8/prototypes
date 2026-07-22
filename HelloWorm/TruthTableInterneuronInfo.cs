using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Rename to LogicGateBase and create derived classes for XorGate, AndGate, etc.
    public class TruthTableInterneuronInfo : IneurULized
    {
        public enum LogicGateType
        {
            Not,
            And,
            Or,
            Nand,
            Nor,
            Xor,
            Xnor,
            Imply,
            Nimply
        }

        public TruthTableInterneuronInfo(
            Network interneuron1,
            Network interneuron2,
            Network interneuron3,
            Network interneuron4
        )
        {
            this.Interneuron1 = interneuron1;
            this.Interneuron2 = interneuron2;
            this.Interneuron3 = interneuron3;
            this.Interneuron4 = interneuron4;

            this.Network = new();
            this.Network.AddReplaceItems(
                this.Interneuron1,
                this.Interneuron2,
                this.Interneuron3,
                this.Interneuron4
            );
        }

        public static bool TryCreate(
            [NotNullWhen(true)] out TruthTableInterneuronInfo? result,
            LogicGateType type,
            BinaryNeuronInfo outputs,
            TruthTableInterneuronTagInfo? interneuronTags = null,
            [CallerArgumentExpression(nameof(result))] string parameterExpression = ""
        )
        {
            bool bResult = false;
            result = null;
            if (VariableInfo.TryParse(parameterExpression, out var variableInfo))
            {
                result = TruthTableInterneuronInfo.Create(
                    type,
                    outputs,
                    variableInfo,
                    interneuronTags
                );
                bResult = true;
            }

            return bResult;
        }

        public static TruthTableInterneuronInfo Create(
            LogicGateType type,
            BinaryNeuronInfo outputs,
            VariableInfo variableInfo,
            TruthTableInterneuronTagInfo? interneuronTags = null
        )
        {
            string typeTagPrefix = string.Empty,
                input1TagPrefix = string.Empty,
                input2TagPrefix = string.Empty;

            if (interneuronTags != null)
            {
                typeTagPrefix = $"{interneuronTags.TypeTagPrefix}.";
                input1TagPrefix = $"{interneuronTags.Input1TagPrefix}.";
                input2TagPrefix = $"{interneuronTags.Input2TagPrefix}.";
            }

            string?[] outputInterneuronTags = [
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 0," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 0)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 0," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 1)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 1," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 0)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 1," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 1)",
            ];

            switch (type)
            {
                default:
                case LogicGateType.And:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron0, outputs.Neuron0, outputs.Neuron0, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Or:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron0, outputs.Neuron1, outputs.Neuron1, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Nand:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron1, outputs.Neuron1, outputs.Neuron1, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Nor:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron1, outputs.Neuron0, outputs.Neuron0, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Xor:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron0, outputs.Neuron1, outputs.Neuron1, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Xnor:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron1, outputs.Neuron0, outputs.Neuron0, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Imply:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron1, outputs.Neuron1, outputs.Neuron0, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Nimply:
                    return TruthTableInterneuronInfo.Create(
                        outputs.Neuron0, outputs.Neuron0, outputs.Neuron1, outputs.Neuron0, outputInterneuronTags
                    );
            }
        }

        private static TruthTableInterneuronInfo Create(
            Neuron output1,
            Neuron output2,
            Neuron output3,
            Neuron output4,
            string?[] outputInterneuronTags
        ) => new TruthTableInterneuronInfo(
            NetworkHelper.CreateInterneuronNetwork(outputInterneuronTags[0], output1),
            NetworkHelper.CreateInterneuronNetwork(outputInterneuronTags[1], output2),
            NetworkHelper.CreateInterneuronNetwork(outputInterneuronTags[2], output3),
            NetworkHelper.CreateInterneuronNetwork(outputInterneuronTags[3], output4)
        );

        public Network LinkInputNeurons(
            InputInfo inputs,
            params Neuron[] additionalInputs
        )
        {
            var result = new Network();

            result.AddReplaceItems(
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneuron1.GetInterneuron(),
                    [
                        inputs.Input1.Neuron0,
                        inputs.Input2.Neuron0,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneuron2.GetInterneuron(),
                    [
                        inputs.Input1.Neuron0,
                        inputs.Input2.Neuron1,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneuron3.GetInterneuron(),
                    [
                        inputs.Input1.Neuron1,
                        inputs.Input2.Neuron0,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneuron4.GetInterneuron(),
                    [
                        inputs.Input1.Neuron1,
                        inputs.Input2.Neuron1,
                        .. additionalInputs
                    ]
                )
            );

            return result;
        }

        public Network Interneuron1 { get; } 
        public Network Interneuron2 { get; } 
        public Network Interneuron3 { get; } 
        public Network Interneuron4 { get; } 

        public Network Network { get; }
    }
}
