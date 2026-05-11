using ei8.Cortex.Coding;
using System.Collections.Concurrent;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeService : ISpikeService
    {
        public const int DefaultThreshold = -55;
        public event EventHandler<TriggeredEventArgs>? Triggered;
        public event EventHandler<FiredEventArgs>? Fired;

        private class SpikeCallbackParameters(
            Neuron target,
            TriggerInfo trigger,
            IEnumerable<FireInfo> reflexArc,
            SpikeService spikeService,
            Network network,
            TimeSpan refractoryPeriod
        )
        {
            public Neuron Target { get; } = target;

            public TriggerInfo Trigger { get; } = trigger;

            public IEnumerable<FireInfo> ReflexArc { get; } = reflexArc;

            public SpikeService SpikeService { get; } = spikeService;
            
            public Network Network { get; } = network;
            
            public TimeSpan RefractoryPeriod { get; } = refractoryPeriod;
        }
        
        private int spikeCount = 1;
        private readonly ConcurrentDictionary<Guid, SpikeInfo> spikeHistory;

        public SpikeService()
        {
            this.spikeHistory = new ConcurrentDictionary<Guid, SpikeInfo>();
        }

        public void SetSpikeCount(int value)
        {
            spikeCount = value;
        }

        public void Spike(IEnumerable<Neuron> targets, Network network, TimeSpan refractoryPeriod)
        {
            for (int i = 1; i <= this.spikeCount; i++)
            {
                foreach (var target in targets)
                {
                    SpikeService.SpikeCore(
                        new SpikeCallbackParameters(
                            target,
                            new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                            Array.Empty<FireInfo>(),
                            this,
                            network,
                            refractoryPeriod
                        )
                    );
                };
            };
        }
        
        private static void SpikeCore(SpikeCallbackParameters state)
        {
            if (state.SpikeService.spikeHistory.TryGetAdd(state.Target.Id, id => new(), out SpikeInfo? spikeInfo))
            {
                var dateTimeTriggers = (ConcurrentDictionary<DateTime, TriggerInfo>)spikeInfo.Triggers;
                dateTimeTriggers.Clean(state.Trigger.Timestamp.Subtract(state.RefractoryPeriod));
                dateTimeTriggers.TryAdd(state.Trigger.Timestamp, state.Trigger);

                var charge = new ChargeInfo(spikeInfo.Triggers.Values);
                state.SpikeService.Triggered?.Invoke(
                    state.SpikeService,
                    new TriggeredEventArgs(
                        state.Target,
                        charge,
                        state.ReflexArc
                    )
                );
                
                // if spiked enough and spiked after repolarization
                if (
                    charge.Result >= SpikeService.DefaultThreshold &&
                    (
                        spikeInfo.LastFire == null ||
                        DateTime.Now > spikeInfo.LastFire.Timestamp.Add(state.RefractoryPeriod)
                    )
                )
                {
                    var spikeResultingFireInfo = new FireInfo(state.Target, DateTime.Now, [.. spikeInfo.Triggers.Values]);
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    state.SpikeService.Fired?.Invoke(
                        state.SpikeService,
                        new FiredEventArgs(
                            spikeResultingFireInfo,
                            charge
                        )
                    );

                    if (state.Network != null)
                    {
                        foreach (var t in state.Network.GetTerminals(state.Target.Id).ToList())
                        {
                            Task.Run(() =>
                                SpikeService.SpikeCore(
                                    new SpikeCallbackParameters(
                                        state.Network.ValidateGet(t.PostsynapticNeuronId),
                                        new TriggerInfo(DateTime.Now, t.Effect, t.Strength, state.Target.Id),
                                        state.ReflexArc.Concat([spikeResultingFireInfo]),
                                        state.SpikeService,
                                        state.Network,
                                        state.RefractoryPeriod
                                    )
                                )
                            );
                        }
                    }
                }
            }
            else
                throw new InvalidOperationException($"SpikeInfo for neuron '{state.Target.Id}' was not found.");
        }
    }
}
