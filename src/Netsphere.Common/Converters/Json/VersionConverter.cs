using System;
using Newtonsoft.Json;

namespace Netsphere.Common.Converters.Json
{
    public class VersionConverter : JsonConverter<Version>
    {
        public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            return Version.Parse(str);
        }
    }
}
