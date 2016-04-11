using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;
using Shaolinq;

namespace Netsphere
{
    internal class PlayerSettingManager
    {
        private static readonly IDictionary<string, IPlayerSettingConverter> Converter = new ConcurrentDictionary<string, IPlayerSettingConverter>();
        private readonly IDictionary<string, object> _settings = new ConcurrentDictionary<string, object>();

        public Player Player { get; }

        static PlayerSettingManager()
        {
            var communtiySettingConverter = new CommunitySettingConverter();
            RegisterConverter(PlayerSetting.AllowCombiInvite, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowFriendRequest, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowRoomInvite, communtiySettingConverter);
            RegisterConverter(PlayerSetting.AllowInfoRequest, communtiySettingConverter);
        }

        public PlayerSettingManager(Player player, PlayerDto dto)
        {
            Player = player;

            foreach (var settingDto in dto.Settings)
                _settings[settingDto.Setting] = GetObject(settingDto.Setting, settingDto.Value);
        }

        public bool Contains(string name)
        {
            return _settings.ContainsKey(name);
        }

        public T Get<T>(string name)
        {
            object value;
            if (!_settings.TryGetValue(name, out value))
                throw new Exception($"Setting {name} not found");

            return (T)value;
        }

        public string Get(string name)
        {
            object value;
            if (!_settings.TryGetValue(name, out value))
                throw new Exception($"Setting {name} not found");

            return (string)value;
        }

        public void AddOrUpdate(string name, string value)
        {
            using (var scope = new DataAccessScope())
            {
                if (_settings.ContainsKey(name))
                {
                    var dto = GameDatabase.Instance.Players
                        .First(plr => plr.Id == (int)Player.Account.Id)
                        .Settings
                        .First(s => s.Setting == name);
                    dto.Value = value;
                    _settings[name] = value;
                }
                else
                {
                    var dto = GameDatabase.Instance.Players
                        .First(plr => plr.Id == (int) Player.Account.Id);

                    var settingsDto = dto.Settings.Create();
                    settingsDto.Setting = name;
                    settingsDto.Value = value;
                    _settings[name] = value;
                }

                scope.Complete();
            }
        }

        public void AddOrUpdate<T>(string name, T value)
        {
            var converter = GetConverter(name);
            if (converter == null && typeof(T) != typeof(string))
                throw new Exception($"No PlayerSettingConverter for {name} found");

            var str = converter != null ? converter.GetString(value) : (string)(object)value;

            using (var scope = new DataAccessScope())
            {
                if (_settings.ContainsKey(name))
                {
                    var dto = GameDatabase.Instance.Players
                        .First(plr => plr.Id == (int)Player.Account.Id)
                        .Settings
                        .First(s => s.Setting == name);
                    dto.Value = str;
                    _settings[name] = value;
                }
                else
                {
                    var dto = GameDatabase.Instance.Players
                        .First(plr => plr.Id == (int)Player.Account.Id);

                    var settingsDto = dto.Settings.Create();
                    settingsDto.Setting = name;
                    settingsDto.Value = str;
                    _settings[name] = value;
                }

                scope.Complete();
            }
        }

        #region Converter

        public static void RegisterConverter(string name, IPlayerSettingConverter converter)
        {
            if (!Converter.TryAdd(name, converter))
                throw new Exception($"Converter for {name} already registered");
        }

        public static void RegisterConverter(PlayerSetting name, IPlayerSettingConverter converter)
        {
            RegisterConverter(name.ToString(), converter);
        }

        private static IPlayerSettingConverter GetConverter(string name)
        {
            IPlayerSettingConverter converter;
            Converter.TryGetValue(name, out converter);
            return converter;
        }

        private static object GetObject(string name, string value)
        {
            var converter = GetConverter(name);
            return converter != null ? converter.GetObject(value) : value;
        }

        #endregion
    }

    internal interface IPlayerSettingConverter
    {
        object GetObject(string value);
        string GetString(object value);
    }

    internal class CommunitySettingConverter : IPlayerSettingConverter
    {
        public object GetObject(string value)
        {
            CommunitySetting setting;
            if (!Enum.TryParse(value, out setting))
                throw new Exception($"CommunitySetting {value} not found");
            return setting;
        }

        public string GetString(object value)
        {
            return value.ToString();
        }
    }
}
