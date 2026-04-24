using ei8.Cortex.Coding.Mirrors;

namespace ei8.Prototypes.HelloWorm
{
    public interface ISettingsService
    {
        IEnumerable<MirrorConfig>? Mirrors { get; set; }
    }
}
