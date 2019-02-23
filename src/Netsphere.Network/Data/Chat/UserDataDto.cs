using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class UserDataDto
    {
        [BlubMember(0)]
        public string Nickname { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public uint TotalExperience { get; set; }

        [BlubMember(3)]
        public int Unk1 { get; set; }

        [BlubMember(4)]
        public string Unk2 { get; set; }

        [BlubMember(5)]
        public string Unk3 { get; set; }

        [BlubMember(6)]
        public uint Level { get; set; }

        [BlubMember(7)]
        [BlubSerializer(typeof(UserDataTimeSpanSerializer))]
        public TimeSpan PlayTime { get; set; }

        [BlubMember(8)]
        public int TotalGames { get; set; }

        [BlubMember(9)]
        public int GamesWon { get; set; }

        [BlubMember(10)]
        public int GamesLost { get; set; }

        [BlubMember(11)]
        public int Unk4 { get; set; }

        [BlubMember(12)]
        public int Unk5 { get; set; }

        [BlubMember(13)]
        public int Unk6 { get; set; }

        [BlubMember(14)]
        public float Unk7 { get; set; }

        [BlubMember(15)]
        public float TDScore { get; set; }

        [BlubMember(16)]
        public float DMScore { get; set; }

        [BlubMember(17)]
        public float ChaserSurvivability { get; set; }

        [BlubMember(18)]
        public float BRScore { get; set; }

        [BlubMember(19)]
        public float CaptainScore { get; set; }

        [BlubMember(20)]
        public float SiegeScore { get; set; }

        [BlubMember(21)]
        public float ArenaScore { get; set; }

        [BlubMember(22)]
        public TDUserDataDto TDStats { get; set; }

        [BlubMember(23)]
        public DMUserDataDto DMStats { get; set; }

        [BlubMember(24)]
        public BRUserDataDto BattleRoyalStats { get; set; }

        [BlubMember(25)]
        public ChaserUserDataDto ChaserStats { get; set; }

        [BlubMember(26)]
        public CPTUserDataDto CaptainStats { get; set; }

        [BlubMember(27)]
        public SiegeUserDataDto SiegeStats { get; set; }

        [BlubMember(28)]
        public ArenaUserDataDto ArenaStats { get; set; }

        [BlubMember(29)]
        public byte Unk8 { get; set; }

        [BlubMember(30)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public UserDataItemDto[] Clothes { get; set; }

        [BlubMember(31)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public UserDataItemDto[] Weapons { get; set; }

        [BlubMember(32)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public UserDataItemDto[] Skills { get; set; }

        public UserDataDto()
        {
            TDStats = new TDUserDataDto();
            DMStats = new DMUserDataDto();
            ChaserStats = new ChaserUserDataDto();
            BattleRoyalStats = new BRUserDataDto();
            CaptainStats = new CPTUserDataDto();
            SiegeStats = new SiegeUserDataDto();
            ArenaStats = new ArenaUserDataDto();
            Clothes = Array.Empty<UserDataItemDto>();
            Weapons = Array.Empty<UserDataItemDto>();
            Skills = Array.Empty<UserDataItemDto>();
        }
    }
}
