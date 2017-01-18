using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class SAuthInEuAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public uint SessionId { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string SessionId2 { get; set; }

        [BlubMember(4)]
        public AuthLoginResult Result { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
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

    [BlubContract]
    public class SServerListAckMessage : IAuthMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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
