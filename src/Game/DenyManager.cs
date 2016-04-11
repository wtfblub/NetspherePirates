using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Netsphere.Network;

namespace Netsphere
{
    internal class DenyManager : IReadOnlyCollection<Deny>
    {
        private readonly ConcurrentDictionary<ulong, Deny> _denies = new ConcurrentDictionary<ulong, Deny>();
        private readonly ConcurrentStack<Deny> _deniesToRemove = new ConcurrentStack<Deny>();

        public Player Player { get; }
        public int Count => _denies.Count;
        public Deny this[ulong accountId] => _denies.GetValueOrDefault(accountId);

        public DenyManager(Player plr, PlayerDto dto)
        {
            Player = plr;

            foreach (var denyDto in dto.Ignores)
            {
                var deny = new Deny(denyDto);
                _denies.TryAdd(deny.DenyId, deny);
            }
        }

        public Deny Add(Player plr)
        {
            var deny = new Deny(plr.Account.Id, plr.Account.Nickname);
            if (!_denies.TryAdd(deny.DenyId, deny))
                throw new ArgumentException("Player is already ignored", nameof(plr));
            return deny;
        }

        public bool Remove(Deny deny)
        {
            return Remove(deny.DenyId);
        }

        public bool Remove(ulong accountId)
        {
            var deny = this[accountId];
            if (deny == null)
                return false;

            _denies.Remove(accountId);
            if (deny.ExistsInDatabase)
                _deniesToRemove.Push(deny);
            return true;
        }

        internal void Save()
        {
            Deny denyToRemove;
            while (_deniesToRemove.TryPop(out denyToRemove))
                GameDatabase.Instance.PlayerDeny.GetReference(denyToRemove.Id).Delete();

            foreach (var deny in _denies.Values.Where(deny => !deny.ExistsInDatabase))
            {
                var denyDto = GameDatabase.Instance.Players
                    .GetReference((int)Player.Account.Id)
                    .Ignores.Create(deny.Id);
                deny.ExistsInDatabase = true;
                
                denyDto.DenyPlayer = GameDatabase.Instance.Players
                    .GetReference((int)deny.DenyId);
            }
        }

        public bool Contains(ulong accountId)
        {
            return _denies.ContainsKey(accountId);
        }

        public IEnumerator<Deny> GetEnumerator()
        {
            return _denies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class Deny
    {
        internal bool ExistsInDatabase { get; set; }

        public int Id { get; }
        public ulong DenyId { get; }
        public string Nickname { get; internal set; }

        internal Deny(PlayerDenyDto dto)
        {
            ExistsInDatabase = true;
            Id = dto.Id;
            DenyId = (ulong)dto.DenyPlayer.Id;

            // Try a fast lookup first in case the player is currently online
            // otherwise get the name from the database
            Nickname = GameServer.Instance.PlayerManager[DenyId]?.Account.Nickname ??
                AuthDatabase.Instance.Accounts.GetReference((int)DenyId).Nickname;
        }

        internal Deny(ulong accountId, string nickname)
        {
            Id = DenyIdGenerator.GetNextId();
            DenyId = accountId;
            Nickname = nickname;
        }
    }
}
