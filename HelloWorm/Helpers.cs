using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm
{
    internal static class Helpers
    {
        internal static Terminal CreateTerminal(
            Guid presynapticNeuronId,
            Guid postsynapticNeuronId
        ) => Helpers.CreateTerminal(presynapticNeuronId, postsynapticNeuronId, NeurotransmitterEffect.Excite, 1f);

        internal static Terminal CreateTerminal(
            Guid presynapticNeuronId,
            Guid postsynapticNeuronId,
            NeurotransmitterEffect effect,
            float strength
        ) => new Terminal(Guid.NewGuid(), presynapticNeuronId, postsynapticNeuronId, effect, strength);
    }
}
