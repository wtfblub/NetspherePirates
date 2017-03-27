using DotNetty.Common.Utilities;

namespace Netsphere.API
{
    internal static class ChannelAttributes
    {
        public static readonly AttributeKey<ChannelState> State = AttributeKey<ChannelState>.ValueOf(nameof(State));
    }
}
