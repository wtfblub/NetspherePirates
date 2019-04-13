using System.Drawing;

namespace Netsphere.Server.Game.Data
{
    public class ChannelInfo
    {
        public uint Id { get; set; }
        public ChannelCategory Category { get; set; }
        public int PlayerLimit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rank { get; set; }
        public Color Color { get; set; }
        public Color TooltipColor { get; set; }
        public uint MinLevel { get; set; }
        public uint MaxLevel { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
