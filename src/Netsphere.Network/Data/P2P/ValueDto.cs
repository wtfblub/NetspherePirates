using BlubLib.Serialization;

namespace Netsphere.Network.Data.P2P
{
    public class ValueDto
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1)]
        public float Value1 { get; set; }

        [Serialize(2)]
        public float Value2 { get; set; }
    }
}
