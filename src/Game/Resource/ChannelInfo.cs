namespace Netsphere.Resource
{
    public class ChannelInfo
    {
        public uint Id { get; set; }
        public ChannelCategory Category { get; set; }
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
        public byte Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
