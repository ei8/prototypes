using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm;
using NLog;
using System.Collections.Concurrent;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeService : ISpikeService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<TriggeredEventArgs>? Triggered;

        public event EventHandler<FiredEventArgs>? Fired;

        private class SpikeCallbackParameters(
            Neuron target,
            SpikeOrigin origin,
            TriggerInfo trigger,
            IEnumerable<FireInfo> reflexArc,
            SpikeService spikeService,
            Network network,
            TimeSpan refractoryPeriod
        )
        {
            public Neuron Target { get; } = target;

            public SpikeOrigin Origin { get; } = origin;

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
                            new SpikeOrigin(Guid.NewGuid()),
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

                ICollection<TriggerInfo> triggers = spikeInfo.Triggers.Values;

                int sumCharge = SpikeService.GetSumCharge(triggers, state.Target);

                state.SpikeService.Triggered?.Invoke(
                    state.SpikeService,
                    new TriggeredEventArgs(
                        state.Target,
                        state.Origin,
                        state.Trigger,
                        sumCharge,
                        state.ReflexArc
                    )
                );
                
                // if spiked enough and spiked after repolarization
                if (
                    sumCharge >= Constants.Spiker.DefaultThreshold &&
                    (
                        spikeInfo.LastFire == null ||
                        DateTime.Now > spikeInfo.LastFire.Timestamp.Add(state.RefractoryPeriod)
                    )
                )
                {
                    var spikeResultingFireInfo = new FireInfo(state.Target, DateTime.Now, [.. triggers]);
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    state.SpikeService.Fired?.Invoke(
                        state.SpikeService,
                        new FiredEventArgs(
                            spikeResultingFireInfo,
                            sumCharge
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
                                        state.Origin,
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

        private static int GetSumCharge(ICollection<TriggerInfo> triggers, Neuron target)
        {
            var positiveCharge = SpikeService.GetChargeByEffect(triggers, NeurotransmitterEffect.Excite);
            var negativeCharge = SpikeService.GetChargeByEffect(triggers, NeurotransmitterEffect.Inhibit);
            var sumCharge = (int)(Constants.Spiker.RestingPotential + positiveCharge + negativeCharge);

            // TODO: return sumCharge in TriggeredEventArgs
            SpikeService.logger.Debug(
                new LogMessageGenerator(() =>
                    $"EffectiveTriggers: {triggers.Count()}; " +
                    $"Sum charge: {positiveCharge} + {negativeCharge} = {sumCharge} ({target.ToReadableString()})")
            );

            return sumCharge;
        }

        private static float GetChargeByEffect(IEnumerable<TriggerInfo> effectiveTriggers, NeurotransmitterEffect effect)
        {
            // sum triggers within the last refractory period
            return effectiveTriggers.Sum(
                ti => ti.Effect == effect ?
                    Constants.Spiker.SpikeDepolarizationAmount * ti.Strength * (int)effect :
                    0
                );
        }
    }
}
