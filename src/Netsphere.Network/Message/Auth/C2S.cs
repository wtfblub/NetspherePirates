using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class CAuthInEUReqMessage : IAuthMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Username { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Password { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(4)]
        public int Unk3 { get; set; }

        [BlubMember(5)]
        public int Unk4 { get; set; }

        [BlubMember(6)]
        public int Unk5 { get; set; }

        [BlubMember(7, typeof(StringSerializer))]
        public string Unk6 { get; set; }
    }

    [BlubContract]
    public class CServerListReqMessage : IAuthMessage
    { }
}