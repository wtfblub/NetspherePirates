using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class LoginEUAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public uint Unk1 { get; set; }

        [BlubMember(2)]
        public string Unk2 { get; set; }

        [BlubMember(3)]
        public string SessionId { get; set; }

        [BlubMember(4)]
        public AuthLoginResult Result { get; set; }

        [BlubMember(5)]
        public string Unk3 { get; set; }

        [BlubMember(6)]
        public string BannedUntil { get; set; }

        public LoginEUAckMessage()
        {
            Unk2 = "";
            SessionId = "";
            Unk3 = "";
            BannedUntil = "";
        }

        public LoginEUAckMessage(DateTimeOffset bannedUntil)
            : this()
        {
            Result = AuthLoginResult.Banned;
            BannedUntil = bannedUntil.ToString("yyyyMMddHHmmss");
        }

        public LoginEUAckMessage(AuthLoginResult result)
            : this()
        {
            Result = result;
        }

        public LoginEUAckMessage(AuthLoginResult result, ulong accountId, string sessionId)
            : this()
        {
            Result = result;
            AccountId = accountId;
            SessionId = sessionId;
        }
    }

    [BlubContract]
    public class ServerListAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ServerInfoDto[] ServerList { get; set; }

        public ServerListAckMessage()
            : this(Array.Empty<ServerInfoDto>())
        { }

        public ServerListAckMessage(ServerInfoDto[] serverList)
        {
            ServerList = serverList;
        }
    }

    [BlubContract]
    public class OptionVersionCheckAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        public byte[] Data { get; set; }

        public OptionVersionCheckAckMessage()
        {
            Data = Array.Empty<byte>();
        }

        public OptionVersionCheckAckMessage(byte[] data)
        {
            Data = data;
        }
    }

    [BlubContract]
    public class GameDataXBNAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        public XBNType XBNType { get; set; }

        [BlubMember(1)]
        public byte[] Data { get; set; }

        [BlubMember(2)]
        public int TotalLength { get; set; }
    }
}
