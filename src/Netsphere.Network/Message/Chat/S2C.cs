using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Chat
{
    public class SLoginAckMessage : ChatMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }

        public SLoginAckMessage()
        { }

        public SLoginAckMessage(uint result)
        {
            Result = result;
        }
    }

    public class SFriendAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int Result { get; set; }

        [Serialize(1)]
        public int Unk { get; set; }

        [Serialize(2)]
        public FriendDto Friend { get; set; }

        public SFriendAckMessage()
        {
            Friend = new FriendDto();
        }

        public SFriendAckMessage(int result)
            : this()
        {
            Result = result;
        }

        public SFriendAckMessage(int result, int unk, FriendDto friend)
        {
            Result = result;
            Unk = unk;
            Friend = friend;
        }
    }

    public class SFriendListAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public FriendDto[] Friends { get; set; }

        public SFriendListAckMessage()
        {
            Friends = Array.Empty<FriendDto>();
        }

        public SFriendListAckMessage(FriendDto[] friends)
        {
            Friends = friends;
        }
    }

    public class SCombiAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int Result { get; set; }

        [Serialize(1)]
        public int Unk { get; set; }

        [Serialize(2)]
        public CombiDto Combi { get; set; }

        public SCombiAckMessage()
        {
            Combi = new CombiDto();
        }

        public SCombiAckMessage(int result, int unk, CombiDto combi)
        {
            Result = result;
            Unk = unk;
            Combi = combi;
        }
    }

    public class SCombiListAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public CombiDto[] Combies { get; set; }

        public SCombiListAckMessage()
        {
            Combies = Array.Empty<CombiDto>();
        }

        public SCombiListAckMessage(CombiDto[] combies)
        {
            Combies = combies;
        }
    }

    public class SCheckCombiNameAckMessage : ChatMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        public SCheckCombiNameAckMessage()
        {
            Unk2 = "";
        }

        public SCheckCombiNameAckMessage(uint unk1, string unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    public class SDenyChatAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int Result { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public DenyAction Action { get; set; }

        [Serialize(2)]
        public DenyDto Deny { get; set; }

        public SDenyChatAckMessage()
        {
            Deny = new DenyDto();
        }

        public SDenyChatAckMessage(int result, DenyAction action, DenyDto deny)
        {
            Result = result;
            Action = action;
            Deny = deny;
        }
    }

    public class SDenyChatListAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public DenyDto[] Denies { get; set; }

        public SDenyChatListAckMessage()
        {
            Denies = Array.Empty<DenyDto>();
        }

        public SDenyChatListAckMessage(DenyDto[] denies)
        {
            Denies = denies;
        }
    }

    public class SUserDataAckMessage : ChatMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }

        [Serialize(1)]
        public UserDataDto UserData { get; set; }

        public SUserDataAckMessage()
        {
            UserData = new UserDataDto();
        }

        public SUserDataAckMessage(UserDataDto userData)
        {
            UserData = userData;
        }
    }

    public class SUserDataListAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public UserDataDto[] UserData { get; set; }

        public SUserDataListAckMessage()
        {
            UserData = Array.Empty<UserDataDto>();
        }

        public SUserDataListAckMessage(UserDataDto[] userData)
        {
            UserData = userData;
        }
    }

    public class SChannelPlayerListAckMessage : ChatMessage
    {
        public UserDataWithNickDto[] UserData { get; set; }

        public SChannelPlayerListAckMessage()
        {
            UserData = Array.Empty<UserDataWithNickDto>();
        }

        public SChannelPlayerListAckMessage(UserDataWithNickDto[] userData)
        {
            UserData = userData;
        }
    }

    public class SChannelEnterPlayerAckMessage : ChatMessage
    {
        [Serialize(0)]
        public UserDataWithNickDto UserData { get; set; }

        public SChannelEnterPlayerAckMessage()
        {
            UserData = new UserDataWithNickDto();
        }

        public SChannelEnterPlayerAckMessage(UserDataWithNickDto userData)
        {
            UserData = userData;
        }
    }

    public class SChannelLeavePlayerAckMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SChannelLeavePlayerAckMessage()
        { }

        public SChannelLeavePlayerAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SChatMessageAckMessage : ChatMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ChatType ChatType { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Message { get; set; }

        public SChatMessageAckMessage()
        {
            Nickname = "";
            Message = "";
        }

        public SChatMessageAckMessage(ChatType chatType, ulong accountId, string nick, string message)
        {
            ChatType = chatType;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
    }

    public class SWhisperChatMessageAckMessage : ChatMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string ToNickname { get; set; }

        [Serialize(2)]
        public ulong AccountId { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(4, typeof(StringSerializer))]
        public string Message { get; set; }

        public SWhisperChatMessageAckMessage()
        {
            ToNickname = "";
            Nickname = "";
            Message = "";
        }

        public SWhisperChatMessageAckMessage(uint unk, string toNickname, ulong accountId, string nick, string message)
        {
            Unk = unk;
            ToNickname = toNickname;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
    }

    public class SInvitationPlayerAckMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [Serialize(2)]
        public UserDataDto UserData { get; set; }

        public SInvitationPlayerAckMessage()
        {
            Unk2 = "";
            UserData = new UserDataDto();
        }

        public SInvitationPlayerAckMessage(ulong unk1, string unk2, UserDataDto userData)
        {
            Unk1 = unk1;
            Unk2 = unk2;
            UserData = userData;
        }
    }

    public class SClanMemberListAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public UserDataWithNickLongDto[] Players { get; set; }

        public SClanMemberListAckMessage()
        {
            Players = Array.Empty<UserDataWithNickLongDto>();
        }

        public SClanMemberListAckMessage(UserDataWithNickLongDto[] players)
        {
            Players = players;
        }
    }

    public class SNoteListAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int PageCount { get; set; }

        [Serialize(1)]
        public byte CurrentPage { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; } // MessageType? - MessageType UI does not exist in this version

        [Serialize(3, typeof(ArrayWithIntPrefixSerializer))]
        public NoteDto[] Notes { get; set; }

        public SNoteListAckMessage()
        {
            Notes = Array.Empty<NoteDto>();
        }

        public SNoteListAckMessage(int pageCount, byte currentPage, NoteDto[] notes)
        {
            PageCount = pageCount;
            CurrentPage = currentPage;
            Unk3 = 7;
            Notes = notes;
        }
    }

    public class SSendNoteAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int Result { get; set; }

        public SSendNoteAckMessage()
        { }

        public SSendNoteAckMessage(int result)
        {
            Result = result;
        }
    }

    public class SReadNoteAckMessage : ChatMessage
    {
        [Serialize(0)]
        public ulong Id { get; set; }

        [Serialize(1)]
        public NoteContentDto Note { get; set; }

        [Serialize(2)]
        public int Unk { get; set; }

        public SReadNoteAckMessage()
        {
            Note = new NoteContentDto();
        }

        public SReadNoteAckMessage(ulong id, NoteContentDto note, int unk)
        {
            Id = id;
            Note = note;
            Unk = unk;
        }
    }

    public class SDeleteNoteAckMessage : ChatMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public DeleteNoteDto[] Notes { get; set; }

        public SDeleteNoteAckMessage()
        {
            Notes = Array.Empty<DeleteNoteDto>();
        }

        public SDeleteNoteAckMessage(DeleteNoteDto[] notes)
        {
            Notes = notes;
        }
    }

    public class SNoteErrorAckMessage : ChatMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }

        public SNoteErrorAckMessage()
        { }

        public SNoteErrorAckMessage(int unk)
        {
            Unk = unk;
        }
    }

    public class SNoteReminderInfoAckMessage : ChatMessage
    {
        [Serialize(0)]
        public byte NoteCount { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2)]
        public byte Unk3 { get; set; }

        public SNoteReminderInfoAckMessage()
        { }

        public SNoteReminderInfoAckMessage(byte noteCount, byte unk2, byte unk3)
        {
            NoteCount = noteCount;
            Unk2 = unk2;
            Unk3 = unk3;
        }
    }
}
