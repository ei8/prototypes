using Reactor.Neurons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Reactor.Spikes
{
    public class SpikeService : ISpikeService
    {
        private class SpikeParameters
        {
            public Neuron Target { get; set; }

            public SpikeOrigin Origin { get; set; }

            public TriggerInfo Trigger { get; set; }

            public IEnumerable<FireInfo> Path { get; set; }

            public NeuronCollection Neurons { get; set; }
        }
        
        public event EventHandler Spiking;

        private int spikeCount = 1;
        private NeuronCollection neurons;

        public SpikeService(NeuronCollection neurons)
        {
            this.neurons = neurons;
        }

        public void SetSpikeCount(int value)
        {
            this.spikeCount = value;
        }

        public void Spike(IEnumerable<SpikeTarget> targets)
        {
            this.Spiking?.Invoke(this, EventArgs.Empty);

            for (int i = 1; i <= this.spikeCount; i++)
            {
                var nts = targets.Select(st => this.neurons[st.Id]);
                foreach (Neuron target in nts)
                {
                    SpikeService.SpikeNeuron(
                        new SpikeParameters()
                        {
                            Target = target,
                            Origin = new SpikeOrigin(Helper.GetNewShortGuid()),
                            Trigger = new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, "User"),
                            Path = new FireInfo[0],
                            Neurons = this.neurons
                        }
                        );
                }
            }
        }

        private static void SpikeNeuron(object stateInfo)
        {
            SpikeParameters parameters = (SpikeParameters)stateInfo;
            var spikeResultingFireInfo = parameters.Target.Spike(parameters.Origin, parameters.Trigger, parameters.Path);
            if (spikeResultingFireInfo != FireInfo.Empty)
            {
                parameters.Target.Terminals.ToList().ForEach(
                    t => ThreadPool.QueueUserWorkItem(SpikeService.SpikeNeuron, new SpikeParameters()
                        {
                            Target = parameters.Neurons[t.TargetId],
                            Origin = parameters.Origin,
                            Trigger = new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                            Path = parameters.Path.Concat(new FireInfo[] { spikeResultingFireInfo }),
                            Neurons = parameters.Neurons
                        }
                    )
                );
            }
        }
    }
}
