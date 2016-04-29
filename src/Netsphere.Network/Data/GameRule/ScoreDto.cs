using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ScoreDto
    {
        [BlubMember(0)]
        public LongPeerId Killer { get; set; }

        [BlubMember(1, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [BlubMember(2)]
        public LongPeerId Target { get; set; }

        [BlubMember(3)]
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

    [BlubContract]
    public class Score2Dto
    {
        [BlubMember(0)]
        public LongPeerId Killer { get; set; }

        [BlubMember(1, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [BlubMember(2)]
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
