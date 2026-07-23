using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    // TODO: Rename IneurULized to IneurUL, can generate Network based on contained IneurULs (InputInfo)
    public class NotGate : LogicGateBase
    {
        public NotGate() : base()
        {
        }

        protected override string[] GetInterneuronTags(VariableInfo variableInfo, LogicGateInterneuronTagInfo? interneuronTagInfo = null)
        {
            string coreOperatorPrefix = string.Empty,
                coreInputTagPrefix = string.Empty;

            if (interneuronTagInfo != null)
            {
                if (!string.IsNullOrEmpty(interneuronTagInfo.TypeTagPrefix))
                    coreOperatorPrefix = $"{interneuronTagInfo.TypeTagPrefix}.";
                if (interneuronTagInfo.InputTagPrefixes?.Length > 0)
                    coreInputTagPrefix = $"{interneuronTagInfo.InputTagPrefixes[0]}.";
            }

            return [
                $"{coreOperatorPrefix}{variableInfo.Function}({coreInputTagPrefix}{variableInfo.Inputs.Single()} = 0)",
                $"{coreOperatorPrefix}{variableInfo.Function}({coreInputTagPrefix}{variableInfo.Inputs.Single()} = 1)"
            ];
        }

        protected override Neuron[] GetInterneuronOutputs(BinaryNeuronInfo output) =>
        [
            output.Neuron1,
            output.Neuron0
        ];

        protected override Network LinkInputNeurons(BinaryNeuronInfo[] inputs, params Neuron[] additionalInputs)
        {
            var result = new Network();
            result.AddReplaceItems(
                [
                    NetworkHelper.LinkInputNeuronsToInterneuron(
                        this.Interneurons[0].GetInterneuron(),
                        [
                            inputs.Single().Neuron0,
                            .. additionalInputs
                        ]
                    ),
                    NetworkHelper.LinkInputNeuronsToInterneuron(
                        this.Interneurons[1].GetInterneuron(),
                        [
                            inputs.Single().Neuron1,
                            .. additionalInputs
                        ]
                    )
                ]
            );
            return result;
        }
    }
}
