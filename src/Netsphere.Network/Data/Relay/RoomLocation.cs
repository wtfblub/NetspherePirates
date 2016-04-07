namespace Netsphere.Network.Data.Relay
{
    public struct RoomLocation
    {
        public uint Value => this;

        public byte ServerId { get; set; }
        public byte ChannelId { get; set; }
        public ushort RoomId { get; set; }

        public RoomLocation(uint value)
        {
            ServerId = (byte)(value / 100000);

            var tmp = ServerId * 100000;
            ChannelId = (byte)((value - tmp) / 1000);

            tmp = ChannelId * 1000 + tmp;
            RoomId = (ushort)(value - tmp);
        }

        public RoomLocation(byte serverId, byte channelId, ushort roomId)
        {
            ServerId = serverId;
            ChannelId = channelId;
            RoomId = roomId;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator uint(RoomLocation roomLocation)
        {
            return (uint)(roomLocation.RoomId + 100000 * roomLocation.ServerId + 1000 * roomLocation.ChannelId);
        }

        public static implicit operator RoomLocation(uint value)
        {
            return new RoomLocation(value);
        }
    }
}
