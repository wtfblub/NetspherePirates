using System;
using System.Net;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class IPEndPointConverter : JsonConverter<IPEndPoint>
    {
        public override void WriteJson(JsonWriter writer, IPEndPoint value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override IPEndPoint ReadJson(JsonReader reader, Type objectType, IPEndPoint existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var arr = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new IPEndPoint(IPAddress.Parse(arr[0]), int.Parse(arr[1]));
        }
    }
}
