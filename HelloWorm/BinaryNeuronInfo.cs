using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Spiker;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ei8.Prototypes.HelloWorm
{
    public class BinaryNeuronInfo : IneurULized
    {
        public BinaryNeuronInfo(Neuron neuron1, Neuron neuron0)
        {
            this.Neuron1 = neuron1;
            this.Neuron0 = neuron0;

            this.Network = new();
            this.Network.AddReplaceItems(
                [
                    this.Neuron1,
                    this.Neuron0
                ]
            );
        }

        public static BinaryNeuronInfo Create(
            string tagPrefix,
            string trueString = "1",
            string falseString = "0"
        ) =>
            new(
                NetworkHelper.CreateNeuron($"{tagPrefix} = {trueString}"),
                NetworkHelper.CreateNeuron($"{tagPrefix} = {falseString}")
            );

        public static bool TryCreate(
            [NotNullWhen(true)] out BinaryNeuronInfo? result,
            string? tagPrefix = null,
            [CallerArgumentExpression(nameof(result))] string parameterExpression = "",
            string trueString = "1",
            string falseString = "0"
        )
        {
            bool bResult = false;
            result = null;

            if (VariableInfo.TryParse(parameterExpression, out var variableInfo))
            {
                result = BinaryNeuronInfo.Create(
                    $"{tagPrefix}" +
                    (string.IsNullOrWhiteSpace(tagPrefix) ? string.Empty : ".") +
                    variableInfo.ToString(),
                    trueString,
                    falseString
                );
                bResult = true;
            }

            return bResult;
        }

        public Neuron Neuron1 { get; }
        public Neuron Neuron0 { get; }

        public Network Network { get; }
    }
}
