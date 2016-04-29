using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Chat
{
    [BlubContract]
    public class CLoginReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string SessionId { get; set; }
    }

    [BlubContract]
    public class CDenyChatReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public DenyAction Action { get; set; }

        [BlubMember(1)]
        public DenyDto Deny { get; set; }
    }

    [BlubContract]
    public class CFriendReqMessage : ChatMessage
    {
        [BlubMember(1)]
        public uint Action { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CCheckCombiNameReqMessage : ChatMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Name { get; set; }
    }

    [BlubContract]
    public class CCombiReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class CGetUserDataReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class CSetUserDataReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public UserDataDto UserData { get; set; }
    }

    [BlubContract]
    public class CChatMessageReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ChatType ChatType { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    [BlubContract]
    public class CWhisperChatMessageReqMessage : ChatMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string ToNickname { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    [BlubContract]
    public class CInvitationPlayerReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class CNoteListReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public byte Page { get; set; }

        [BlubMember(1)]
        public int MessageType { get; set; }
    }

    [BlubContract]
    public class CSendNoteReqMessage : ChatMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Receiver { get; set; }

        [BlubMember(1)]
        public ulong Unk1 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Title { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Message { get; set; }

        [BlubMember(4)]
        public int Unk2 { get; set; }

        [BlubMember(5)]
        public NoteGiftDto Gift { get; set; }
    }

    [BlubContract]
    public class CReadNoteReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong Id { get; set; }
    }

    [BlubContract]
    public class CDeleteNoteReqMessage : ChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Notes { get; set; }
    }

    [BlubContract]
    public class CNoteReminderInfoReqMessage : ChatMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }
}
