namespace Netsphere.Common.Messaging
{
    public class ServerShutdownMessage
    {
        public ushort Id { get; set; }
        public ServerType ServerType { get; set; }
    }
}
