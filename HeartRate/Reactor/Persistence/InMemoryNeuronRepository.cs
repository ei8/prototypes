using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Graph.Domain.Model;

namespace Reactor.Persistence
{
    public class InMemoryNeuronRepository : INeuronRepository
    {
        private Dictionary<Guid, Neuron> cache;
        private NeuronCollection neurons;

        public enum EdgeDirection
        {
            Inbound,
            Outbound,
            Any
        }

        public InMemoryNeuronRepository(NeuronCollection neurons)
        {
            this.cache = new Dictionary<Guid, Neuron>();
            this.neurons = neurons;
        }

        public async Task Clear()
        {
            this.cache.Clear();
            this.neurons.Clear();
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<NeuronResult>> Get(Guid guid, Guid? centralGuid = null, RelativeType type = RelativeType.NotSet, bool shouldLoadTerminals = false, CancellationToken token = default(CancellationToken))
        {
            IEnumerable<NeuronResult> result = null;
            
            if (!centralGuid.HasValue)
                result = new NeuronResult[] { new NeuronResult() { Neuron = this.cache[guid] } };
            else
            {
                var temp = this.GetNeuronResults(centralGuid.Value, string.Empty, InMemoryNeuronRepository.ConvertRelativeToDirection(type));
                // TODO: optimize by passing this into GetNeuronResults AQL
                result = temp.Where(nr =>
                    (nr.Neuron != null && nr.Neuron.Id == guid.ToString()) ||
                    (nr.Terminal != null && nr.Terminal.TargetId == guid.ToString())
                    );
            }

            return await Task.FromResult(result);
        }

        private IEnumerable<NeuronResult> GetNeuronResults(Guid? centralGuid, string settingName, EdgeDirection direction, NeuronQuery neuronQuery = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            // IEnumerable<NeuronResult> result = null;

            //using (var db = ArangoDatabase.CreateWithSetting(settingName))
            //{
            //    var queryString = NeuronRepository.CreateQuery(centralGuid, direction, neuronQuery, limit, out List<QueryParameter> queryParameters);

            //    result = db.CreateStatement<NeuronResult>(queryString, queryParameters)
            //                .AsEnumerable()
            //                .ToArray();
            //}
            var ns = this.cache.Values.Where(n => centralGuid == null || n.Id == centralGuid.ToString() || n.Terminals.Any(t => t.TargetId == centralGuid.ToString()));
            var result = new List<NeuronResult>();
            ns.ToList().ForEach(n =>
            {
                if (centralGuid == null || n.Id == centralGuid.ToString())
                    result.Add(new NeuronResult() { Neuron = n });
                else
                    result.Add(new NeuronResult() { Neuron = n, Terminal = n.Terminals.First(t => t.TargetId == centralGuid.ToString()) });
            }
            );

            return result;
        }

        private static EdgeDirection ConvertRelativeToDirection(RelativeType type)
        {
            switch (type)
            {
                case RelativeType.Postsynaptic:
                    return EdgeDirection.Outbound;
                case RelativeType.Presynaptic:
                    return EdgeDirection.Inbound;
                default:
                    return EdgeDirection.Any;
            }
        }

        public async Task<Neuron> Get(Guid guid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await this.Get(guid, shouldLoadTerminals: true)).FirstOrDefault()?.Neuron;
        }

        public async Task<IEnumerable<NeuronResult>> GetAll(Guid? centralGuid = null, RelativeType type = RelativeType.NotSet, NeuronQuery neuronQuery = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            IEnumerable<NeuronResult> result = null;

            result = this.GetNeuronResults(
                centralGuid,
                string.Empty,
                InMemoryNeuronRepository.ConvertRelativeToDirection(type),
                neuronQuery,
                limit);

            return await Task.FromResult(result);
        }

        public async Task Initialize(string databaseName)
        {
            await Task.CompletedTask;
        }

        public async Task Remove(Neuron value, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.cache.Remove(Guid.Parse(value.Id));
            this.neurons.Remove(value.Id);
            await Task.CompletedTask;
        }

        public async Task Save(Neuron value, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.cache[Guid.Parse(value.Id)] = value;

            var result = new Neurons.Neuron(
            value.Id,
            value.Tag,
            value.Terminals.Select(t => new Neurons.Terminal(t.TargetId, (Neurons.NeurotransmitterEffect)(int)t.Effect, t.Strength)).ToArray()
            );

            if (this.neurons.Contains(value.Id))
            {
                var i = this.neurons.IndexOf(this.neurons[value.Id]);
                this.neurons[i] = result;
            }
            else
                this.neurons.Add(result);
            
            await Task.CompletedTask;
        }
    }
}
