using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SpikerNetworkHelper = ei8.Cortex.Coding.Spiker.NetworkHelper;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Promote to ei8.Cortex.Coding.Spiker.NetworkHelper
    public static class NetworkHelper
    {
        public static bool TryCreateNeuron(
            [NotNullWhen(true)] out Neuron? result,
            [CallerArgumentExpression(nameof(result))] string parameterExpression = ""
        )
        {
            bool bResult = false;
            result = null;
            if (VariableInfo.TryParse(parameterExpression, out var variable))
            {
                result = NetworkHelper.CreateNeuron(variable.Inputs.Single());
                bResult = true;
            }

            return bResult;
        }

        public static Neuron CreateNeuron(string? tag = null) =>
            Neuron.CreateTransient(Guid.NewGuid(), tag, null, null);

        public static Network CreateInterneuronNetwork(params Neuron[] postsynapticNeurons) =>
            NetworkHelper.CreateInterneuronNetwork(null, postsynapticNeurons);

        public static Network CreateInterneuronNetwork(string? interneuronTag = null, params Neuron[] postsynapticNeurons)
        {
            var network = new Network();
            Neuron neuron = NetworkHelper.CreateNeuron(interneuronTag);
            network.AddReplace(neuron);

            foreach (var post in postsynapticNeurons)
                network.AddReplace(SpikerNetworkHelper.CreateTerminal(neuron, post));

            return network;
        }

        public static Network LinkInputNeuronsToInterneuron(Neuron interneuron, params Neuron[] inputNeurons)
        {
            var network = new Network();
            foreach (Neuron input in inputNeurons)
                network.AddReplace(SpikerNetworkHelper.CreateTerminal(input, interneuron, NeurotransmitterEffect.Excite, 1f / inputNeurons.Length));
            return network;
        }

        public static Network CreateInputNeuronNetwork(MirrorConfig mirrorConfig, float strengthToInterneurons, params Network[] interneurons) =>
            SpikerNetworkHelper.CreateInputNeuronNetwork(mirrorConfig, strengthToInterneurons, [..interneurons.Select(i => i.GetInterneuron())]);
    }
}
