using System.Drawing;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ChannelInfoDto
    {
        [BlubMember(0)]
        public ushort Id { get; set; }

        [BlubMember(1)]
        public ushort PlayersOnline { get; set; }

        [BlubMember(2)]
        public ushort PlayerLimit { get; set; }

        [BlubMember(3)]
        public int Unk1 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string Rank { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Description { get; set; }

        [BlubMember(7, typeof(ColorSerializer))]
        public Color TextColor { get; set; }

        [BlubMember(8)]
        public uint MinLevel { get; set; }

        [BlubMember(9)]
        public uint MaxLevel { get; set; }

        [BlubMember(10)]
        public int Unk2 { get; set; }

        [BlubMember(11, typeof(ColorSerializer))]
        public Color TooltipColor { get; set; }
    }
}
