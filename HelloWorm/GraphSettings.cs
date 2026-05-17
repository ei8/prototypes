using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.StressEnergy;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    public class GraphSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public SpikeVisualizationSettings SpikeVisualization { get; set; }
        private bool shortenMirrorTags;

        public bool ShortenMirrorTags
        {
            get => this.shortenMirrorTags;
            set
            {
                if (this.shortenMirrorTags != value)
                {
                    this.shortenMirrorTags = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShortenMirrorTags)));
                }
            }
        }


        public GraphSettings()
        {
            this.SpikeVisualization = new SpikeVisualizationSettings();
            this.ShortenMirrorTags = false;
        }
    }
}
