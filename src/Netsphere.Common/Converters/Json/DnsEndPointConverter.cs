using System;
using System.Net;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class DnsEndPointConverter : JsonConverter<DnsEndPoint>
    {
        public override void WriteJson(JsonWriter writer, DnsEndPoint value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, $"{value.Host}:{value.Port}");
        }

        public override DnsEndPoint ReadJson(JsonReader reader, Type objectType, DnsEndPoint existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var split = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
                throw new FormatException("Wrong format for DnsEndPoint");

            if (!ushort.TryParse(split[1], out var port))
                throw new FormatException("Wrong format for DnsEndPoint");

            return new DnsEndPoint(split[0], port);
        }
    }
}
