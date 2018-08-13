using System;
using System.Net;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            return IPAddress.Parse(str);
        }
    }
}
