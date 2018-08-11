using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class UserDataDto
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public ulong AccountId { get; set; }

        [BlubMember(3)]
        public short ServerId { get; set; }

        [BlubMember(4)]
        public short ChannelId { get; set; }

        /*
        0xFFFFFFFE License
        0xFFFFFFFD Tutorial
        0xFFFFFFFF No room
        */

        [BlubMember(5)]
        public uint RoomId { get; set; }

        [BlubMember(6)]
        public byte Unk3 { get; set; } // Gender?

        [BlubMember(7)]
        public uint TotalExp { get; set; }

        [BlubMember(8)]
        public TDUserDataDto TDStats { get; set; }

        [BlubMember(9)]
        public DMUserDataDto DMStats { get; set; }

        [BlubMember(10)]
        public ChaserUserDataDto ChaserStats { get; set; }

        [BlubMember(11)]
        public BRUserDataDto BattleRoyalStats { get; set; }

        [BlubMember(12)]
        public CPTUserDataDto CaptainStats { get; set; }

        [BlubMember(13)]
        public CommunitySetting AllowCombiInvite { get; set; }

        [BlubMember(14)]
        public CommunitySetting AllowFriendRequest { get; set; }

        [BlubMember(15)]
        public CommunitySetting AllowRoomInvite { get; set; }

        [BlubMember(16)]
        public CommunitySetting AllowInfoRequest { get; set; }

        [BlubMember(17)]
        public Team Team { get; set; }

        [BlubMember(18)]
        public int Unk4 { get; set; }

        [BlubMember(19)]
        public byte Unk5 { get; set; }

        [BlubMember(20)]
        public short Unk6 { get; set; }

        [BlubMember(21)]
        [BlubSerializer(typeof(FixedArraySerializer), 9)]
        public byte[] Unk7 { get; set; }

        public UserDataDto()
        {
            Unk1 = 1;
            Unk7 = new byte[9];

            TDStats = new TDUserDataDto();
            DMStats = new DMUserDataDto();
            ChaserStats = new ChaserUserDataDto();
            BattleRoyalStats = new BRUserDataDto();
            CaptainStats = new CPTUserDataDto();
        }
    }

    [BlubContract]
    public class UserDataWithNickDto
    {
        [BlubMember(0)]
        public uint AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public UserDataDto Data { get; set; }

        public UserDataWithNickDto()
        {
            Nickname = "";
            Data = new UserDataDto();
        }
    }

    [BlubContract]
    public class UserDataWithNickLongDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public UserDataDto Data { get; set; }

        public UserDataWithNickLongDto()
        {
            Nickname = "";
            Data = new UserDataDto();
        }
    }
}
