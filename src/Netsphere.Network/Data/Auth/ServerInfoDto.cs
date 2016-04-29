using System.Net;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Auth
{
    [BlubContract]
    public class ServerInfoDto
    {
        [BlubMember(0)]
        public bool IsEnabled { get; set; } // ?

        [BlubMember(1)]
        public uint Id { get; set; }

        [BlubMember(2)]
        public ServerType Type { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(4)]
        public ushort PlayerLimit { get; set; }

        [BlubMember(5)]
        public ushort PlayerOnline { get; set; }

        [BlubMember(6, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(7)]
        public ushort GroupId { get; set; }

        public ServerInfoDto()
        {
            IsEnabled = true;
            Name = "";
        }
    }
}
