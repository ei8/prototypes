using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{
    public interface IAdder : IneurULized
    {
        InputInfo Addends { get; }

        BinaryNeuronInfo CarryOver { get; }
    }
}
