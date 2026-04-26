using ei8.Cortex.Coding.Mirrors;
using neurUL.Common.Domain.Model;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ei8.Prototypes.HelloWorm
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            using (JsonDocument jsonDocument =JsonDocument.Parse(File.ReadAllText("customSettings.json")))
            {
                JsonElement rootElement = jsonDocument.RootElement;
                JsonElement jsonElement = rootElement.GetProperty("Mirrors");
                string mirrorsString = jsonElement.ToString();
                var mirrors = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<MirrorConfig>>(mirrorsString);
                AssertionConcern.AssertStateTrue(mirrors != null, "Mirror Configs required.");

                this.Mirrors = mirrors!;
            }
        }

        public IEnumerable<MirrorConfig> Mirrors { get; set; }
    }
}
