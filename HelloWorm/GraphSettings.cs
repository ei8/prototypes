using ei8.Cortex.Coding;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ei8.Prototypes.HelloWorm
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GraphSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool shortenMirrorTags;
        private IEnumerable<Neuron> filterNeurons;
        private IEnumerable<Neuron> hideTagsNeurons;

        public GraphSettings()
        {
            this.SustainTriggered = false;
            this.SustainFired = false;
            this.ResetPeriod = 2000;
            this.SustainPeriod = 2000;
            this.ShortenMirrorTags = false;

            this.ResetFilters();
        }

        [MemberNotNull(nameof(filterNeurons), nameof(hideTagsNeurons))]
        public void ResetFilters()
        {
            this.FilterNeurons = Enumerable.Empty<Neuron>();
            this.HideTagsNeurons = Enumerable.Empty<Neuron>();
        }

        [Browsable(false)]
        public IEnumerable<Neuron> HideTagsNeurons
        {
            get => this.hideTagsNeurons;
            set
            {
                if (this.hideTagsNeurons !=  value)
                {
                    this.hideTagsNeurons = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HideTagsNeurons)));
                }
            }
        }

        [Browsable(false)]
        public IEnumerable<Neuron> FilterNeurons
        {
            get => this.filterNeurons;
            set
            {
                if (this.filterNeurons !=  value)
                {
                    this.filterNeurons = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterNeurons)));
                }
            }
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
