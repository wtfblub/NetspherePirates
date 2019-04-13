using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Chat
{
    [BlubContract]
    public class LoginAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }

        public LoginAckMessage()
        {
        }

        public LoginAckMessage(uint result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class FriendActionAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        [BlubMember(2)]
        public FriendDto Friend { get; set; }

        public FriendActionAckMessage()
        {
            Friend = new FriendDto();
        }

        public FriendActionAckMessage(int result)
            : this()
        {
            Result = result;
        }

        public FriendActionAckMessage(int result, int unk, FriendDto friend)
        {
            Result = result;
            Unk = unk;
            Friend = friend;
        }
    }

    [BlubContract]
    public class FriendListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public FriendDto[] Friends { get; set; }

        public FriendListAckMessage()
        {
            Friends = Array.Empty<FriendDto>();
        }

        public FriendListAckMessage(FriendDto[] friends)
        {
            Friends = friends;
        }
    }

    [BlubContract]
    public class CombiActionAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        [BlubMember(2)]
        public CombiDto Combi { get; set; }

        public CombiActionAckMessage()
        {
            Combi = new CombiDto();
        }

        public CombiActionAckMessage(int result, int unk, CombiDto combi)
        {
            Result = result;
            Unk = unk;
            Combi = combi;
        }
    }

    [BlubContract]
    public class CombiListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public CombiDto[] Combies { get; set; }

        public CombiListAckMessage()
        {
            Combies = Array.Empty<CombiDto>();
        }

        public CombiListAckMessage(CombiDto[] combies)
        {
            Combies = combies;
        }
    }

    [BlubContract]
    public class CombiCheckNameAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        public CombiCheckNameAckMessage()
        {
            Unk2 = "";
        }

        public CombiCheckNameAckMessage(uint unk1, string unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class DenyActionAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public DenyAction Action { get; set; }

        [BlubMember(2)]
        public DenyDto Deny { get; set; }

        public DenyActionAckMessage()
        {
            Deny = new DenyDto();
        }

        public DenyActionAckMessage(int result, DenyAction action, DenyDto deny)
        {
            Result = result;
            Action = action;
            Deny = deny;
        }
    }

    [BlubContract]
    public class DenyListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public DenyDto[] Denies { get; set; }

        public DenyListAckMessage()
        {
            Denies = Array.Empty<DenyDto>();
        }

        public DenyListAckMessage(DenyDto[] denies)
        {
            Denies = denies;
        }
    }

    [BlubContract]
    public class ChannelPlayerListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public PlayerInfoShortDto[] UserData { get; set; }

        public ChannelPlayerListAckMessage()
        {
            UserData = Array.Empty<PlayerInfoShortDto>();
        }

        public ChannelPlayerListAckMessage(PlayerInfoShortDto[] userData)
        {
            UserData = userData;
        }
    }

    [BlubContract]
    public class ChannelEnterPlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public PlayerInfoShortDto UserData { get; set; }

        public ChannelEnterPlayerAckMessage()
        {
            UserData = new PlayerInfoShortDto();
        }

        public ChannelEnterPlayerAckMessage(PlayerInfoShortDto userData)
        {
            UserData = userData;
        }
    }

    [BlubContract]
    public class ChannelLeavePlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public ChannelLeavePlayerAckMessage()
        {
        }

        public ChannelLeavePlayerAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class MessageChatAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ChatType ChatType { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public string Nickname { get; set; }

        [BlubMember(3)]
        public string Message { get; set; }

        public MessageChatAckMessage()
        {
            Nickname = "";
            Message = "";
        }

        public MessageChatAckMessage(ChatType chatType, ulong accountId, string nick, string message)
        {
            ChatType = chatType;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
    }

    [BlubContract]
    public class MessageWhisperChatAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1)]
        public string ToNickname { get; set; }

        [BlubMember(2)]
        public ulong AccountId { get; set; }

        [BlubMember(3)]
        public string Nickname { get; set; }

        [BlubMember(4)]
        public string Message { get; set; }

        public MessageWhisperChatAckMessage()
        {
            ToNickname = "";
            Nickname = "";
            Message = "";
        }

        public MessageWhisperChatAckMessage(uint unk, string toNickname, ulong accountId, string nick, string message)
        {
            Unk = unk;
            ToNickname = toNickname;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
    }

    [BlubContract]
    public class RoomInvitationPlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public PlayerLocationDto Location { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        public RoomInvitationPlayerAckMessage()
        {
            Unk2 = "";
            Location = new PlayerLocationDto();
        }

        public RoomInvitationPlayerAckMessage(ulong unk1, string unk2, PlayerLocationDto location)
        {
            Unk1 = unk1;
            Unk2 = unk2;
            Location = location;
        }
    }

    [BlubContract]
    public class ClanMemberListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public PlayerInfoDto[] Players { get; set; }

        public ClanMemberListAckMessage()
        {
            Players = Array.Empty<PlayerInfoDto>();
        }

        public ClanMemberListAckMessage(PlayerInfoDto[] players)
        {
            Players = players;
        }
    }

    [BlubContract]
    public class NoteListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int PageCount { get; set; }

        [BlubMember(1)]
        public byte CurrentPage { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; } // MessageType? - MessageType UI does not exist in this version

        [BlubMember(3)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NoteDto[] Notes { get; set; }

        public NoteListAckMessage()
        {
            Notes = Array.Empty<NoteDto>();
        }

        public NoteListAckMessage(int pageCount, byte currentPage, NoteDto[] notes)
        {
            PageCount = pageCount;
            CurrentPage = currentPage;
            Unk3 = 7;
            Notes = notes;
        }
    }

    [BlubContract]
    public class NoteSendAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        public NoteSendAckMessage()
        {
        }

        public NoteSendAckMessage(int result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class NoteReadAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Id { get; set; }

        [BlubMember(1)]
        public NoteContentDto Note { get; set; }

        [BlubMember(2)]
        public int Unk { get; set; }

        public NoteReadAckMessage()
        {
            Note = new NoteContentDto();
        }

        public NoteReadAckMessage(ulong id, NoteContentDto note, int unk)
        {
            Id = id;
            Note = note;
            Unk = unk;
        }
    }

    [BlubContract]
    public class NoteDeleteAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public DeleteNoteDto[] Notes { get; set; }

        public NoteDeleteAckMessage()
        {
            Notes = Array.Empty<DeleteNoteDto>();
        }

        public NoteDeleteAckMessage(DeleteNoteDto[] notes)
        {
            Notes = notes;
        }
    }

    [BlubContract]
    public class NoteErrorAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        public NoteErrorAckMessage()
        {
        }

        public NoteErrorAckMessage(int unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class NoteCountAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public byte NoteCount { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        public NoteCountAckMessage()
        {
        }

        public NoteCountAckMessage(byte noteCount, byte unk2, byte unk3)
        {
            NoteCount = noteCount;
            Unk2 = unk2;
            Unk3 = unk3;
        }
    }

    [BlubContract]
    public class PlayerInfoAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public PlayerInfoDto Player { get; set; }

        public PlayerInfoAckMessage()
        {
        }

        public PlayerInfoAckMessage(PlayerInfoDto player)
        {
            Player = player;
        }
    }

    [BlubContract]
    public class PlayerPositionAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public PlayerLocationDto Player { get; set; }

        public PlayerPositionAckMessage()
        {
            Player = new PlayerLocationDto();
        }

        public PlayerPositionAckMessage(ulong accountId, PlayerLocationDto player)
        {
            AccountId = accountId;
            Player = player;
        }
    }

    [BlubContract]
    public class PlayerPlayerInfoListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public PlayerInfoDto[] Players { get; set; }

        public PlayerPlayerInfoListAckMessage()
        {
            Players = Array.Empty<PlayerInfoDto>();
        }

        public PlayerPlayerInfoListAckMessage(PlayerInfoDto[] players)
        {
            Players = players;
        }
    }

    [BlubContract]
    public class UserDataTwoReqMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class UserDataFourAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        public UserDataDto UserData { get; set; }

        public UserDataFourAckMessage()
        {
            UserData = new UserDataDto();
        }

        public UserDataFourAckMessage(int unk, UserDataDto userData)
        {
            Unk = unk;
            UserData = userData;
        }
    }

    [BlubContract]
    public class ClanChangeNoticeAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }

        public ClanChangeNoticeAckMessage()
        {
            Unk = "";
        }
    }

    [BlubContract]
    public class NoteRejectImportuneAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubSystemMessageMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubNewsRemindMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class ClubNoteSendAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubMemberListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubMemberDto[] Members { get; set; }

        public ClubMemberListAckMessage()
        {
            Members = Array.Empty<ClubMemberDto>();
        }
    }

    [BlubContract]
    public class ClubMemberLoginStateAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class ChannelPlayerNameTagListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NameTagDto[] NameTags { get; set; }

        public ChannelPlayerNameTagListAckMessage()
        {
            NameTags = Array.Empty<NameTagDto>();
        }
    }

    [BlubContract]
    public class ClubClubMemberInfoAck2Message : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        public ClubMember2Dto ClubMember { get; set; }

        public ClubClubMemberInfoAck2Message()
        {
            ClubMember = new ClubMember2Dto();
        }
    }

    [BlubContract]
    public class ClubMemberListAck2Message : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubMember2Dto[] Members { get; set; }

        public ClubMemberListAck2Message()
        {
            Members = Array.Empty<ClubMember2Dto>();
        }
    }

    [BlubContract]
    public class BroadcastClubCloseMessage : IChatMessage
    {
    }
}
