using System;

namespace Netsphere.Common.Configuration
{
    public class ServerListOptions
    {
        public string Name { get; set; }
        public ushort GroupId { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}
