using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class UserDataDto
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2)]
        public ulong AccountId { get; set; }

        [Serialize(3)]
        public short ServerId { get; set; }

        [Serialize(4)]
        public short ChannelId { get; set; }

        /*
        0xFFFFFFFE License
        0xFFFFFFFD Tutorial
        0xFFFFFFFF No room
        */
        [Serialize(5)]
        public uint RoomId { get; set; }

        [Serialize(6)]
        public byte Unk3 { get; set; } // Gender?

        [Serialize(7)]
        public uint TotalExp { get; set; }

        [Serialize(8)]
        public TDUserDataDto TDStats { get; set; }

        [Serialize(9)]
        public DMUserDataDto DMStats { get; set; }

        [Serialize(10)]
        public ChaserUserDataDto ChaserStats { get; set; }

        [Serialize(11)]
        public BRUserDataDto BattleRoyalStats { get; set; }

        [Serialize(12)]
        public CPTUserDataDto CaptainStats { get; set; }

        [Serialize(13, typeof(EnumSerializer))]
        public CommunitySetting AllowCombiInvite { get; set; }

        [Serialize(14, typeof(EnumSerializer))]
        public CommunitySetting AllowFriendRequest { get; set; }

        [Serialize(15, typeof(EnumSerializer))]
        public CommunitySetting AllowRoomInvite { get; set; }

        [Serialize(16, typeof(EnumSerializer))]
        public CommunitySetting AllowInfoRequest { get; set; }

        [Serialize(17, typeof(EnumSerializer))]
        public Team Team { get; set; }

        [Serialize(18)]
        public int Unk4 { get; set; }

        [Serialize(19)]
        public byte Unk5 { get; set; }

        [Serialize(20)]
        public short Unk6 { get; set; }

        [Serialize(21, typeof(FixedArraySerializer), 9)]
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

    public class UserDataWithNickDto
    {
        [Serialize(0)]
        public uint AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2)]
        public UserDataDto Data { get; set; }

        public UserDataWithNickDto()
        {
            Nickname = "";
            Data = new UserDataDto();
        }
    }

    public class UserDataWithNickLongDto
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2)]
        public UserDataDto Data { get; set; }

        public UserDataWithNickLongDto()
        {
            Nickname = "";
            Data = new UserDataDto();
        }
    }
}
