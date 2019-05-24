using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Settings;

namespace HeartRateAnalyzer
{
    public class DependencyService : IDependencyService
    {
        private ISettingsServiceImplementation settingsServiceImplementation;

        public T Get<T>() where T : class
        {
            T result = default(T);
            if (typeof(T) == typeof(ISettingsServiceImplementation))
            {
                if (this.settingsServiceImplementation == null)
                {
                    this.settingsServiceImplementation = new SettingsServiceImplementation();
                }
                result = (T) this.settingsServiceImplementation;
            }
            return result;
        }
    }
}
