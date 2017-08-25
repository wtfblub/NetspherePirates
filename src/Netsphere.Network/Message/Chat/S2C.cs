using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlubLib;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Serializers;
using ProudNet;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Chat
{
    [BlubContract]
    public class SLoginAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }

        public SLoginAckMessage()
        { }

        public SLoginAckMessage(uint result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class SFriendAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    public class SFriendListAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SCombiAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    public class SCombiListAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SCheckCombiNameAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
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

    [BlubContract]
    public class SDenyChatAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        [BlubMember(1)]
        public DenyAction Action { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    public class SDenyChatListAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SUserDataAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1)]
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

    [BlubContract]
    public class SUserDataListAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract(typeof(Serializer))]
    public class SChannelPlayerListAckMessage : IChatMessage
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

        internal class Serializer : ISerializer<SChannelPlayerListAckMessage>
        {
            public bool CanHandle(Type type) => type == typeof(SChannelPlayerListAckMessage);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BinaryWriter writer, SChannelPlayerListAckMessage value)
            {
                using (var w2 = new BinaryWriter(new MemoryStream()))
                {
                    w2.Write((ushort)value.UserData.Length);

                    var serializer = BlubLib.Serialization.Serializer.GetSerializer<UserDataWithNickDto>();
                    foreach (var userData in value.UserData)
                        serializer.Serialize(w2, userData);

                    var data = w2.ToArray();
                    var compressed = data.CompressLZO();
                    writer.WriteStruct(compressed);
                    writer.Write(compressed.Length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SChannelPlayerListAckMessage Deserialize(BinaryReader reader)
            {
                var compressed = reader.ReadStruct();
                reader.ReadInt32(); // length

                var decompressed = compressed.DecompressLZO(compressed.Length * 10);

                using (var r2 = decompressed.ToBinaryReader())
                {
                    var serializer = BlubLib.Serialization.Serializer.GetSerializer<UserDataWithNickDto>();
                    var userData = new UserDataWithNickDto[r2.ReadInt16()];
                    for (var i = 0; i < userData.Length; i++)
                        userData[i] = serializer.Deserialize(r2);
                    return new SChannelPlayerListAckMessage(userData);
                }
            }
        }
    }

    [BlubContract]
    public class SChannelEnterPlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
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

    [BlubContract]
    public class SChannelLeavePlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChannelLeavePlayerAckMessage()
        { }

        public SChannelLeavePlayerAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChatMessageAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ChatType ChatType { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
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

    [BlubContract]
    public class SWhisperChatMessageAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string ToNickname { get; set; }

        [BlubMember(2)]
        public ulong AccountId { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
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

    [BlubContract]
    public class SInvitationPlayerAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    public class SClanMemberListAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SNoteListAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int PageCount { get; set; }

        [BlubMember(1)]
        public byte CurrentPage { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; } // MessageType? - MessageType UI does not exist in this version

        [BlubMember(3, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SSendNoteAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        public SSendNoteAckMessage()
        { }

        public SSendNoteAckMessage(int result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class SReadNoteAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public ulong Id { get; set; }

        [BlubMember(1)]
        public NoteContentDto Note { get; set; }

        [BlubMember(2)]
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

    [BlubContract]
    public class SDeleteNoteAckMessage : IChatMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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

    [BlubContract]
    public class SNoteErrorAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        public SNoteErrorAckMessage()
        { }

        public SNoteErrorAckMessage(int unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class SNoteReminderInfoAckMessage : IChatMessage
    {
        [BlubMember(0)]
        public byte NoteCount { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
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
