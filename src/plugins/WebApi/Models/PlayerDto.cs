namespace WebApi.Models
{
    public class PlayerDto
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public int Level { get; set; }
        public int TotalExperience { get; set; }
        public int PEN { get; set; }
        public int AP { get; set; }
        public byte ActiveCharacter { get; set; }
        public CharacterDto[] Characters { get; set; }
        public PlayerItemDto[] Inventory { get; set; }

        public uint? ChannelId { get; set; }
        public uint? RoomId { get; set; }
    }
}
