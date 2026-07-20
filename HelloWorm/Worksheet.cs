using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Spiker;
using neurUL.Common.Domain.Model;
using NLog;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public class Worksheet : ICreatable, ILocated, ISpikableReporting, ISpikable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<DateTime, FireInfo> fireHistory;
        private readonly ISpikeService spikeService;
        private Network? network;
        private string name;

        public float ProcessingRatio => 1f;

        public IDictionary<DateTime, FireInfo> FireHistory => this.fireHistory;

        public TimeSpan RefractoryPeriod { get; set; }
        public TimeSpan RelatedSpikesPeriod { get; set; }

        public Network? Network => this.network;

        public Point Location { get; set; }
        public required IComposite Parent { get; set; }
        public required string Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Worm.Name)));
                }
            }
        }

        public event EventHandler<TriggeredEventArgs>? Triggered;
        public event EventHandler<FiredEventArgs>? Fired;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Worksheet(ISpikeService spikeService)
        {
            this.RefractoryPeriod = Constants.Worm.InitialRefractoryPeriod;
            this.RelatedSpikesPeriod = Constants.Worm.InitialRelatedSpikesPeriod;
            this.network = null;
            this.Location = new Point(0, 0);
            this.spikeService = spikeService;
            this.SubscribeReporting(
                this.spikeService,
                Worksheet.logger,
                (s, e) => this.Triggered?.Invoke(this, e),
                (s, e) => this.Fired?.Invoke(this, e)
            );
        }

        public void Initialize(IEnumerable<MirrorConfig>? mirrorConfigs)
        {
            AssertionConcern.AssertArgumentNotNull(mirrorConfigs, nameof(mirrorConfigs));
        }

        public void Initialize(Network? network)
        {
            if (this.network != network)
            {
                AssertionConcern.AssertArgumentNotNull(network, nameof(network));

                this.network = network;
            }
        }

        public void Initialize(string name, IRectangularComposite parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        public void Spike(params Neuron[] neurons)
        {
            if (this.network != null)
            {
                this.spikeService.Spike(
                    neurons,
                    this.network,
                    this.RefractoryPeriod
                );
            }
        }
    }
}
