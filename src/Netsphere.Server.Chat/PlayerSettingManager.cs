using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToDB;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;

namespace Netsphere.Server.Chat
{
    public class PlayerSettingManager : ISaveable
    {
        private static readonly IDictionary<string, IPlayerSettingConverter> s_converters;

        private readonly IdGeneratorService _idGeneratorService;
        private readonly IDictionary<string, Setting> _settings;

        public Session Session { get; }

        static PlayerSettingManager()
        {
            s_converters = new ConcurrentDictionary<string, IPlayerSettingConverter>();
            var communtiySettingConverter = new CommunitySettingConverter();
            RegisterConverter(PlayerSetting.AllowCombiInvite, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowFriendRequest, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowRoomInvite, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowInfoRequest, communtiySettingConverter);
        }

        public PlayerSettingManager(Session session, PlayerEntity entity, IdGeneratorService idGeneratorService)
        {
            _idGeneratorService = idGeneratorService;
            _settings = new ConcurrentDictionary<string, Setting>();
            Session = session;

            foreach (var settingEntity in entity.Settings)
            {
                var setting = new Setting(settingEntity.Id, GetObject(settingEntity.Setting, settingEntity.Value));
                setting.SetExistsState(true);
                _settings[settingEntity.Setting] = setting;
            }
        }

        public bool Contains(string name)
        {
            return _settings.ContainsKey(name);
        }

        public T Get<T>(string name)
        {
            if (!_settings.TryGetValue(name, out var setting))
                throw new Exception($"Setting {name} not found");

            return (T)setting.Data;
        }

        public string Get(string name)
        {
            if (!_settings.TryGetValue(name, out var setting))
                throw new Exception($"Setting {name} not found");

            return (string)setting.Data;
        }

        public void AddOrUpdate(string name, string value)
        {
            if (_settings.TryGetValue(name, out var setting))
                setting.Data = value;
            else
                _settings[name] = new Setting(_idGeneratorService.GetNextId(IdKind.Setting), value);
        }

        public void AddOrUpdate<T>(string name, T value)
        {
            if (_settings.TryGetValue(name, out var setting))
                setting.Data = value;
            else
                _settings[name] = new Setting(_idGeneratorService.GetNextId(IdKind.Setting), value);
        }

        public async Task Save(GameContext db)
        {
            foreach (var pair in _settings)
            {
                var name = pair.Key;
                var setting = pair.Value;
                if (!setting.Exists)
                {
                    await db.InsertAsync(new PlayerSettingEntity
                    {
                        Id = setting.Id,
                        PlayerId = (int)Session.AccountId,
                        Setting = name,
                        Value = GetString(name, setting.Data)
                    });
                    setting.SetExistsState(true);
                }
                else
                {
                    if (!setting.IsDirty)
                        continue;

                    await db.UpdateAsync(setting);
                    setting.SetDirtyState(false);
                }
            }
        }

        #region Converter
        public static void RegisterConverter(string name, IPlayerSettingConverter converter)
        {
            if (!s_converters.TryAdd(name, converter))
                throw new Exception($"Converter for {name} already registered");
        }

        public static void RegisterConverter(PlayerSetting name, IPlayerSettingConverter converter)
        {
            RegisterConverter(name.ToString(), converter);
        }

        private static IPlayerSettingConverter GetConverter(string name)
        {
            s_converters.TryGetValue(name, out var converter);
            return converter;
        }

        private static object GetObject(string name, string value)
        {
            var converter = GetConverter(name);
            return converter != null ? converter.GetObject(value) : value;
        }

        private static string GetString(string name, object value)
        {
            var converter = GetConverter(name);
            return converter != null ? converter.GetString(value) : (string)value;
        }
        #endregion

        private class Setting : DatabaseObject
        {
            private object _data;

            public long Id { get; }
            public object Data
            {
                get => _data;
                set => SetIfChanged(ref _data, value);
            }

            public Setting(long id, object data)
            {
                Id = id;
                _data = data;
            }
        }
    }

    public interface IPlayerSettingConverter
    {
        object GetObject(string value);
        string GetString(object value);
    }

    internal class CommunitySettingConverter : IPlayerSettingConverter
    {
        public object GetObject(string value)
        {
            if (!Enum.TryParse(value, out CommunitySetting setting))
                throw new Exception($"CommunitySetting {value} not found");

            return setting;
        }

        public string GetString(object value)
        {
            return value.ToString();
        }
    }
}
