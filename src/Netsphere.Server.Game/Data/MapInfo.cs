namespace Netsphere.Server.Game.Data
{
    public class MapInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GameRule GameRule { get; set; }
        public int PlayerLimit { get; set; }
        public bool IsEnabled { get; set; }

        public override string ToString()
        {
            return $"{Name}({GameRule})";
        }
    }
}
