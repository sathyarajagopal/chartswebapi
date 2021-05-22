using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChartsWebAPI.Utils
{
    public sealed class SerializationHelper
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new JsonContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        }

        public static object DeSerializeObject(string json)
        {
            return JsonConvert.DeserializeObject<object>(json);
        }

        public sealed class JsonContractResolver : CamelCasePropertyNamesContractResolver { }
    }
}
