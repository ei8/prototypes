using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public interface ISpikable : INeurULized
    {
        IDictionary<DateTime, FireInfo> FireHistory { get; }

        void Initialize(Network? network, IEnumerable<MirrorConfig>? mirrorConfigs);

        TimeSpan RefractoryPeriod { get; set; }

        TimeSpan RelatedSpikesPeriod { get; set; }
    }
}
