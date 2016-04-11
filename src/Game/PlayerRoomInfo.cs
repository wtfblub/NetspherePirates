using System;
using Netsphere.Game;
using Netsphere.Game.Systems;

namespace Netsphere
{
    internal class PlayerRoomInfo
    {
        public LongPeerId PeerId { get; set; } = 0;
        public bool IsConnecting { get; set; }
        public byte Slot { get; set; }

        public PlayerTeam Team { get; set; }
        public PlayerRecord Stats { get; set; }
        public PlayerState State { get; set; }
        public PlayerGameMode Mode { get; set; }
        public bool IsReady { get; set; }

        public TimeSpan PlayTime { get; set; }
        public TimeSpan[] CharacterPlayTime { get; set; } = { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero };

        public void Reset()
        {
            Stats?.Reset();
            PlayTime = TimeSpan.Zero;
            for (var i = 0; i < CharacterPlayTime.Length; i++)
                CharacterPlayTime[i] = TimeSpan.Zero;
            
            IsReady = false;
        }
    }
}
