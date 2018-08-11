using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Chat
{
    [BlubContract]
    public class CLoginReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public string SessionId { get; set; }
    }

    [BlubContract]
    public class CDenyChatReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public DenyAction Action { get; set; }

        [BlubMember(1)]
        public DenyDto Deny { get; set; }
    }

    [BlubContract]
    public class CFriendReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Action { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CCheckCombiNameReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public string Name { get; set; }
    }

    [BlubContract]
    public class CCombiReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class CGetUserDataReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class CSetUserDataReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public UserDataDto UserData { get; set; }
    }

    [BlubContract]
    public class CChatMessageReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ChatType ChatType { get; set; }

        [BlubMember(1)]
        public string Message { get; set; }
    }

    [BlubContract]
    public class CWhisperChatMessageReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public string ToNickname { get; set; }

        [BlubMember(1)]
        public string Message { get; set; }
    }

    [BlubContract]
    public class CInvitationPlayerReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class CNoteListReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public byte Page { get; set; }

        [BlubMember(1)]
        public int MessageType { get; set; }
    }

    [BlubContract]
    public class CSendNoteReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public string Receiver { get; set; }

        [BlubMember(1)]
        public ulong Unk1 { get; set; }

        [BlubMember(2)]
        public string Title { get; set; }

        [BlubMember(3)]
        public string Message { get; set; }

        [BlubMember(4)]
        public int Unk2 { get; set; }

        [BlubMember(5)]
        public NoteGiftDto Gift { get; set; }
    }

    [BlubContract]
    public class CReadNoteReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Id { get; set; }
    }

    [BlubContract]
    public class CDeleteNoteReqMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Notes { get; set; }
    }

    [BlubContract]
    public class CNoteReminderInfoReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }
}
