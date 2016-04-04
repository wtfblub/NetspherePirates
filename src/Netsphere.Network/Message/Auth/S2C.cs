using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Auth
{
    public class SAuthInEuAckMessage : AuthMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public uint SessionId { get; set; }

        [Serialize(2, Compiler = typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [Serialize(3, Compiler = typeof(StringSerializer))]
        public string SessionId2 { get; set; }

        [Serialize(4, Compiler = typeof(EnumSerializer))]
        public AuthLoginResult Result { get; set; }

        [Serialize(5, Compiler = typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [Serialize(6, Compiler = typeof(StringSerializer))]
        public string BannedUntil { get; set; }

        public SAuthInEuAckMessage()
        {
            Unk1 = "";
            SessionId2 = "";
            Unk2 = "";
            BannedUntil = "";
        }

        public SAuthInEuAckMessage(DateTimeOffset bannedUntil)
            : this()
        {
            Result = AuthLoginResult.Banned;
            BannedUntil = bannedUntil.ToString("yyyyMMddHHmmss");
        }

        public SAuthInEuAckMessage(AuthLoginResult result)
            : this()
        {
            Result = result;
        }

        public SAuthInEuAckMessage(AuthLoginResult result, ulong accountId, uint sessionId)
            : this()
        {
            Result = result;
            AccountId = accountId;
            SessionId = (uint) accountId;
            SessionId2 = sessionId.ToString();
        }
    }

    public class SServerListAckMessage : AuthMessage
    {
        [Serialize(0, Compiler = typeof(ArrayWithIntPrefixSerializer))]
        public ServerInfoDto[] ServerList { get; set; }

        public SServerListAckMessage()
            : this(Array.Empty<ServerInfoDto>())
        { }

        public SServerListAckMessage(ServerInfoDto[] serverList)
        {
            ServerList = serverList;
        }
    }
}
