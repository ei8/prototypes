using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Promote to ISpikable
    public interface ISpikableTemp : ISpikable
    {
        void Spike(params Neuron[] neurons);
    }
}
