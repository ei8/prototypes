using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SpikeVisualizationSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool enabled;

        public SpikeVisualizationSettings()
        {
            this.enabled = false;
            this.SustainTriggered = false;
            this.SustainFired = false;
            this.ResetPeriod = 2000;
            this.SustainPeriod = 2000;
        }

        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Enabled)));
                }
            }
        }

        public bool SustainTriggered { get; set; }

        public bool SustainFired { get; set; }

        public int ResetPeriod { get; set; }

        public int SustainPeriod { get; set; }
    }
}
