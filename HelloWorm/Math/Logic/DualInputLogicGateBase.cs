using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public abstract class DualInputLogicGateBase : LogicGateBase
    {
        protected DualInputLogicGateBase() : base()
        {
        }

        protected override string[] GetInterneuronTags(VariableInfo variableInfo, LogicGateInterneuronTagInfo? interneuronTagInfo = null)
        {
            string typeTagPrefix = string.Empty,
                input1TagPrefix = string.Empty,
                input2TagPrefix = string.Empty;

            if (interneuronTagInfo != null)
            {
                typeTagPrefix = $"{interneuronTagInfo.TypeTagPrefix}.";
                input1TagPrefix = $"{interneuronTagInfo.InputTagPrefixes[0]}.";
                input2TagPrefix = $"{interneuronTagInfo.InputTagPrefixes[1]}.";
            }

            return [
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 0," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 0)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 0," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 1)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 1," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 0)",
                $"{typeTagPrefix}{variableInfo.Function}({input1TagPrefix}{variableInfo.Inputs.First()} = 1," +
                $"{input2TagPrefix}{variableInfo.Inputs.ElementAt(1)} = 1)",
            ];
        }

        protected override Network LinkInputNeurons(
            BinaryNeuronInfo[] inputs,
            params Neuron[] additionalInputs
        )
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(inputs.Length, 2);

            var result = new Network();

            result.AddReplaceItems(
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneurons[0].GetInterneuron(),
                    [
                        inputs[0].Neuron0,
                        inputs[1].Neuron0,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneurons[1].GetInterneuron(),
                    [
                        inputs[0].Neuron0,
                        inputs[1].Neuron1,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneurons[2].GetInterneuron(),
                    [
                        inputs[0].Neuron1,
                        inputs[1].Neuron0,
                        .. additionalInputs
                    ]
                ),
                NetworkHelper.LinkInputNeuronsToInterneuron(
                    this.Interneurons[3].GetInterneuron(),
                    [
                        inputs[0].Neuron1,
                        inputs[1].Neuron1,
                        .. additionalInputs
                    ]
                )
            );

            return result;
        }
    }
}
