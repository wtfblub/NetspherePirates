using BlubLib.Serialization;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Serializers;

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

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminGradeChangeAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
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

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfoDto[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public string Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        [BlubMember(9)]
        public int Unk10 { get; set; }

        [BlubMember(10)]
        public int Unk11 { get; set; }

        [BlubMember(11)]
        public int Unk12 { get; set; }

        [BlubMember(12)]
        public string Unk13 { get; set; }

        [BlubMember(13)]
        public string Unk14 { get; set; }

        [BlubMember(14)]
        public int Unk15 { get; set; }

        [BlubMember(15)]
        public int Unk16 { get; set; }

        [BlubMember(16)]
        public int Unk17 { get; set; }

        [BlubMember(17)]
        public int Unk18 { get; set; }
    }

    [BlubContract]
    public class ClubJoinWaiterInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public JoinWaiterInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubNewJoinMemberInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NewMemberInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubJoinConditionInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public string Unk6 { get; set; }

        [BlubMember(6)]
        public string Unk7 { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinerListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public MemberLeftDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinSettingMemberListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
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
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public Netsphere.Network.Data.Chat.ClubMemberDto[] Members { get; set; }
    }

    [BlubContract]
    public class ClubNewsInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NewsInfoDto[] News { get; set; }
    }

    [BlubContract]
    public class ClubMyInfoAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        [BlubMember(9)]
        public int Unk10 { get; set; }

        [BlubMember(10)]
        public int Unk11 { get; set; }

        [BlubMember(11)]
        public int Unk12 { get; set; }

        [BlubMember(12)]
        public byte Unk13 { get; set; }
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
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public BoardInfoDto[] Unk2 { get; set; }
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

    [BlubContract]
    public class ClubCreateAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubCloseAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubJoinAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubRestoreAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubClubInfoAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfo2Dto[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubSearchAck2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfo2Dto[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubEditURLAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubEditIntroduceAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubStuffListAck2Message : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public Netsphere.Network.Data.Chat.ClubMember2Dto[] Members { get; set; }
    }

    [BlubContract]
    public class ClubRankListAckMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ClubInfo2Dto[] Clubs { get; set; }
    }

    [BlubContract]
    public class ClubMatchNoticeMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
