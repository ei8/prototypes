using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            this.GraphPath = "/cortex/graph/neurons";
        }

        public string GraphPath { get; set; }
    }
}
