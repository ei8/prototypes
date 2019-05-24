using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace HeartRateAnalyzer
{
    public class Candidate : INotifyPropertyChanged
    {
        public enum StatusValue
        {
            New,
            Analyzing,
            Analyzed
        }

        public Candidate(Neuron value)
        {
            this.neuron = value;
            this.status = StatusValue.New;
        }

        private Neuron neuron;

        public event PropertyChangedEventHandler PropertyChanged;

        public Neuron Neuron
        {
            get => this.neuron;
            //set
            //{
            //    if (this.neuron != value)
            //    {
            //        this.neuron = value;
            //        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Neuron)));
            //    }
            //}
        }

        private StatusValue status;

        public StatusValue Status
        {
            get => this.status;
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }
    }
}
