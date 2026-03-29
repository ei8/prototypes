using ei8.Prototypes.HelloWorm;
using ei8.Prototypes.HelloWorm.Spiker.Neurons;
using System.Diagnostics;

namespace HelloWorld.Spiker.Spikes
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

        public SpikeService()
        {
        }

        public void SetSpikeCount(int value)
        {
            this.spikeCount = value;
        }

        public void Spike(IEnumerable<SpikeTarget> targets, NeuronCollection neurons)
        {
            this.Spiking?.Invoke(this, EventArgs.Empty);

            for (int i = 1; i <= this.spikeCount; i++)
            {
                var nts = targets.Select(st => neurons[st.Id]);
                foreach (Neuron target in nts)
                {
                    SpikeService.SpikeNeuron(
                        new SpikeParameters()
                        {
                            Target = target,
                            Origin = new SpikeOrigin(NeuronHelper.GetNewShortGuid()),
                            Trigger = new TriggerInfo(DateTime.Now, Constants.Spiker.NeurotransmitterEffect.Excite, 1f, "User"),
                            Path = new FireInfo[0],
                            Neurons = neurons
                        }
                        );
                }
            }
        }

        private static void SpikeNeuron(object? stateInfo)
        {
            SpikeParameters parameters = (SpikeParameters)stateInfo!;
            var spikeResultingFireInfo = parameters.Target.Spike(parameters.Origin, parameters.Trigger, parameters.Path);
            if (spikeResultingFireInfo != FireInfo.Empty)
            {
                parameters.Target.Terminals.ToList().ForEach(
                    t =>
                    {
                        var sp = new SpikeParameters()
                        {
                            Target = parameters.Neurons[t.TargetId],
                            Origin = parameters.Origin,
                            Trigger = new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                            Path = parameters.Path.Concat(new FireInfo[] { spikeResultingFireInfo }),
                            Neurons = parameters.Neurons
                        };
                        if (!ThreadPool.QueueUserWorkItem(SpikeService.SpikeNeuron, sp))
                            Debug.WriteLine("Unable to queue work item for: " + sp.Target.Id + ":" + sp.Target.Data);
                    }
                );
            }
        }
    }
}
