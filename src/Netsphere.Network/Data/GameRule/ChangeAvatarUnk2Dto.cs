using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    public class ChangeAvatarUnk2Dto
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public float Unk2 { get; set; }

        [Serialize(2)]
        public float Unk3 { get; set; }
    }
}
