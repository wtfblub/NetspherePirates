using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Club
{
    [BlubContract]
    public class ClubCreateAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubCloseAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubJoinAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubNameCheckAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubRestoreAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminInviteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinCommandAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminGradeChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminNoticeChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminInfoModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterCancelAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminMasterChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinConditionModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubAdminBoardModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubSearchAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ClubSearchInfoDto[] Clubs { get; set; }
    }

    [BlubContract]
    public class ClubInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public ClubSearchInfoDto Info { get; set; }
    }

    [BlubContract]
    public class ClubJoinWaiterInfoAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public JoinWaiterInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubNewJoinMemberInfoAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ClubMemberInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubJoinConditionInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Unk6 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk7 { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinerListAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public UnjoinerDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinSettingMemberListAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public UnjoinSettingMemberDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubGradeCountAckMessage : IClubMessage
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
    public class ClubStuffListAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ClubMemberDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubNewsInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }
    }

    [BlubContract]
    public class ClubMyInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public MyInfoDto Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardWriteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadAckMessage : IClubMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public BoardMessageDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardModifyAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAllAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadFailedAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }
}
