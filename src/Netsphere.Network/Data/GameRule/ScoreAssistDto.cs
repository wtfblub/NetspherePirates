using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    public class ScoreAssistDto
    {
        [Serialize(0)]
        public LongPeerId Killer { get; set; }

        [Serialize(1)]
        public LongPeerId Assist { get; set; }

        [Serialize(2, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [Serialize(3)]
        public LongPeerId Target { get; set; }

        [Serialize(4)]
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

    public class ScoreAssist2Dto
    {
        [Serialize(0)]
        public LongPeerId Killer { get; set; }

        [Serialize(1)]
        public LongPeerId Assist { get; set; }

        [Serialize(2, typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Weapon { get; set; }

        [Serialize(3)]
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
