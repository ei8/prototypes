using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm.Spiker.Neurons;
using System.Diagnostics;

namespace ei8.Prototypes.HelloWorm.Spiker.Spikes
{
    public class SpikeService : ISpikeService
    {
        private class SpikeParameters
        {
            public SpikingNeuron Target { get; set; }

            public SpikeOrigin Origin { get; set; }

            public TriggerInfo Trigger { get; set; }

            public IEnumerable<FireInfo> Path { get; set; }

            public Network Network { get; set; }
        }
        
        private int spikeCount = 1;
        private readonly Network network;

        public SpikeService(Network network)
        {
            this.network = network;
        }

        public void SetSpikeCount(int value)
        {
            this.spikeCount = value;
        }

        public void Spike(IEnumerable<Guid> targets)
        {
            for (int i = 1; i <= this.spikeCount; i++)
            {
                var targetNeurons = targets.Select(this.network.ValidateGet);

                foreach (SpikingNeuron target in targetNeurons)
                {
                    SpikeService.SpikeNeuron(
                        new SpikeParameters()
                        {
                            Target = target,
                            Origin = new SpikeOrigin(Guid.NewGuid()),
                            Trigger = new TriggerInfo(DateTime.Now, NeurotransmitterEffect.Excite, 1f, Guid.Empty),
                            Path = Array.Empty<FireInfo>(),
                            Network = this.network
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
                parameters.Network.GetTerminals(parameters.Target.Id).ToList().ForEach(
                    t =>
                    {
                        var target = parameters.Network.ValidateGet(t.PostsynapticNeuronId);
                        var sp = new SpikeParameters()
                        {
                            Target = target,
                            Origin = parameters.Origin,
                            Trigger = new TriggerInfo(DateTime.Now, t.Effect, t.Strength, parameters.Target.Id),
                            Path = parameters.Path.Concat([ spikeResultingFireInfo ]),
                            Network = parameters.Network
                        };
                        if (!ThreadPool.QueueUserWorkItem(SpikeService.SpikeNeuron, sp))
                            Debug.WriteLine($"Unable to queue work item for: {sp.Target}");
                    }
                );
            }
        }
    }
}
