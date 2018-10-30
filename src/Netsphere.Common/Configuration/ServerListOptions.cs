using System;

namespace Netsphere.Common.Configuration
{
    public class ServerListOptions
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}
