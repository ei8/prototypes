using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Graph.Domain.Model;

namespace Reactor.Persistence
{
    public class InMemorySettingsRepository : IRepository<Settings>
    {
        private Settings cache;

        public async Task Clear()
        {
            this.cache = new Settings();
            await Task.CompletedTask;
        }

        public async Task<Settings> Get(Guid guid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.FromResult(this.cache);
        }

        public async Task Initialize(string databaseName)
        {
            await Task.CompletedTask;
        }

        public Task Remove(Settings value, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task Save(Settings value, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.cache = value;
            await Task.CompletedTask;
        }
    }
}
