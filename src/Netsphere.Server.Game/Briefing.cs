using System.IO;
using BlubLib.IO;

namespace Netsphere.Server.Game
{
    public class Briefing
    {
        public TeamId WinnerTeam { get; set; }
        public BriefingTeam[] Teams { get; set; }
        public BriefingPlayer[] Players { get; set; }
        public ulong[] Spectators { get; set; }

        public byte[] GetData()
        {
            using (var w = new MemoryStream().ToBinaryWriter(false))
            {
                Serialize(w);
                return w.ToArray();
            }
        }

        protected virtual void Serialize(BinaryWriter w)
        {
            w.Write((int)WinnerTeam);
            w.Write(Teams.Length);
            w.Write(Players.Length);
            w.Write(Spectators.Length);

            foreach (var team in Teams)
                team.Serialize(w);

            foreach (var plr in Players)
                plr.Serialize(w);

            foreach (var spectator in Spectators)
            {
                w.Write(spectator);
                w.Write((long)0);
            }
        }
    }

    public class BriefingTeam
    {
        public TeamId TeamId { get; set; }
        public uint Score { get; set; }

        public BriefingTeam()
        {
        }

        public BriefingTeam(TeamId teamId, uint score)
        {
            TeamId = teamId;
            Score = score;
        }

        public virtual void Serialize(BinaryWriter w)
        {
            w.WriteEnum(TeamId);
            w.Write(Score);
        }
    }

    public class BriefingPlayer
    {
        public ulong AccountId { get; set; }
        public uint Experience { get; set; }
        public TeamId TeamId { get; set; }
        public PlayerState State { get; set; }
        public PlayerGameMode Mode { get; set; }
        public bool IsReady { get; set; }
        public uint TotalScore { get; set; }

        // Result data
        public uint ExperienceGained { get; set; }
        public uint BonusExperienceGained { get; set; }
        public uint PENGained { get; set; }
        public uint BonusPENGained { get; set; }
        public bool LevelUp { get; set; }

        public virtual void Serialize(BinaryWriter w)
        {
            w.Write(AccountId);
            w.WriteEnum(TeamId);
            w.WriteEnum(State);
            w.Write(IsReady);
            w.Write((int)Mode);
            w.Write(TotalScore);
            w.Write(0);
            w.Write(PENGained);
            w.Write(ExperienceGained);
            w.Write(Experience);
            w.Write(LevelUp);
            w.Write(BonusExperienceGained);
            w.Write(BonusPENGained);
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
            w.Write((byte)0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
        }
    }
}
