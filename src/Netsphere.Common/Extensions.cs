using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Netsphere.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection This)
            where TImplementation : class, IService, TService
        {
            return This.AddSingleton(typeof(IService), x => x.GetService(typeof(TService)))
                .AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddService<TImplementation>(this IServiceCollection This)
            where TImplementation : class, IService
        {
            return This.AddSingleton<IService>(x => x.GetRequiredService<TImplementation>())
                .AddSingleton<TImplementation>();
        }

        public static IServiceCollection AddHostedServiceEx<TService, TImplementation>(this IServiceCollection This)
            where TImplementation : class, IHostedService, TService
        {
            return This.AddSingleton(typeof(IHostedService), x => x.GetService(typeof(TService)))
                .AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddHostedServiceEx<TImplementation>(this IServiceCollection This)
            where TImplementation : class, IHostedService
        {
            return This.AddSingleton<IHostedService>(x => x.GetRequiredService<TImplementation>())
                .AddSingleton<TImplementation>();
        }
    }

    public static class ObjectExtensions
    {
        public static string ToJson(this object This)
        {
            return JsonConvert.SerializeObject(This);
        }
    }

    public static class DnsEndPointExtensions
    {
        public static IPEndPoint ToIPEndPoint(this DnsEndPoint This)
        {
            var addresses = Dns.GetHostAddresses(This.Host);
            var address = addresses.FirstOrDefault(x => This.AddressFamily == AddressFamily.Unspecified ||
                                                        x.AddressFamily == This.AddressFamily);
            return new IPEndPoint(address, This.Port);
        }

        public static async Task<IPEndPoint> ToIPEndPointAsync(this DnsEndPoint This)
        {
            var addresses = await Dns.GetHostAddressesAsync(This.Host).AnyContext();
            var address = addresses.FirstOrDefault(x => This.AddressFamily == AddressFamily.Unspecified ||
                                                        x.AddressFamily == This.AddressFamily);
            return new IPEndPoint(address, This.Port);
        }
    }
}
