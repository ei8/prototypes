using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{
    public interface ISubtractor : IneurULized
    {
        InputInfo Operands { get; }

        BinaryNeuronInfo Borrow { get; }

        BinaryNeuronInfo Difference { get; }
    }
}
