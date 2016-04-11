using System;
using System.IO;
using System.Runtime.ExceptionServices;
using Netsphere.Network;

// ReSharper disable once CheckNamespace
namespace Netsphere.Game
{
    internal abstract class PlayerRecord
    {
        public Player Player { get; }
        public abstract uint TotalScore { get; }
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint Suicides { get; set; }
        public uint Deaths { get; set; }

        protected PlayerRecord(Player player)
        {
            Player = player;
        }

        public virtual uint GetPenGain(out uint bonusPen)
        {
            bonusPen = 0;
            return 0;
        }

        public virtual uint GetExpGain(out uint bonusExp)
        {
            bonusExp = 0;
            return 0;
        }

        public virtual void Reset()
        {
            Kills = 0;
            KillAssists = 0;
            Suicides = 0;
            Deaths = 0;
        }

        public virtual void Serialize(BinaryWriter w, bool isResult)
        {
            w.Write(Player.Account.Id);
            w.WriteEnum(Player.RoomInfo.Team.Team);
            w.WriteEnum(Player.RoomInfo.State);
            w.Write(Player.RoomInfo.IsReady);
            w.Write((uint)Player.RoomInfo.Mode);
            w.Write(TotalScore);
            w.Write(0);

            uint bonusPen = 0;
            uint bonusExp = 0;
            var rankUp = false;
            if (isResult)
            {
                w.Write(GetPenGain(out bonusPen));

                var expGain = GetExpGain(out bonusExp);

                rankUp = Player.GainExp(expGain);

                w.Write(expGain);
            }
            else
            {
                w.Write(0);
                w.Write(0);
            }

            w.Write(Player.TotalExperience);
            w.Write(rankUp);
            w.Write(bonusExp);
            w.Write(bonusPen);
            w.Write(0);

            /*
                1 PC Room(korean internet cafe event)
                2 PEN+
                4 EXP+
                8 20%
                16 25%
                32 30%
            */
            w.Write(0);
            w.Write((byte)0);
            w.Write((byte)0);
            w.Write((byte)0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
        }
    }
}
