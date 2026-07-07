using ei8.Cortex.Coding.Spiker;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public class Graph : IGraph
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler Reloaded;
        public event EventHandler<ActivityLoggedEventArgs> ActivityLogged;

        private readonly ConcurrentDictionary<Guid, NeuronSpikeInfo> spikes;
        private readonly GraphSettings settings;
        private bool isPlaying;

        public Graph(ISpikableReporting2 spikable)
        {
            this.Spikable = spikable;
            this.Spikable.Triggered += this.Spikable_Triggered;
            this.Spikable.Fired += this.Spikable_Fired;

            this.spikes = new();
            this.settings = new GraphSettings();

            this.settings.ShortenMirrorTags = true;
            this.settings.SustainTriggered = false;
            this.settings.SustainFired = true;

            this.isPlaying = true;
            this.PropertyChanged += this.Graph_PropertyChanged;
        }

        private void Graph_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.IsPlaying):
                    if (this.IsPlaying)
                        this.spikes.Clear();
                    break;
            }
        }

        public ISpikableReporting2 Spikable { get; }

        public GraphSettings Settings => this.settings;

        private void OnActivityLogged(ActivityLoggedEventArgs e)
        {
            if (
                this.ActivityLogged != null &&
                this.spikes.TryGetAdd(
                    e.NeuronId, 
                    id => new NeuronSpikeInfo(id, NeuronStatusValue.NotSet, DateTime.Now), 
                    out NeuronSpikeInfo? sui
                )
            )
            {
                sui.Status = e.NewStatus;
                sui.Timestamp = DateTime.Now;
                this.ActivityLogged(this, e);
            }
        }

        private void Spikable_Fired(object? sender, FiredEventArgs e)
        {
            if (this.isPlaying)
                this.OnActivityLogged(
                    new ActivityLoggedEventArgs(
                        e.FireInfo.Triggers.Select(t => t.PresynapticId),
                        e.FireInfo.Target.Id,
                        NeuronStatusValue.Fired
                    )
                );
        }

        private void Spikable_Triggered(object? sender, TriggeredEventArgs e)
        {
            if (this.isPlaying)
                this.OnActivityLogged(
                    new ActivityLoggedEventArgs(
                        e.ReflexArc.Any() ? [e.ReflexArc.Last().Target.Id] : Enumerable.Empty<Guid>(),
                        e.Target.Id,
                        NeuronStatusValue.Triggered
                    )
                );
        }

        public void ProcessTick()
        {
            if (this.isPlaying)
            {
                var activeSpikes = this.spikes.Where(s => s.Value.Status != NeuronStatusValue.NotSet);

                foreach (var spike in activeSpikes)
                {
                    if (spike.Value.Status == NeuronStatusValue.FiredPreviously || spike.Value.Status == NeuronStatusValue.TriggeredPreviously)
                    {
                        if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > (
                                (this.settings.ResetPeriod + this.settings.SustainPeriod) / 1000
                            )
                        )
                            this.OnActivityLogged(new ActivityLoggedEventArgs(Enumerable.Empty<Guid>(), spike.Key, NeuronStatusValue.NotSet));
                    }
                    else if (DateTime.Now.Subtract(spike.Value.Timestamp).TotalSeconds > (this.settings.ResetPeriod / 1000))
                    {
                        if (spike.Value.Status == NeuronStatusValue.Triggered && this.settings.SustainTriggered)
                            this.OnActivityLogged(new ActivityLoggedEventArgs(Enumerable.Empty<Guid>(), spike.Key, NeuronStatusValue.TriggeredPreviously));
                        else if (spike.Value.Status == NeuronStatusValue.Fired && this.settings.SustainFired)
                            this.OnActivityLogged(new ActivityLoggedEventArgs(Enumerable.Empty<Guid>(), spike.Key, NeuronStatusValue.FiredPreviously));
                        else
                            this.OnActivityLogged(new ActivityLoggedEventArgs(Enumerable.Empty<Guid>(), spike.Key, NeuronStatusValue.NotSet));
                    }
                }
            }
        }

        public bool IsPlaying => this.isPlaying;

        public void Play()
        {
            if (!this.isPlaying)
            {
                this.isPlaying = true;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsPlaying)));
            }
        }

        public void Pause()
        {
            if (this.isPlaying)
            {
                this.isPlaying = false;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsPlaying)));
            }
        }

        public void Reload()
        {
            this.Reloaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
