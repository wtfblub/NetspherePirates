using System;
using Microsoft.Extensions.Hosting;

namespace ProudNet.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseProudNetServer(this IHostBuilder This, Action<IProudNetServerBuilder> builder)
        {
            builder(new ProudNetServerBuilder(This));
            return This;
        }
    }
}
