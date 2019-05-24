using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor
{
    // TODO: Separate neurul.common from neurul.common.events and use neurul.common.jsonhelper instead
    public static class JsonHelper
    {
        private const string MissingValueMessage = "Required value not found.";

        public static T GetRequiredValue<T>(JObject jObject, string dataId)
        {
            T result = default(T);

            if (jObject.TryGetValue(dataId, out JToken jt))
                result = jt.Value<T>();
            else
                throw new ArgumentException(JsonHelper.MissingValueMessage, dataId);

            return result;
        }

        public static T GetRequiredValue<T>(JToken jToken, string dataId)
        {
            T result = default(T);
            var target = jToken.SelectToken(dataId);
            if (target != null)
                result = target.Value<T>();
            else
                throw new ArgumentException(JsonHelper.MissingValueMessage, dataId);

            return result;
        }

        public static JEnumerable<JToken> GetRequiredChildren(JObject jObject, string dataId)
        {
            JEnumerable<JToken> result = new JEnumerable<JToken>();

            if (jObject.TryGetValue(dataId, out JToken jt))
                result = jt.Children();
            else
                throw new ArgumentException("Required children not found.", dataId);

            return result;
        }

        public static JObject JObjectParse(string value)
        {
            JsonReader reader = new JsonTextReader(new StringReader(value));
            reader.DateParseHandling = DateParseHandling.None;
            return JObject.Load(reader);
        }
    }
}
