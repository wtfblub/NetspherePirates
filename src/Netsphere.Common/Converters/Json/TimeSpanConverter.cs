using System;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (uint)value.TotalMilliseconds);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<uint>(reader);
            return TimeSpan.FromMilliseconds(value);
        }
    }
}
