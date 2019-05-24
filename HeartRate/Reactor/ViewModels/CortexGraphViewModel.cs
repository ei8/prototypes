using ReactiveUI;
using Reactor.Neurons;
using Reactor.ResultMarkers;
using Reactor.SpikeResults;
using Reactor.Spikes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Graph.Domain.Model;

namespace Reactor.ViewModels
{
    public class CortexGraphViewModel : ReactiveObject
    {
        private string avatarUri;
        private NeuronCollection neurons;
        private INotificationLogClient notificationLogClient;
        private INeuronRepository neuronRepository; 
        private TreeModel treeModel;
        private ISpikeTargetListService spikeTargetListService;
        private IResultMarkerService resultMarkerService;

        public CortexGraphViewModel(NeuronCollection neurons, INotificationLogClient notificationLogClient, IRepository<works.ei8.Cortex.Graph.Domain.Model.Neuron> neuronRepository, ISpikeTargetListService spikeTargetListService, IResultMarkerService resultMarkerService)
        {
            this.AvatarUri = Properties.Settings.Default.AvatarUri;
            this.neurons = neurons;
            this.notificationLogClient = notificationLogClient;
            this.neuronRepository = (INeuronRepository) neuronRepository;
            this.spikeTargetListService = spikeTargetListService;
            this.resultMarkerService = resultMarkerService;

            this.ReloadCommand = ReactiveCommand.Create(this.Reload, this.WhenAnyValue(vm => vm.AvatarUri).Select(s => !string.IsNullOrEmpty(s)));
            this.RenderCommand = ReactiveCommand.Create(this.Render, this.WhenAnyValue(vm => vm.AvatarUri).Select(s => !string.IsNullOrEmpty(s)));
        }

        public string AvatarUri
        {
            get => this.avatarUri;
            set => this.RaiseAndSetIfChanged(ref this.avatarUri, value);
        }

        public TreeModel TreeModel
        {
            get => this.treeModel;
            set => this.RaiseAndSetIfChanged(ref this.treeModel, value);
        }

        public ReactiveCommand ReloadCommand { get; }
        public ReactiveCommand RenderCommand { get; }

        private async void Reload()
        {
            this.notificationLogClient.Initialize(this.avatarUri);
            await this.notificationLogClient.Regenerate();
        }

        private async void Render()
        {
            this.TreeModel = new TreeModel(this.neurons) { RootId = string.Empty };

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SpikeTargets))
                Properties.Settings.Default.SpikeTargets.Split(',').ToList().ForEach(s => this.spikeTargetListService.Add(new SpikeTarget(s.Trim())));

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ResultMarkers))
                Properties.Settings.Default.ResultMarkers.Split(',').ToList().ForEach(s => this.resultMarkerService.Add(new ResultMarker(s.Trim())));
        }
    }
}
