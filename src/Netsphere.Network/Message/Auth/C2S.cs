using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class LoginEUReqMessage : IAuthMessage
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

        [BlubMember(8)]
        public int Unk7 { get; set; }

        [BlubMember(9, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        [BlubMember(10, typeof(StringSerializer))]
        public string Unk9 { get; set; }
    }

    [BlubContract]
    public class ServerListReqMessage : IAuthMessage
    { }

    [BlubContract]
    public class OptionVersionCheckReqMessage : IAuthMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public uint Checksum { get; set; }
    }
}