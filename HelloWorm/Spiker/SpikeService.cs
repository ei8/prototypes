using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;

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
            ISpikable spikable,
            Action<TriggeredEventArgs> triggeredCallback,
            Action<FiredEventArgs> firedCallback
        )
        {
            public Neuron Target { get; } = target;

            public SpikeOrigin Origin { get; } = origin;

            public TriggerInfo Trigger { get; } = trigger;

            public IEnumerable<FireInfo> ReflexArc { get; } = reflexArc;

            public ISpikable Spikable { get; } = spikable;

            public Action<TriggeredEventArgs> TriggeredCallback { get; } = triggeredCallback;

            public Action<FiredEventArgs> FiredCallback { get; } = firedCallback;
        }
        
        private int spikeCount = 1;

        public SpikeService()
        {
        }

        public void SetSpikeCount(int value)
        {
            spikeCount = value;
        }

        public void Spike(IEnumerable<Guid> targets, ISpikable spikable)
        {
            if (spikable.Network != null)
            {
                for (int i = 1; i <= this.spikeCount; i++)
                {
                    var targetNeurons = targets.Select(spikable.Network.ValidateGet);

                    foreach (var target in targetNeurons)
                    {
                        SpikeService.SpikeCore(
                            new SpikeCallbackParameters(
                                target,
                                new SpikeOrigin(Guid.NewGuid()),
                                new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                                Array.Empty<FireInfo>(),
                                spikable,
                                (tea) => this.Triggered?.Invoke(this, tea),
                                (fea) => this.Fired?.Invoke(this, fea)
                            )
                        );
                    };
                };
            }
        }
        
        private static void SpikeCore(SpikeCallbackParameters state)
        {
            if (((ConcurrentDictionary<Guid, SpikeInfo>)state.Spikable.NeuronSpikeHistory).TryGetAdd(state.Target.Id, id => new(), out SpikeInfo? spikeInfo))
            {
                var dateTimeTriggers = (ConcurrentDictionary<DateTime, TriggerInfo>)spikeInfo.Triggers;
                dateTimeTriggers.Clean(state.Trigger.Timestamp.Subtract(state.Spikable.RefractoryPeriod));
                dateTimeTriggers.TryAdd(state.Trigger.Timestamp, state.Trigger);

                ICollection<TriggerInfo> triggers = spikeInfo.Triggers.Values;

                int sumCharge = SpikeService.GetSumCharge(triggers, state.Target);

                state.TriggeredCallback(
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
                        !spikeInfo.LastFire.HasValue ||
                        DateTime.Now > spikeInfo.LastFire.Value.Timestamp.Add(state.Spikable.RefractoryPeriod)
                    )
                )
                {
                    var spikeResultingFireInfo = new FireInfo(state.Target, DateTime.Now, [.. triggers]);
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    state.FiredCallback(
                        new FiredEventArgs(
                            spikeResultingFireInfo,
                            sumCharge
                        )
                    );

                    if (state.Spikable.Network != null)
                    {
                        foreach (var t in state.Spikable.Network.GetTerminals(state.Target.Id).ToList())
                        {
                            Task.Run(() =>
                                SpikeService.SpikeCore(
                                    new SpikeCallbackParameters(
                                        state.Spikable.Network.ValidateGet(t.PostsynapticNeuronId),
                                        state.Origin,
                                        new TriggerInfo(DateTime.Now, t.Effect, t.Strength, state.Target.Id),
                                        state.ReflexArc.Concat([spikeResultingFireInfo]),
                                        state.Spikable,
                                        state.TriggeredCallback,
                                        state.FiredCallback
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
