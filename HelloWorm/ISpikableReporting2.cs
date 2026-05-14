using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm
{
    // TODO: Transfer to ISpikableReporting
    public interface ISpikableReporting2 : ISpikableReporting
    {
        event EventHandler<TriggeredEventArgs>? Triggered;

        event EventHandler<FiredEventArgs>? Fired;
    }
}
