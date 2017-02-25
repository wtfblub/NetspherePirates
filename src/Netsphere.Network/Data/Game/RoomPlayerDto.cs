﻿using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RoomPlayerDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(3)]
        public byte Unk2 { get; set; }

        public RoomPlayerDto()
        {
            Nickname = "";
        }
    }
}
