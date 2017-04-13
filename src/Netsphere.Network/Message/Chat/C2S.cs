using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Chat
{
    [BlubContract]
    public class LoginReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string SessionId { get; set; }
    }

    [BlubContract]
    public class DenyActionReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public DenyAction Action { get; set; }

        [BlubMember(1)]
        public DenyDto Deny { get; set; }
    }

    [BlubContract]
    public class FriendActionReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Action { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CombiCheckNameReqMessage : IChatMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Name { get; set; }
    }

    [BlubContract]
    public class CombiActionReqMessage : IChatMessage
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
    public class UserDataOneReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class UserDataThreeAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        [BlubMember(2)]
        public UserDataDto UserData { get; set; }
    }

    [BlubContract]
    public class MessageChatReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ChatType ChatType { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    [BlubContract]
    public class MessageWhisperChatReqMessage : IChatMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string ToNickname { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    [BlubContract]
    public class RoomInvitationPlayerReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class NoteListReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public byte Page { get; set; }

        [BlubMember(1)]
        public int MessageType { get; set; }
    }

    [BlubContract]
    public class NoteSendReqMessage : IChatMessage
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
    public class NoteReadReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Id { get; set; }
    }

    [BlubContract]
    public class NoteDeleteReqMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Notes { get; set; }
    }

    [BlubContract]
    public class NoteCountReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class OptionSaveCommunityReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }
    }

    [BlubContract]
    public class OptionSaveBinaryReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Unk2 { get; set; }
    }

    [BlubContract]
    public class NoteRejectImportuneReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubNoteSendReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ClubNoteDto Note { get; set; }
    }

    [BlubContract]
    public class ClubMemberListReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }
}
