using BlubLib.Serialization;
using Netsphere.Network.Data.Auth;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class LoginEUReqMessage : IAuthMessage
    {
        [BlubMember(0)]
        public string Username { get; set; }

        [BlubMember(1)]
        public string Password { get; set; }

        [BlubMember(2)]
        public string Unk1 { get; set; }

        [BlubMember(3)]
        public string Unk2 { get; set; }

        [BlubMember(4)]
        public int Unk3 { get; set; }

        [BlubMember(5)]
        public int Unk4 { get; set; }

        [BlubMember(6)]
        public int Unk5 { get; set; }

        [BlubMember(7)]
        public string Unk6 { get; set; }

        [BlubMember(8)]
        public int Unk7 { get; set; }

        [BlubMember(9)]
        public string Unk8 { get; set; }

        [BlubMember(10)]
        public string Unk9 { get; set; }

        [BlubMember(11)]
        public AeriaTokenDto Token { get; set; }

        [BlubMember(12)]
        public string Unk10 { get; set; }
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

    [BlubContract]
    public class GameDataXBNReqMessage : IAuthMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }
}
