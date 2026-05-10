using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    internal class ParseSensorResult(object @object, Neuron value)
    {
        public object Object { get; } = @object;

        public Neuron Value { get; } = value;
    }
}
