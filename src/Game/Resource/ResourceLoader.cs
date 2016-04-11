using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BlubLib.Configuration;
using Netsphere.Resource.xml;
using NLog;
using NLog.Fluent;

namespace Netsphere.Resource
{
    internal class ResourceLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public string ResourcePath { get; }

        public ResourceLoader(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        public byte[] GetBytes(string fileName)
        {
            var path = Path.Combine(ResourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public IEnumerable<Experience> LoadExperience()
        {
            var dto = Deserialize<ExperienceDto>("xml/experience.x7");

            var i = 0;
            return dto.exp.Select(expDto => new Experience
            {
                Level = i++,
                ExperienceToNextLevel = expDto.require,
                TotalExperience = expDto.accumulate
            });
        }

        public IEnumerable<ChannelInfo> LoadChannels()
        {
            var dto = Deserialize<ChannelSettingDto>("xml/_eu_channel_setting.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/channel_setting_string_table.xml");

            foreach (var channelDto in dto.channel_info)
            {
                var channel = new ChannelInfo
                {
                    Id = channelDto.id,
                    Category = (ChannelCategory)channelDto.category,
                    PlayerLimit = dto.setting.limit_player,
                    Type = channelDto.type
                };

                var name = stringTable.@string.First(s => s.key.Equals(channelDto.name_key, StringComparison.InvariantCultureIgnoreCase));
                if (string.IsNullOrWhiteSpace(name.eng))
                    throw new Exception("Missing english translation for " + channelDto.name_key);

                channel.Name = name.eng;
                yield return channel;
            }
        }

        public IEnumerable<MapInfo> LoadMaps()
        {
            var dto = Deserialize<GameInfoDto>("xml/_eu_gameinfo.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/gameinfo_string_table.xml");

            foreach (var mapDto in dto.map.Where(map => map.id != -1 && !map.dev_mode))
            {
                mapDto.bginfo_path = mapDto.bginfo_path.ToLower();

                var map = new MapInfo
                {
                    Id = (byte)mapDto.id,
                    MinLevel = mapDto.require_level,
                    ServerId = mapDto.require_server,
                    ChannelId = mapDto.require_channel,
                    RespawnType = mapDto.respawn_type
                };
                var data = GetBytes(mapDto.bginfo_path);
                if (data == null)
                {
                    Logger.Warn()
                        .Message("bginfo_path:{0} not found", mapDto.bginfo_path)
                        .Write();
                    continue;
                }

                using (var ms = new MemoryStream(data))
                    map.Config = IniFile.Load(ms);

                foreach (var enabledMode in map.Config["MAPINFO"].Where(pair => pair.Key.StartsWith("enableMode", StringComparison.InvariantCultureIgnoreCase)).Select(pair => pair.Value))
                {
                    switch (enabledMode.Value.ToLower())
                    {
                        case "sl":
                            map.GameRules.Add(GameRule.Chaser);
                            break;

                        case "t":
                            map.GameRules.Add(GameRule.Touchdown);
                            break;

                        case "c":
                            map.GameRules.Add(GameRule.Captain);
                            break;

                        case "f":
                            map.GameRules.Add(GameRule.BattleRoyal);
                            break;

                        case "d":
                            map.GameRules.Add(GameRule.Deathmatch);
                            break;

                        case "s":
                            map.GameRules.Add(GameRule.Survival);
                            break;

                        case "n":
                            map.GameRules.Add(GameRule.Practice);
                            break;

                        case "a":
                            map.GameRules.Add(GameRule.Arcade);
                            break;

                        case "std": // wtf is this?
                            break;
                        case "m": // wtf is this?
                            break;

                        default:
                            throw new Exception("Invalid game rule " + enabledMode);
                    }
                }

                var name = stringTable.@string.First(s => s.key.Equals(mapDto.map_name_key, StringComparison.InvariantCultureIgnoreCase));
                if (string.IsNullOrWhiteSpace(name.eng))
                    throw new Exception("Missing english translation for " + mapDto.map_name_key);

                map.Name = name.eng;
                yield return map;
            }
        }

        public IEnumerable<ItemEffect> LoadEffects()
        {
            var dto = Deserialize<ItemEffectDto>("xml/item_effect.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/item_effect_string_table.xml");

            foreach (var itemEffectDto in dto.item.Where(itemEffect => itemEffect.id != 0))
            {
                var itemEffect = new ItemEffect
                {
                    Id = itemEffectDto.id
                };

                foreach (var attributeDto in itemEffectDto.attribute)
                {
                    itemEffect.Attributes.Add(new ItemEffectAttribute
                    {
                        Attribute = (Attribute)Enum.Parse(typeof(Attribute), attributeDto.effect.Replace("_", ""), true),
                        Value = attributeDto.value,
                        Rate = float.Parse(attributeDto.rate, CultureInfo.InvariantCulture)
                    });
                }

                var name = stringTable.@string.First(s => s.key.Equals(itemEffectDto.text_key, StringComparison.InvariantCultureIgnoreCase));
                if (string.IsNullOrWhiteSpace(name.eng))
                {
                    Logger.Warn()
                        .Message("Missing english translation for item effect {0}", itemEffectDto.text_key)
                        .Write();

                    name.eng = itemEffectDto.NAME;
                }

                itemEffect.Name = name.eng;
                yield return itemEffect;
            }
        }

        public IEnumerable<GameTempo> LoadGameTempos()
        {
            var dto = Deserialize<ConstantInfoDto>("xml/constant_info.x7");

            foreach (var gameTempoDto in dto.GAMEINFOLIST)
            {
                var tempo = new GameTempo
                {
                    Name = gameTempoDto.TEMPVALUE.value
                };

                var values = gameTempoDto.GAMETEPMO_COMMON_TOTAL_VALUE;
                tempo.ActorDefaultHPMax = float.Parse(values.GAMETEMPO_actor_default_hp_max, CultureInfo.InvariantCulture);
                tempo.ActorDefaultMPMax = float.Parse(values.GAMETEMPO_actor_default_mp_max, CultureInfo.InvariantCulture);
                tempo.ActorDefaultMoveSpeed = values.GAMETEMPO_fastrun_required_mp;

                yield return tempo;
            }
        }

        #region DefaultItems

        public IEnumerable<DefaultItem> LoadDefaultItems()
        {
            var dto = Deserialize<DefaultItemDto>("xml/default_item.x7");

            foreach (var itemDto in dto.male.item)
            {
                var item = new DefaultItem
                {
                    ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
                    Gender = CharacterGender.Male,
                    //Slot = (byte) ParseDefaultItemSlot(itemDto.Value),
                    Variation = itemDto.variation
                };
                yield return item;
            }
            foreach (var itemDto in dto.female.item)
            {
                var item = new DefaultItem
                {
                    ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
                    Gender = CharacterGender.Female,
                    //Slot = (byte) ParseDefaultItemSlot(itemDto.Value),
                    Variation = itemDto.variation
                };
                yield return item;
            }
        }

        //private static CostumeSlot ParseDefaultItemSlot(string slot)
        //{
        //    Func<string, bool> equals = str => slot.Equals(str, StringComparison.InvariantCultureIgnoreCase);

        //    if (equals("hair"))
        //        return CostumeSlot.Hair;

        //    if (equals("face"))
        //        return CostumeSlot.Face;

        //    if (equals("coat"))
        //        return CostumeSlot.Shirt;

        //    if (equals("pants"))
        //        return CostumeSlot.Pants;

        //    if (equals("gloves"))
        //        return CostumeSlot.Gloves;

        //    if (equals("shoes"))
        //        return CostumeSlot.Shoes;

        //    throw new Exception("Invalid slot " + slot);
        //}

        #endregion

        #region Items

        public IEnumerable<ItemInfo> LoadItems()
        {
            var dto = Deserialize<ItemInfoDto>("xml/iteminfo.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.xml");

            foreach (var categoryDto in dto.category)
            {
                foreach (var subCategoryDto in categoryDto.sub_category)
                {
                    foreach (var itemDto in subCategoryDto.item)
                    {
                        var id = new ItemNumber(categoryDto.id, subCategoryDto.id, itemDto.number);
                        ItemInfo item;

                        switch (id.Category)
                        {
                            case ItemCategory.Skill:
                                item = LoadAction(id, itemDto);
                                break;

                            case ItemCategory.Weapon:
                                item = LoadWeapon(id, itemDto);
                                break;

                            default:
                                item = new ItemInfo();
                                break;
                        }

                        item.ItemNumber = id;
                        item.Level = itemDto.@base.base_info.require_level;
                        item.MasterLevel = itemDto.@base.base_info.require_master;
                        item.Gender = ParseGender(itemDto.SEX);
                        item.Image = itemDto.client.icon.image;

                        if (itemDto.@base.license != null)
                            item.License = ParseItemLicense(itemDto.@base.license.require);

                        var name = stringTable.@string.FirstOrDefault(s => s.key.Equals(itemDto.@base.base_info.name_key, StringComparison.InvariantCultureIgnoreCase));
                        if (string.IsNullOrWhiteSpace(name?.eng))
                        {
                            Logger.Warn()
                                .Message("Missing english translation for {0}", name != null ? itemDto.@base.base_info.name_key : id.ToString())
                                .Write();
                            item.Name = name != null ? name.key : itemDto.NAME;
                        }
                        else
                            item.Name = name.eng;

                        yield return item;
                    }
                }
            }
        }

        private static ItemLicense ParseItemLicense(string license)
        {
            Func<string, bool> equals = str => license.Equals(str, StringComparison.InvariantCultureIgnoreCase);

            if (equals("license_none"))
                return ItemLicense.None;

            if (equals("LICENSE_CHECK_NONE"))
                return ItemLicense.None;

            if (equals("LICENSE_PLASMA_SWORD"))
                return ItemLicense.PlasmaSword;

            if (equals("license_counter_sword"))
                return ItemLicense.CounterSword;

            if (equals("LICENSE_STORM_BAT"))
                return ItemLicense.StormBat;

            if (equals("LICENSE_ASSASSIN_CLAW"))
                return ItemLicense.None; // ToDo

            if (equals("LICENSE_SUBMACHINE_GUN"))
                return ItemLicense.SubmachineGun;

            if (equals("license_revolver"))
                return ItemLicense.Revolver;

            if (equals("license_semi_rifle"))
                return ItemLicense.SemiRifle;

            if (equals("LICENSE_SMG3"))
                return ItemLicense.None; // ToDo

            if (equals("license_HAND_GUN"))
                return ItemLicense.None; // ToDo

            if (equals("LICENSE_SMG4"))
                return ItemLicense.None; // ToDo

            if (equals("LICENSE_HEAVYMACHINE_GUN"))
                return ItemLicense.HeavymachineGun;

            if (equals("LICENSE_GAUSS_RIFLE"))
                return ItemLicense.GaussRifle;

            if (equals("license_rail_gun"))
                return ItemLicense.RailGun;

            if (equals("license_cannonade"))
                return ItemLicense.Cannonade;

            if (equals("LICENSE_CENTRYGUN"))
                return ItemLicense.Sentrygun;

            if (equals("license_centi_force"))
                return ItemLicense.SentiForce;

            if (equals("LICENSE_SENTINEL"))
                return ItemLicense.SentiNel;

            if (equals("license_mine_gun"))
                return ItemLicense.MineGun;

            if (equals("LICENSE_MIND_ENERGY"))
                return ItemLicense.MindEnergy;

            if (equals("license_mind_shock"))
                return ItemLicense.MindShock;

            // SKILLS

            if (equals("LICENSE_ANCHORING"))
                return ItemLicense.Anchoring;

            if (equals("LICENSE_FLYING"))
                return ItemLicense.Flying;

            if (equals("LICENSE_INVISIBLE"))
                return ItemLicense.Invisible;

            if (equals("license_detect"))
                return ItemLicense.Detect;

            if (equals("LICENSE_SHIELD"))
                return ItemLicense.Shield;

            if (equals("LICENSE_BLOCK"))
                return ItemLicense.Block;

            if (equals("LICENSE_BIND"))
                return ItemLicense.Bind;

            if (equals("LICENSE_METALLIC"))
                return ItemLicense.Metallic;

            throw new Exception("Invalid license " + license);
        }

        private static Gender ParseGender(string gender)
        {
            Func<string, bool> equals = str => gender.Equals(str, StringComparison.InvariantCultureIgnoreCase);

            if (equals("all"))
                return Gender.None;

            if (equals("woman"))
                return Gender.Female;

            if (equals("man"))
                return Gender.Male;

            throw new Exception("Invalid gender " + gender);
        }

        private static ItemInfo LoadAction(ItemNumber id, ItemInfoItemDto itemDto)
        {
            if (itemDto.action == null)
            {
                Logger.Warn()
                    .Message("Missing action for item {0}", id)
                    .Write();
                return new ItemInfoAction();
            }

            var item = new ItemInfoAction
            {
                RequiredMP = float.Parse(itemDto.action.ability.required_mp, CultureInfo.InvariantCulture),
                DecrementMP = float.Parse(itemDto.action.ability.decrement_mp, CultureInfo.InvariantCulture),
                DecrementMPDelay = float.Parse(itemDto.action.ability.decrement_mp_delay, CultureInfo.InvariantCulture)
            };

            if (itemDto.action.@float != null)
                item.ValuesF = itemDto.action.@float.Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();

            if (itemDto.action.integer != null)
                item.Values = itemDto.action.integer.Select(i => i.value).ToList();

            return item;
        }

        private static ItemInfo LoadWeapon(ItemNumber id, ItemInfoItemDto itemDto)
        {
            if (itemDto.weapon == null)
            {
                Logger.Warn()
                    .Message("Missing weapon for item {0}", id)
                    .Write();
                return new ItemInfoWeapon();
            }

            var ability = itemDto.weapon.ability;
            var item = new ItemInfoWeapon
            {
                Type = ability.type,
                RateOfFire = float.Parse(ability.rate_of_fire, CultureInfo.InvariantCulture),
                Power = float.Parse(ability.power, CultureInfo.InvariantCulture),
                MoveSpeedRate = float.Parse(ability.move_speed_rate, CultureInfo.InvariantCulture),
                AttackMoveSpeedRate = float.Parse(ability.attack_move_speed_rate, CultureInfo.InvariantCulture),
                MagazineCapacity = ability.magazine_capacity,
                CrackedMagazineCapacity = ability.cracked_magazine_capacity,
                MaxAmmo = ability.max_ammo,
                Accuracy = float.Parse(ability.accuracy, CultureInfo.InvariantCulture),
                Range = string.IsNullOrWhiteSpace(ability.range) ? 0 : float.Parse(ability.range, CultureInfo.InvariantCulture),
                SupportSniperMode = ability.support_sniper_mode > 0,
                SniperModeFov = ability.sniper_mode_fov > 0,
                AutoTargetDistance = ability.auto_target_distance == null ? 0 : float.Parse(ability.auto_target_distance, CultureInfo.InvariantCulture)
            };

            if (itemDto.weapon.@float != null)
                item.ValuesF = itemDto.weapon.@float.Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();

            if (itemDto.weapon.integer != null)
                item.Values = itemDto.weapon.integer.Select(i => i.value).ToList();

            return item;
        }

        #endregion

        private T Deserialize<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));

            var path = Path.Combine(ResourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            using (var r = new StreamReader(path))
                return (T)serializer.Deserialize(r);
        }
    }
}
