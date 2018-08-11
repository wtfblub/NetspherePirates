using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ScoreAssistDto
    {
        [BlubMember(0)]
        public LongPeerId Killer { get; set; }

        [BlubMember(1)]
        public LongPeerId Assist { get; set; }

        [BlubMember(2)]
        [BlubSerializer(typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [BlubMember(3)]
        public LongPeerId Target { get; set; }

        [BlubMember(4)]
        public byte Unk { get; set; }

        public ScoreAssistDto()
        {
            Killer = 0;
            Assist = 0;
            Target = 0;
        }

        public ScoreAssistDto(LongPeerId killer, LongPeerId assist, LongPeerId target, AttackAttribute weapon)
        {
            Killer = killer;
            Assist = assist;
            Target = target;
            Weapon = weapon;
        }
    }

    [BlubContract]
    public class ScoreAssist2Dto
    {
        [BlubMember(0)]
        public LongPeerId Killer { get; set; }

        [BlubMember(1)]
        public LongPeerId Assist { get; set; }

        [BlubMember(2)]
        [BlubSerializer(typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [BlubMember(3)]
        public LongPeerId Target { get; set; }

        public ScoreAssist2Dto()
        {
            Killer = 0;
            Assist = 0;
            Target = 0;
        }

        public ScoreAssist2Dto(LongPeerId killer, LongPeerId assist, LongPeerId target, AttackAttribute weapon)
        {
            Killer = killer;
            Assist = assist;
            Target = target;
            Weapon = weapon;
        }
    }
}
