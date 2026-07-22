using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm
{
    public interface ICircuit : IneurULized
    {
        ParameterInfo Parameters { get; }
    }
}
