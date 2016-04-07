using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Chat
{
    public class CLoginReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string SessionId { get; set; }
    }

    public class CDenyChatReqMessage : ChatMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public DenyAction Action { get; set; }

        [Serialize(1)]
        public DenyDto Deny { get; set; }
    }

    public class CFriendReqMessage : ChatMessage
    {
        [Serialize(1)]
        public uint Action { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class CCheckCombiNameReqMessage : ChatMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Name { get; set; }
    }

    public class CCombiReqMessage : ChatMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }
    }

    public class CGetUserDataReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class CSetUserDataReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public UserDataDto UserData { get; set; }
    }

    public class CChatMessageReqMessage : ChatMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ChatType ChatType { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    public class CWhisperChatMessageReqMessage : ChatMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string ToNickname { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    public class CInvitationPlayerReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class CNoteListReqMessage : ChatMessage
    {
        [Serialize(0)]
        public byte Page { get; set; }

        [Serialize(1)]
        public int MessageType { get; set; }
    }

    public class CSendNoteReqMessage : ChatMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Receiver { get; set; }

        [Serialize(1)]
        public ulong Unk1 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Title { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Message { get; set; }

        [Serialize(4)]
        public int Unk2 { get; set; }

        [Serialize(5)]
        public NoteGiftDto Gift { get; set; }
    }

    public class CReadNoteReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong Id { get; set; }
    }

    public class CDeleteNoteReqMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Notes { get; set; }
    }

    public class CNoteReminderInfoReqMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong Unk { get; set; }
    }
}
