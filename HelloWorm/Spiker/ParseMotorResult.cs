using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class ParseMotorResult(Neuron neuron, FireInfo fireInfo, object value)
    {
        public Neuron Neuron { get; } = neuron;
        public FireInfo FireInfo { get; } = fireInfo;
        public object Value { get; } = value;
    }
}
