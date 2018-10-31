using System.IO;
using Hjson;
using Microsoft.Extensions.Configuration.Json;

namespace Netsphere.Common.Configuration.Hjson
{
    public class HjsonConfigurationProvider : JsonConfigurationProvider
    {
        public HjsonConfigurationProvider(JsonConfigurationSource source)
            : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            var hjson = HjsonValue.Load(stream);
            using (var jsonStream = new MemoryStream())
            {
                hjson.Save(jsonStream);
                jsonStream.Position = 0;
                base.Load(jsonStream);
            }
        }
    }
}
