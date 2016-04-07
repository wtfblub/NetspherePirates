using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    public class ScoreDto
    {
        [Serialize(0)]
        public LongPeerId Killer { get; set; }

        [Serialize(1, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [Serialize(2)]
        public LongPeerId Target { get; set; }

        [Serialize(3)]
        public byte Unk { get; set; }

        public ScoreDto()
        {
            Killer = 0;
            Target = 0;
        }

        public ScoreDto(LongPeerId killer, LongPeerId target, AttackAttribute weapon)
        {
            Killer = killer;
            Target = target;
            Weapon = weapon;
        }
    }

    public class Score2Dto
    {
        [Serialize(0)]
        public LongPeerId Killer { get; set; }

        [Serialize(1, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [Serialize(2)]
        public LongPeerId Target { get; set; }

        public Score2Dto()
        {
            Killer = 0;
            Target = 0;
        }

        public Score2Dto(LongPeerId killer, LongPeerId target, AttackAttribute weapon)
        {
            Killer = killer;
            Target = target;
            Weapon = weapon;
        }
    }
}
