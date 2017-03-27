using BlubLib.Serialization;
using System.Net;
using ProudNet.Serialization.Serializers;

namespace Auth.ServiceModel
{
    [BlubContract]
    public class ServerInfoDto
    {
        [BlubMember(0)]
        public byte Id { get; set; }

        [BlubMember(1)]
        public string Name { get; set; }

        [BlubMember(2)]
        public ushort PlayerLimit { get; set; }

        [BlubMember(3)]
        public ushort PlayerOnline { get; set; }

        [BlubMember(4, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(5, typeof(IPEndPointSerializer))]
        public IPEndPoint ChatEndPoint { get; set; }
    }
}
