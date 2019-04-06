using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Club
{
    [BlubContract]
    public class ClubCreateReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public string Unk6 { get; set; }

        [BlubMember(6)]
        public string Unk7 { get; set; }

        [BlubMember(7)]
        public string Unk8 { get; set; }

        [BlubMember(8)]
        public string Unk9 { get; set; }

        [BlubMember(9)]
        public string Unk10 { get; set; }
    }

    [BlubContract]
    public class ClubCloseReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

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
    public class ClubUnjoinReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubNameCheckReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Name { get; set; }
    }

    [BlubContract]
    public class ClubRestoreReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubAdminInviteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinCommandReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminGradeChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public AdminGradeDto[] Grades { get; set; }
    }

    [BlubContract]
    public class ClubAdminNoticeChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Notice { get; set; }
    }

    [BlubContract]
    public class ClubAdminInfoModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubAdminSubMasterCancelReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminMasterChangeReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ClubAdminJoinConditionModifyReqMessage : IClubMessage
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

        [BlubMember(7)]
        public string Unk7 { get; set; }
    }

    [BlubContract]
    public class ClubAdminBoardModifyReqMessage : IClubMessage
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
    public class ClubSearchReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }
    }

    [BlubContract]
    public class ClubInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinWaiterInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubNewJoinMemberInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinConditionInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinerListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinSettingMemberListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubGradeCountReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubStuffListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubNewsInfoReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubBoardWriteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubBoardModifyReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardDeleteAllReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubBoardSearchNickReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadOtherClubReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubBoardReadMineReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubCreateReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }
    }

    [BlubContract]
    public class ClubCloseReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubJoinReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }

        [BlubMember(1)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubUnjoinReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubRestoreReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubClubInfoReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public uint ClubId { get; set; }
    }

    [BlubContract]
    public class ClubSearchReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }
    }

    [BlubContract]
    public class ClubEditURLReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Url { get; set; }
    }

    [BlubContract]
    public class ClubEditIntroduceReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public string Text { get; set; }
    }

    [BlubContract]
    public class ClubStuffListReq2Message : IClubMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubRankListReqMessage : IClubMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
