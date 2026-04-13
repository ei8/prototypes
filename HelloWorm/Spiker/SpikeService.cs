using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm;
using ei8.Prototypes.HelloWorm.Spiker;
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
            SpikeService spikeService
        )
        {
            public Neuron Target { get; } = target;

            public SpikeOrigin Origin { get; } = origin;

            public TriggerInfo Trigger { get; } = trigger;

            public IEnumerable<FireInfo> Path { get; } = path;

            public SpikeService SpikeService { get; } = spikeService;
        }
        
        private int spikeCount = 1;
        private readonly Network network;
        private readonly ConcurrentDictionary<Guid, SpikeInfo> spikes;

        public SpikeService(Network network)
        {
            this.network = network;
            this.spikes = new ConcurrentDictionary<Guid, SpikeInfo>();
        }

        public void SetSpikeCount(int value)
        {
            spikeCount = value;
        }

        public void Spike(IEnumerable<Guid> targets)
        {
            for (int i = 1; i <= spikeCount; i++)
            {
                var targetNeurons = targets.Select(network.ValidateGet);

                foreach (Neuron target in targetNeurons)
                {
                    SpikeService.SpikeNeuron(
                        new SpikeParameters(
                            target,
                            new SpikeOrigin(Guid.NewGuid()),
                            new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                            Array.Empty<FireInfo>(),
                            this
                        )
                    );
                }
            }
        }

        private static void SpikeNeuron(object? stateInfo)
        {
            SpikeParameters parameters = (SpikeParameters)stateInfo!;

            if (!parameters.SpikeService.spikes.ContainsKey(parameters.Target.Id))
                parameters.SpikeService.spikes.TryAdd(parameters.Target.Id, new SpikeInfo(parameters.Target.Id));

            if (parameters.SpikeService.spikes.TryGetValue(parameters.Target.Id, out SpikeInfo? spikeInfo))
            {
                var spikeResultingFireInfo = FireInfo.Empty;

                spikeInfo.Triggers.Clean((ti) => ti.Timestamp, parameters.Trigger.Timestamp.Subtract(Constants.Spiker.RefractoryPeriod));
                spikeInfo.Triggers.TryAdd(parameters.Trigger.Timestamp, parameters.Trigger);

                var tis = spikeInfo.Triggers.Values;

                var positiveCharge = SpikeService.GetChargeByEffect(tis, NeurotransmitterEffect.Excite);
                var negativeCharge = SpikeService.GetChargeByEffect(tis, NeurotransmitterEffect.Inhibit);
                var sumCharge = (int)(Constants.Spiker.RestingPotential + positiveCharge + negativeCharge);

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
                    spikeResultingFireInfo = new FireInfo(DateTime.Now, tis.ToArray());
                    spikeInfo.LastFire = spikeResultingFireInfo;
                    parameters.SpikeService.Fired?.Invoke(
                        parameters.SpikeService,
                        new FiredEventArgs(parameters.Target, spikeResultingFireInfo, sumCharge)
                    );
                }

                if (spikeResultingFireInfo != FireInfo.Empty)
                {
                    parameters.SpikeService.network.GetTerminals(parameters.Target.Id).ToList().ForEach(
                        t =>
                        {
                            var sp = new SpikeParameters(
                                parameters.SpikeService.network.ValidateGet(t.PostsynapticNeuronId),
                                parameters.Origin,
                                new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                                parameters.Path.Concat([spikeResultingFireInfo]),
                                parameters.SpikeService
                            );
                            if (!ThreadPool.QueueUserWorkItem(SpikeNeuron, sp))
                                Debug.WriteLine($"Unable to queue work item for: {sp.Target}");
                        }
                    );
                }
            }
            else
                throw new InvalidOperationException($"SpikeInfo for neuron '{parameters.Target.Id}' was not found.");
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
