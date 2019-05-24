using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Reactor.Neurons
{
    public class Neuron
    {
        public Neuron(string id, string data, params Terminal[] terminals) : this(id, data, Constants.DefaultThreshold, terminals)
        {
        }

        public Neuron(string id, string data, int threshold, params Terminal[] terminals)
        {
            this.Id = id;
            this.Data = data;
            this.Threshold = threshold;
            this.triggerInfos = new List<TriggerInfo>();
            this.fireInfos = new List<FireInfo>();
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

        private IList<TriggerInfo> triggerInfos;

        private IList<FireInfo> fireInfos;

        public FireInfo Spike(SpikeOrigin spikeOrigin, TriggerInfo triggerInfo, IEnumerable<FireInfo> path)
        {
            FireInfo result = FireInfo.Empty;

            this.triggerInfos.Add(triggerInfo);
            // Note: this.triggerInfos.ToList() required to fix "collection modified" issue
            var effectiveTriggers = Neuron.GetEffectiveTriggers(this.triggerInfos.ToList());
            var positiveCharge = Neuron.GetChargeByEffect(effectiveTriggers, NeurotransmitterEffect.Excite);
            var negativeCharge = Neuron.GetChargeByEffect(effectiveTriggers, NeurotransmitterEffect.Inhibit);
            var sumCharge = (int) (Constants.RestingPotential + positiveCharge + negativeCharge);
            
            this.Triggered?.Invoke(this, new TriggeredEventArgs(spikeOrigin, triggerInfo, sumCharge, path));

            // if spiked enough and spiked after repolarization
            if (sumCharge >= this.Threshold && DateTime.Now > this.fireInfos.LastOrDefault().Timestamp.Add(Constants.RefractoryPeriod))
            {
                result = new FireInfo(DateTime.Now, effectiveTriggers.ToArray());
                this.fireInfos.Add(result);
                this.triggerInfos.Clear();
                this.Fired?.Invoke(this, new FiredEventArgs(result, sumCharge));            
            }

            return result;
        }

        private static float GetChargeByEffect(IEnumerable<TriggerInfo> effectiveTriggers, NeurotransmitterEffect effect)
        {
            // sum triggers within the last refractory period
            return effectiveTriggers.Sum(
                ti => ti.Effect == effect ? 
                    Constants.SpikeDepolarizationAmount * ti.Strength * ((int) effect) : 
                    0
                );
        }

        private static IEnumerable<TriggerInfo> GetEffectiveTriggers(IList<TriggerInfo> triggerInfos)
        {
            return triggerInfos.Where(d => d.Timestamp > DateTime.Now.Subtract(Constants.RefractoryPeriod));
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
