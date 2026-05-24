using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.StressEnergy;
using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GraphSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool shortenMirrorTags;

        public GraphSettings()
        {
            this.SustainTriggered = false;
            this.SustainFired = false;
            this.ResetPeriod = 2000;
            this.SustainPeriod = 2000;
            this.ShortenMirrorTags = false;
        }

        [Category(nameof(Constants.PropertyCategory.Visualization))]
        public bool SustainTriggered { get; set; }

        [Category(nameof(Constants.PropertyCategory.Visualization))]
        public bool SustainFired { get; set; }

        [Category(nameof(Constants.PropertyCategory.Visualization))]
        public int ResetPeriod { get; set; }

        [Category(nameof(Constants.PropertyCategory.Visualization))]
        public int SustainPeriod { get; set; }

        [Category(nameof(Constants.PropertyCategory.Appearance))]
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
    }
}
