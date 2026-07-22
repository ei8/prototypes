using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Create NotGate, inherit from LogicGateBase
    public class InverterInterneuronInfo : IneurULized
    {
        public InverterInterneuronInfo(
            Network interneuron1,
            Network interneuron2
        )
        {
            this.Interneuron1 = interneuron1;
            this.Interneuron2 = interneuron2;

            this.Network = new();
            this.Network.AddReplaceItems(
                this.Interneuron1,
                this.Interneuron2
            );
        }

        public static bool TryCreate(
            [NotNullWhen(true)] out InverterInterneuronInfo? result,
            BinaryNeuronInfo outputs,
            string? operatorPrefix = null,
            string? inputTagPrefix = null,
            [CallerArgumentExpression(nameof(result))] string parameterExpression = ""
        )
        {
            bool bResult = false;
            result = null;
            if (VariableInfo.TryParse(parameterExpression, out var variableInfo))
            {
                result = InverterInterneuronInfo.Create(
                    outputs,
                    variableInfo,
                    operatorPrefix,
                    inputTagPrefix
                );
                bResult = true;
            }

            return bResult;
        }

        public static InverterInterneuronInfo Create(
            BinaryNeuronInfo outputs,
            VariableInfo variableInfo,
            string? operatorPrefix = null,
            string? inputTagPrefix = null
        )
        {
            string coreOperatorPrefix = string.Empty,
                coreInputTagPrefix = string.Empty;

            if (!string.IsNullOrEmpty(operatorPrefix))
                coreOperatorPrefix = $"{operatorPrefix}.";
            if (!string.IsNullOrEmpty(inputTagPrefix))
                coreInputTagPrefix = $"{inputTagPrefix}.";

            return new InverterInterneuronInfo(
               NetworkHelper.CreateInterneuronNetwork($"{coreOperatorPrefix}{variableInfo.Function}({coreInputTagPrefix}{variableInfo.Inputs.Single()} = 0)", outputs.Neuron1),
               NetworkHelper.CreateInterneuronNetwork($"{coreOperatorPrefix}{variableInfo.Function}({coreInputTagPrefix}{variableInfo.Inputs.Single()} = 1)", outputs.Neuron0)
            );
        }


        public Network LinkInputNeurons(
            BinaryNeuronInfo input,
            params Neuron[] additionalInputs
        )
        {
            var result = new Network();
            result.AddReplaceItems(
                [
                    NetworkHelper.LinkInputNeuronsToInterneuron(
                        this.Interneuron1.GetInterneuron(),
                        [
                            input.Neuron0,
                            .. additionalInputs
                        ]
                    ),
                    NetworkHelper.LinkInputNeuronsToInterneuron(
                        this.Interneuron2.GetInterneuron(),
                        [
                            input.Neuron1,
                            .. additionalInputs
                        ]
                    )
                ]
            );
            return result;
        }

        public Network Interneuron1 { get; }
        public Network Interneuron2 { get; }

        public Network Network { get; }
    }
}
