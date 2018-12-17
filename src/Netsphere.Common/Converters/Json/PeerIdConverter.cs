using System;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class PeerIdConverter : JsonConverter<PeerId>
    {
        public override void WriteJson(JsonWriter writer, PeerId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Value);
        }

        public override PeerId ReadJson(JsonReader reader, Type objectType, PeerId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<ushort>(reader);
            return new PeerId(value);
        }
    }
}
