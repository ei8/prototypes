using ei8.Cortex.Coding;
using System.Collections.Concurrent;

namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public class SpikingNeuron : ei8.Cortex.Coding.Neuron
    {
        public event EventHandler<TriggeredEventArgs>? Triggered;

        public event EventHandler<FiredEventArgs>? Fired;

        private readonly ConcurrentDictionary<DateTime, TriggerInfo> triggerInfos;
        private FireInfo? lastFireInfo;

        public SpikingNeuron(Guid id, string tag) : base(
            id,
            tag,
            string.Empty,
            Guid.Empty,
            string.Empty,
            DateTimeOffset.Now,
            Guid.Empty,
            string.Empty,
            DateTimeOffset.Now,
            Guid.Empty,
            string.Empty,
            string.Empty,
            int.MinValue
        )
        {
            this.triggerInfos = new ConcurrentDictionary<DateTime, TriggerInfo>();
            this.lastFireInfo = null;
        }

        public FireInfo Spike(SpikeOrigin spikeOrigin, TriggerInfo triggerInfo, IEnumerable<FireInfo> path)
        {
            FireInfo result = FireInfo.Empty;

            this.triggerInfos.Clean((ti) => ti.Timestamp, triggerInfo.Timestamp.Subtract(Constants.Spiker.RefractoryPeriod));
            this.triggerInfos.TryAdd(triggerInfo.Timestamp, triggerInfo);

            var tis = this.triggerInfos.Values;

            var positiveCharge = SpikingNeuron.GetChargeByEffect(tis, NeurotransmitterEffect.Excite);
            var negativeCharge = SpikingNeuron.GetChargeByEffect(tis, NeurotransmitterEffect.Inhibit);
            var sumCharge = (int) (Constants.Spiker.RestingPotential + positiveCharge + negativeCharge);

            this.Triggered?.Invoke(this, new TriggeredEventArgs(spikeOrigin, triggerInfo, sumCharge, path));

            #region DEBUG
            // Debug.WriteLine($"EffectiveTriggers: {this.triggerInfos.Count()}; Sum charge: {positiveCharge} + {negativeCharge} = {sumCharge} ({this.Data})");
            #endregion

            // if spiked enough and spiked after repolarization
            if (
                sumCharge >= Constants.Spiker.DefaultThreshold && 
                (
                    !this.lastFireInfo.HasValue || 
                    DateTime.Now > this.lastFireInfo.Value.Timestamp.Add(Constants.Spiker.RefractoryPeriod)
                )
            )
            {
                result = new FireInfo(DateTime.Now, tis.ToArray());
                this.lastFireInfo = result;
                this.Fired?.Invoke(this, new FiredEventArgs(result, sumCharge));
            }

            return result;
        }

        private static float GetChargeByEffect(IEnumerable<TriggerInfo> effectiveTriggers, NeurotransmitterEffect effect)
        {
            // sum triggers within the last refractory period
            return effectiveTriggers.Sum(
                ti => ti.Effect == effect ? 
                    Constants.Spiker.SpikeDepolarizationAmount * ti.Strength * ((int) effect) : 
                    0
                );
        }

        public override string ToString()
        {
            return $"{this.Id}:Neuron '{this.Tag}'";
        }
    }
}
