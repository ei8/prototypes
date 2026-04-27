using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm;
using System.Diagnostics;

namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class SpikeService : ISpikeService
    {
        public event EventHandler<TriggeredEventArgs>? Triggered;

        public event EventHandler<FiredEventArgs>? Fired;

        private class SpikeParameters(
            Neuron target,
            SpikeOrigin origin,
            TriggerInfo trigger,
            IEnumerable<FireInfo> path,
            ISpikable spikable,
            Action<TriggeredEventArgs> triggeredCallback,
            Action<FiredEventArgs> firedCallback
        )
        {
            public Neuron Target { get; } = target;

            public SpikeOrigin Origin { get; } = origin;

            public TriggerInfo Trigger { get; } = trigger;

            public IEnumerable<FireInfo> Path { get; } = path;

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
                for (int i = 1; i <= spikeCount; i++)
                {
                    var targetNeurons = targets.Select(spikable.Network.ValidateGet);

                    foreach (Neuron target in targetNeurons)
                    {
                        SpikeService.SpikeCore(
                            new SpikeParameters(
                                target,
                                new SpikeOrigin(Guid.NewGuid()),
                                new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                                Array.Empty<FireInfo>(),
                                spikable,
                                (tea) => this.Triggered?.Invoke(this, tea),
                                (fea) => this.Fired?.Invoke(this, fea)
                            )
                        );
                    }
                }
            }
        }
        
        private static void SpikeCore(object? stateInfo)
        {
            SpikeParameters parameters = (SpikeParameters)stateInfo!;

            if (parameters.Spikable.SpikeHistory.TryGetAdd(parameters.Target.Id, id => new(), out SpikeInfo? spikeInfo))
            {
                spikeInfo!.Triggers.Clean((ti) => ti.Timestamp, parameters.Trigger.Timestamp.Subtract(Constants.Spiker.RefractoryPeriod));
                spikeInfo.Triggers.TryAdd(parameters.Trigger.Timestamp, parameters.Trigger);

                ICollection<TriggerInfo> triggers = spikeInfo.Triggers.Values;

                int sumCharge = SpikeService.GetSumCharge(triggers);

                parameters.TriggeredCallback(
                    new TriggeredEventArgs(
                        parameters.Spikable, 
                        parameters.Target, 
                        parameters.Origin, 
                        parameters.Trigger, 
                        sumCharge, 
                        parameters.Path
                    )
                );

                #region DEBUG
                // Debug.WriteLine($"EffectiveTriggers: {this.triggerInfos.Count()}; Sum charge: {positiveCharge} + {negativeCharge} = {sumCharge} ({this.Data})");
                #endregion

                // if spiked enough and spiked after repolarization
                if (
                    sumCharge >= Constants.Spiker.DefaultThreshold &&
                    (
                        !spikeInfo.LastFire.HasValue ||
                        DateTime.Now > spikeInfo.LastFire.Value.Timestamp.Add(Constants.Spiker.RefractoryPeriod)
                    )
                )
                {
                    var spikeResultingFireInfo = new FireInfo(parameters.Target, DateTime.Now, triggers.ToArray());
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    parameters.FiredCallback(
                        new FiredEventArgs(
                            parameters.Spikable,
                            spikeResultingFireInfo, 
                            sumCharge
                        )
                    );

                    if (parameters.Spikable.Network != null)
                        // Trigger postsynaptics
                        parameters.Spikable.Network.GetTerminals(parameters.Target.Id).ToList().ForEach(
                            t =>
                            {
                                var sp = new SpikeParameters(
                                    parameters.Spikable.Network.ValidateGet(t.PostsynapticNeuronId),
                                    parameters.Origin,
                                    new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                                    parameters.Path.Concat([spikeResultingFireInfo]),
                                    parameters.Spikable,
                                    parameters.TriggeredCallback,
                                    parameters.FiredCallback
                                );
                                if (!ThreadPool.QueueUserWorkItem(SpikeService.SpikeCore, sp))
                                    Debug.WriteLine($"Unable to queue work item for: {sp.Target}");
                            }
                        );
                }
            }
            else
                throw new InvalidOperationException($"SpikeInfo for neuron '{parameters.Target.Id}' was not found.");
        }

        private static int GetSumCharge(ICollection<TriggerInfo> tis)
        {
            var positiveCharge = SpikeService.GetChargeByEffect(tis, NeurotransmitterEffect.Excite);
            var negativeCharge = SpikeService.GetChargeByEffect(tis, NeurotransmitterEffect.Inhibit);
            var sumCharge = (int)(Constants.Spiker.RestingPotential + positiveCharge + negativeCharge);
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
