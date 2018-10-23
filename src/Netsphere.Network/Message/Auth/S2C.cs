using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Auth
{
    [BlubContract]
    public class SAuthInEuAckMessage : IAuthMessage
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

        public SAuthInEuAckMessage()
        {
            Unk2 = "";
            SessionId = "";
            Unk3 = "";
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

        public SAuthInEuAckMessage(AuthLoginResult result, ulong accountId, string sessionId)
            : this()
        {
            Result = result;
            AccountId = accountId;
            SessionId = sessionId;
        }
    }

    [BlubContract]
    public class SServerListAckMessage : IAuthMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ServerInfoDto[] ServerList { get; set; }

        public SServerListAckMessage()
            : this(Array.Empty<ServerInfoDto>())
        {
        }

        public SServerListAckMessage(ServerInfoDto[] serverList)
        {
            ServerList = serverList;
        }
    }
}
