using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class UserDataItemDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public short Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }

        [BlubMember(5)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        public UserDataItemDto()
        {
            Unk6 = Array.Empty<int>();
        }
    }
}
