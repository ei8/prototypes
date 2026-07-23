using ei8.Cortex.Coding;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public abstract class LogicGateBase : ICircuit
    {
        public LogicGateBase()
        {
            this.Network = new();
            this.Interneurons = [];
            this.Parameters = new([], []);
        }

        public static bool TryCreate<T>(
            [NotNullWhen(true)] out T? result,
            ParameterInfo parameters,
            LogicGateInterneuronTagInfo? interneuronTagInfo = null,
            [CallerArgumentExpression(nameof(result))] string parameterExpression = "",
            params Neuron[] additionalInputs
        ) where T : LogicGateBase, new()
        {
            bool bResult = false;
            result = null;
            if (VariableInfo.TryParse(parameterExpression, out var variableInfo))
            {
                result = new();
                result.Initialize(
                    parameters,
                    variableInfo,
                    interneuronTagInfo,
                    additionalInputs
                );
                bResult = true;
            }

            return bResult;
        }

        protected abstract Neuron[] GetInterneuronOutputs(BinaryNeuronInfo output);

        protected abstract string[] GetInterneuronTags(
            VariableInfo variableInfo,
            LogicGateInterneuronTagInfo? interneuronTagInfo = null
        );

        protected static Network[] CreateInterneuronNetworks(
            Neuron[] outputs,
            string[] outputInterneuronTags
        ) => 
        [
            ..outputs.Select(o => {
            var index = Array.IndexOf(outputs, o);
            return NetworkHelper.CreateInterneuronNetwork(outputInterneuronTags[index], outputs[index]);
        })];

        protected abstract Network LinkInputNeurons(
            BinaryNeuronInfo[] inputs,
            params Neuron[] additionalInputs
        );

        protected void Initialize(
            ParameterInfo parameters, 
            VariableInfo variableInfo,
            LogicGateInterneuronTagInfo? interneuronTagInfo = null,
            params Neuron[] additionalInputs
        )
        {
            this.Parameters = parameters;
            this.Interneurons = LogicGateBase.CreateInterneuronNetworks(
                    this.GetInterneuronOutputs(parameters.Outputs.Single()),
                    this.GetInterneuronTags(variableInfo, interneuronTagInfo)
                );
            
            this.Network.AddReplaceItems(this.Parameters);
            this.Network.AddReplaceItems(
                [
                    ..this.Interneurons,
                    this.LinkInputNeurons(
                        parameters.Inputs,
                        additionalInputs
                    )
                ]
            );
        }

        public Network[] Interneurons { get; set; }

        public Network Network { get; }

        public ParameterInfo Parameters { get; set; }
    }
}