using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;

namespace ei8.Prototypes.HelloWorm
{
    public interface INeurULized
    {
        Network? Network { get; }

        IEnumerable<MirrorConfig>? MirrorConfigs { get; }

        void Initialize(Network network, IEnumerable<MirrorConfig> mirrorConfigs);
    }
}
