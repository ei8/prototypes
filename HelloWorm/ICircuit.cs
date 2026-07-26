using ei8.Cortex.Coding.d23;

namespace ei8.Prototypes.HelloWorm
{
    public interface ICircuit : IneurUL
    {
        FunctionParameterInfo Parameters { get; }
    }
}
