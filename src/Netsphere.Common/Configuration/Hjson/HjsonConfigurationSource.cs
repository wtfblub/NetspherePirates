using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Netsphere.Common.Configuration.Hjson
{
    public class HjsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new HjsonConfigurationProvider(this);
        }
    }
}
