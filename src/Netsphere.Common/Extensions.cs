using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;

namespace Netsphere.Common
{
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
