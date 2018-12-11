namespace Netsphere.Common.Messaging
{
    public class PlayerUpdateMessage
    {
        public ulong AccountId { get; set; }
        public uint TotalExperience { get; set; }
        public uint RoomId { get; set; }
        public TeamId TeamId { get; set; }

        public PlayerUpdateMessage()
        {
        }

        public PlayerUpdateMessage(ulong accountId, uint totalExperience, uint roomId, TeamId teamId)
        {
            AccountId = accountId;
            TotalExperience = totalExperience;
            RoomId = roomId;
            TeamId = teamId;
        }
    }
}
