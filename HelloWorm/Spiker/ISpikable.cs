using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public interface ISpikable : INeurULized
    {
        // TODO: transfer to extension methods
        bool TryGetSpikeTargets<TS, TT>(TS source, TT target, [NotNullWhen(true)] out IEnumerable<Guid>? result)
            where TS : notnull
            where TT : notnull;

        ConcurrentDictionary<Guid, SpikeInfo> SpikeHistory { get; }

        void Initialize(Network? network, IEnumerable<MirrorConfig>? mirrorConfigs);

        TimeSpan RefractoryPeriod { get; set; }

        TimeSpan RelativeSpikesPeriod { get; set; }
    }
}
