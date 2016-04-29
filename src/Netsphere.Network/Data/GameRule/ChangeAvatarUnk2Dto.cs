using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ChangeAvatarUnk2Dto
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public float Unk2 { get; set; }

        [BlubMember(2)]
        public float Unk3 { get; set; }
    }
}
