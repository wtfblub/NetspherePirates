using System.Net;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Auth
{
    public class ServerInfoDto
    {
        [Serialize(0)]
        public bool IsEnabled { get; set; } // ?

        [Serialize(1)]
        public uint Id { get; set; }

        [Serialize(2, Compiler = typeof(EnumSerializer))]
        public ServerType Type { get; set; }

        [Serialize(3, Compiler = typeof(StringSerializer))]
        public string Name { get; set; }

        [Serialize(4)]
        public ushort PlayerLimit { get; set; }

        [Serialize(5)]
        public ushort PlayerOnline { get; set; }

        [Serialize(6, Compiler = typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [Serialize(7)]
        public ushort GroupId { get; set; }

        public ServerInfoDto()
        {
            IsEnabled = true;
            Name = "";
        }
    }
}
