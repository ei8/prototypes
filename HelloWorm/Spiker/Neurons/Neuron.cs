using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public class Neuron
    {
        public Neuron(string id, string data, params Terminal[] terminals) : this(id, data, Constants.Spiker.DefaultThreshold, terminals)
        {
        }

        public Neuron(string id, string data, int threshold, params Terminal[] terminals)
        {
            this.Id = id;
            this.Data = data;
            this.Threshold = threshold;
            this.triggerInfos = new ConcurrentDictionary<DateTime, TriggerInfo>();
            this.lastFireInfo = null;
            this.terminals = new List<Terminal>(terminals);
        }

        public event EventHandler<TriggeredEventArgs> Triggered;

        public event EventHandler<FiredEventArgs> Fired;

        private List<Terminal> terminals;

        [DisplayName("Threshold (mV)")]
        public int Threshold { get; set; }

        public string Data { get; set; }

        [ParenthesizePropertyName(true)]
        public string Id { get; set; }

        public List<Terminal> Terminals
        {
            get
            {
                return this.terminals;
            }
            set
            {
                this.terminals = value;
            }
        }

        private ConcurrentDictionary<DateTime, TriggerInfo> triggerInfos;

        private FireInfo? lastFireInfo;

        public FireInfo Spike(SpikeOrigin spikeOrigin, TriggerInfo triggerInfo, IEnumerable<FireInfo> path)
        {
            FireInfo result = FireInfo.Empty;

            this.triggerInfos.Clean((ti) => ti.Timestamp, triggerInfo.Timestamp.Subtract(Constants.Spiker.RefractoryPeriod));
            this.triggerInfos.TryAdd(triggerInfo.Timestamp, triggerInfo);

            var tis = this.triggerInfos.Values;

            var positiveCharge = Neuron.GetChargeByEffect(tis, Constants.Spiker.NeurotransmitterEffect.Excite);
            var negativeCharge = Neuron.GetChargeByEffect(tis, Constants.Spiker.NeurotransmitterEffect.Inhibit);
            var sumCharge = (int) (Constants.Spiker.RestingPotential + positiveCharge + negativeCharge);

            this.Triggered?.Invoke(this, new TriggeredEventArgs(spikeOrigin, triggerInfo, sumCharge, path));

            #region DEBUG
            // Debug.WriteLine($"EffectiveTriggers: {this.triggerInfos.Count()}; Sum charge: {positiveCharge} + {negativeCharge} = {sumCharge} ({this.Data})");
            #endregion

            // if spiked enough and spiked after repolarization
            if (sumCharge >= this.Threshold && (!this.lastFireInfo.HasValue || DateTime.Now > this.lastFireInfo.Value.Timestamp.Add(Constants.Spiker.RefractoryPeriod)))
            {
                result = new FireInfo(DateTime.Now, tis.ToArray());
                this.lastFireInfo = result;
                this.Fired?.Invoke(this, new FiredEventArgs(result, sumCharge));
            }

            return result;
        }

        private static float GetChargeByEffect(IEnumerable<TriggerInfo> effectiveTriggers, Constants.Spiker.NeurotransmitterEffect effect)
        {
            // sum triggers within the last refractory period
            return effectiveTriggers.Sum(
                ti => ti.Effect == effect ? 
                    Constants.Spiker.SpikeDepolarizationAmount * ti.Strength * ((int) effect) : 
                    0
                );
        }

        public void AddTerminal(Terminal value)
        {
            this.terminals.Add(value);
        }

        public override string ToString()
        {
            return $"{this.Id}:Neuron '{this.Data}'";
        }
    }
}
