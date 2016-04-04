using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Auth
{
    public class CAuthInEUReqMessage : AuthMessage
    {
        [Serialize(0, Compiler = typeof(StringSerializer))]
        public string Username { get; set; }

        [Serialize(1, Compiler = typeof(StringSerializer))]
        public string Password { get; set; }

        [Serialize(2, Compiler = typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [Serialize(3, Compiler = typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [Serialize(4)]
        public int Unk3 { get; set; }

        [Serialize(5)]
        public int Unk4 { get; set; }

        [Serialize(6)]
        public int Unk5 { get; set; }

        [Serialize(7, Compiler = typeof(StringSerializer))]
        public string Unk6 { get; set; }
    }

    public class CServerListReqMessage : AuthMessage
    { }
}