using Netsphere;

namespace WebApi.Models
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreationTimestamp { get; set; }

        public ulong MasterId { get; set; }
        public ulong HostId { get; set; }

        public MapDto Map { get; set; }
        public GameRule GameRule { get; set; }
        public GameState State { get; set; }
        public GameTimeState TimeState { get; set; }

        public int PlayerLimit { get; set; }
        public int SpectatorLimit { get; set; }
        public string Password { get; set; }
        public int TimeLimit { get; set; }
        public ushort ScoreLimit { get; set; }
        public bool IsFriendly { get; set; }
        public bool IsBalanced { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int ItemLimit { get; set; }
        public bool IsNoIntrusion { get; set; }

        public RoomPlayerDto[] Players { get; set; }
    }
}
