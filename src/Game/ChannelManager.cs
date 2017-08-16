using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Netsphere.Database.Game;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    internal class ChannelManager : IReadOnlyCollection<Channel>
    {
        private readonly ConcurrentDictionary<uint, Channel> _channels = new ConcurrentDictionary<uint, Channel>();


        public Channel this[uint id] => GetChannel(id);

        #region Events

        public event EventHandler<ChannelPlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<ChannelPlayerLeftEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(ChannelPlayerJoinedEventArgs e)
        {
            PlayerJoined?.Invoke(this, e);
        }

        protected virtual void OnPlayerLeft(ChannelPlayerLeftEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
        }

        #endregion

        public ChannelManager(IEnumerable<ChannelDto> channelInfos)
        {
            foreach (var info in channelInfos)
            {
                var channel = new Channel
                {
                    Id = info.Id,
                    Name = info.Name,
                    Description = info.Description,
                    PlayerLimit = info.PlayerLimit,
                    MinLevel = info.MinLevel,
                    MaxLevel = info.MaxLevel,
                    Color = Color.FromArgb(info.Color),
                    TooltipColor = Color.FromArgb(info.TooltipColor)
                };
                channel.Color = Color.FromArgb(channel.Color.R, channel.Color.G, channel.Color.B);
                channel.TooltipColor = Color.FromArgb(channel.TooltipColor.R, channel.TooltipColor.G, channel.TooltipColor.B);

                channel.PlayerJoined += (s, e) => OnPlayerJoined(e);
                channel.PlayerLeft += (s, e) => OnPlayerLeft(e);
                _channels.TryAdd((uint)info.Id, channel);
            }
        }

        public Channel GetChannel(uint id)
        {
            Channel channel;
            _channels.TryGetValue(id, out channel);
            return channel;
        }

        public void Update(TimeSpan delta)
        {
            foreach (var channel in _channels.Values)
                channel.Update(delta);
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
