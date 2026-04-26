using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm;
using System.Collections.Concurrent;
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
            SpikeService spikeService,
            Network network
        )
        {
            public Neuron Target { get; } = target;

            public SpikeOrigin Origin { get; } = origin;

            public TriggerInfo Trigger { get; } = trigger;

            public IEnumerable<FireInfo> Path { get; } = path;

            public SpikeService SpikeService { get; } = spikeService;

            public Network Network { get; } = network;
        }
        
        private int spikeCount = 1;

        // TODO: transfer to Worm to make SpikeService stateless
        private readonly ConcurrentDictionary<Guid, SpikeInfo> spikes;

        public SpikeService()
        {
            this.spikes = new ConcurrentDictionary<Guid, SpikeInfo>();
        }

        public void SetSpikeCount(int value)
        {
            spikeCount = value;
        }

        public void Spike(IEnumerable<Guid> targets, Network network)
        {
            for (int i = 1; i <= spikeCount; i++)
            {
                var targetNeurons = targets.Select(network.ValidateGet);

                foreach (Neuron target in targetNeurons)
                {
                    SpikeService.SpikeCore(
                        new SpikeParameters(
                            target,
                            new SpikeOrigin(Guid.NewGuid()),
                            new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                            Array.Empty<FireInfo>(),
                            this,
                            network
                        )
                    );
                }
            }
        }

        private static void SpikeCore(object? stateInfo)
        {
            SpikeParameters parameters = (SpikeParameters)stateInfo!;

            if (parameters.SpikeService.spikes.TryGetAdd(parameters.Target.Id, id => new SpikeInfo(id), out SpikeInfo? spikeInfo))
            {
                var spikeResultingFireInfo = FireInfo.Empty;

                spikeInfo!.Triggers.Clean((ti) => ti.Timestamp, parameters.Trigger.Timestamp.Subtract(Constants.Spiker.RefractoryPeriod));
                spikeInfo.Triggers.TryAdd(parameters.Trigger.Timestamp, parameters.Trigger);

                ICollection<TriggerInfo> triggers = spikeInfo.Triggers.Values;

                int sumCharge = SpikeService.GetSumCharge(triggers);

                parameters.SpikeService.Triggered?.Invoke(
                    parameters.SpikeService,
                    new TriggeredEventArgs(parameters.Target, parameters.Origin, parameters.Trigger, sumCharge, parameters.Path)
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
                    spikeResultingFireInfo = new FireInfo(DateTime.Now, triggers.ToArray());
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    parameters.SpikeService.Fired?.Invoke(
                        parameters.SpikeService,
                        new FiredEventArgs(parameters.Target, spikeResultingFireInfo, sumCharge)
                    );

                    parameters.Network.GetTerminals(parameters.Target.Id).ToList().ForEach(
                        t =>
                        {
                            var sp = new SpikeParameters(
                                parameters.Network.ValidateGet(t.PostsynapticNeuronId),
                                parameters.Origin,
                                new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                                parameters.Path.Concat([spikeResultingFireInfo]),
                                parameters.SpikeService,
                                parameters.Network
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
