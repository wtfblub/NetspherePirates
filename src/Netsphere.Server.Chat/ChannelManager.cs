using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Netsphere.Server.Chat
{
    public class ChannelManager : IReadOnlyCollection<Channel>
    {
        private readonly IDictionary<uint, Channel> _channels;

        public Channel this[uint id] => GetChannel(id);

        public event EventHandler<ChannelEventArgs> PlayerJoined;
        public event EventHandler<ChannelEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(Channel channel, Player plr)
        {
            PlayerJoined?.Invoke(this, new ChannelEventArgs(channel, plr));
        }

        protected virtual void OnPlayerLeft(Channel channel, Player plr)
        {
            PlayerLeft?.Invoke(this, new ChannelEventArgs(channel, plr));
        }

        public ChannelManager()
        {
            _channels = new ConcurrentDictionary<uint, Channel>();
        }

        public Channel GetChannel(uint id)
        {
            _channels.TryGetValue(id, out var channel);
            return channel;
        }

        internal Channel GetOrCreateChannel(uint id)
        {
            var channel = GetChannel(id);
            if (channel != null)
                return channel;

            channel = new Channel(id);
            if (_channels.TryAdd(id, channel))
            {
                channel.PlayerJoined += (_, e) => OnPlayerJoined(e.Channel, e.Player);
                channel.PlayerLeft += (_, e) => OnPlayerLeft(e.Channel, e.Player);
            }
            else
            {
                return GetChannel(id);
            }

            return channel;
        }

        #region IReadOnlyCollection
        public int Count => _channels.Count;

        public IEnumerator<Channel> GetEnumerator()
        {
            return _channels.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
