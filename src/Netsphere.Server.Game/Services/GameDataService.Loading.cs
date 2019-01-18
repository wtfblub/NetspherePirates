using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Configuration;
using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Resource.xml;
using Netsphere.Resource.Xml;
using Netsphere.Server.Game.Data;

namespace Netsphere.Server.Game.Services
{
    public partial class GameDataService
    {
        public void LoadLevels()
        {
            _logger.Information("Loading levels...");
            var dto = Deserialize<ExperienceDto>("xml/experience.x7");
            Levels = Transform().ToImmutableDictionary(x => x.Level, x => x);
            _logger.Information("Loaded {Count} levels", Levels.Count);

            IEnumerable<LevelInfo> Transform()
            {
                for (var i = 0; i < dto.exp.Length; ++i)
                {
                    yield return new LevelInfo
                    {
                        Level = i,
                        ExperienceToNextLevel = dto.exp[i].require,
                        TotalExperience = dto.exp[i].accumulate
                    };
                }
            }
        }

        public void LoadChannels()
        {
            _logger.Information("Loading channels...");
            var dto = Deserialize<ChannelSettingDto>("xml/_eu_channel_setting.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/channel_setting_string_table.x7");
            Channels = Transform().ToImmutableArray();
            _logger.Information("Loaded {Count} channels", Channels.Length);

            IEnumerable<ChannelInfo> Transform()
            {
                foreach (var channelDto in dto.channel_info)
                {
                    var channel = new ChannelInfo
                    {
                        Id = channelDto.id,
                        Category = (ChannelCategory)channelDto.category,
                        PlayerLimit = dto.setting.limit_player,
                        Type = channelDto.type
                    };

                    var name = stringTable.@string.First(x =>
                        x.key.Equals(channelDto.name_key, StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrWhiteSpace(name.eng))
                        throw new Exception("Missing english translation for " + channelDto.name_key);

                    channel.Name = name.eng;
                    yield return channel;
                }
            }
        }

        public void LoadMaps()
        {
            _logger.Information("Loading maps...");
            var dto = Deserialize<GameInfoDto>("xml/_eu_gameinfo.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/gameinfo_string_table.x7");
            Maps = Transform().ToImmutableArray();
            _logger.Information("Loaded {Count} maps", Maps.Length);

            IEnumerable<MapInfo> Transform()
            {
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
                        _logger.Warning("bginfo_path:{biginfo} not found", mapDto.bginfo_path);
                        continue;
                    }

                    using (var ms = new MemoryStream(data))
                        map.Config = IniFile.Load(ms);

                    foreach (var enabledMode in map.Config["MAPINFO"]
                        .Where(pair => pair.Key.StartsWith("enableMode", StringComparison.InvariantCultureIgnoreCase))
                        .Select(pair => pair.Value))
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

                    var name = stringTable.@string.FirstOrDefault(s =>
                        s.key.Equals(mapDto.map_name_key, StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrWhiteSpace(name?.eng))
                    {
                        _logger.Warning("Missing english translation for {MapKey}", mapDto.map_name_key);
                        map.Name = mapDto.map_name_key;
                    }
                    else
                    {
                        map.Name = name.eng;
                    }

                    yield return map;
                }
            }
        }

        public void LoadEffects()
        {
            _logger.Information("Loading effects...");
            var dto = Deserialize<ItemEffectDto>("xml/item_effect.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/item_effect_string_table.x7");
            Effects = Transform().ToImmutableDictionary(x => x.Id, x => x);
            _logger.Information("Loaded {Count} effects", Effects.Count);

            IEnumerable<ItemEffect> Transform()
            {
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
                            Value = float.Parse(attributeDto.value, CultureInfo.InvariantCulture),
                            Rate = float.Parse(attributeDto.rate, CultureInfo.InvariantCulture)
                        });
                    }

                    var name = stringTable.@string.First(s =>
                        s.key.Equals(itemEffectDto.text_key, StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrWhiteSpace(name.eng))
                    {
                        _logger.Warning("Missing english translation for item effect {textKey}", itemEffectDto.text_key);
                        name.eng = itemEffectDto.NAME;
                    }

                    itemEffect.Name = name.eng;
                    yield return itemEffect;
                }
            }
        }

        public void LoadItems()
        {
            _logger.Information("Loading items...");
            var dto = Deserialize<ItemInfoDto>("xml/iteminfo.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.x7");
            Items = Transform().ToImmutableDictionary(x => x.ItemNumber, x => x);
            _logger.Information("Loaded {Count} items", Items.Count);

            IEnumerable<ItemInfo> Transform()
            {
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

                            var name = stringTable.@string.FirstOrDefault(s =>
                                s.key.Equals(itemDto.@base.base_info.name_key, StringComparison.InvariantCultureIgnoreCase));
                            if (string.IsNullOrWhiteSpace(name?.eng))
                            {
                                _logger.Warning("Missing english translation for {id}",
                                    name != null ? itemDto.@base.base_info.name_key : id.ToString());
                                item.Name = name != null ? name.key : itemDto.NAME;
                            }
                            else
                            {
                                item.Name = name.eng;
                            }

                            yield return item;
                        }
                    }
                }
            }

            ItemLicense ParseItemLicense(string license)
            {
                bool Equals(string str)
                {
                    return license.Equals(str, StringComparison.InvariantCultureIgnoreCase);
                }

                if (Equals("license_none"))
                    return ItemLicense.None;

                if (Equals("LICENSE_CHECK_NONE"))
                    return ItemLicense.None;

                if (Equals("LICENSE_PLASMA_SWORD"))
                    return ItemLicense.PlasmaSword;

                if (Equals("license_counter_sword"))
                    return ItemLicense.CounterSword;

                if (Equals("LICENSE_STORM_BAT"))
                    return ItemLicense.StormBat;

                if (Equals("LICENSE_ASSASSIN_CLAW"))
                    return ItemLicense.None; // ToDo

                if (Equals("LICENSE_SUBMACHINE_GUN"))
                    return ItemLicense.SubmachineGun;

                if (Equals("license_revolver"))
                    return ItemLicense.Revolver;

                if (Equals("license_semi_rifle"))
                    return ItemLicense.SemiRifle;

                if (Equals("LICENSE_SMG3"))
                    return ItemLicense.None; // ToDo

                if (Equals("license_HAND_GUN"))
                    return ItemLicense.None; // ToDo

                if (Equals("LICENSE_SMG4"))
                    return ItemLicense.None; // ToDo

                if (Equals("LICENSE_HEAVYMACHINE_GUN"))
                    return ItemLicense.HeavymachineGun;

                if (Equals("LICENSE_GAUSS_RIFLE"))
                    return ItemLicense.GaussRifle;

                if (Equals("license_rail_gun"))
                    return ItemLicense.RailGun;

                if (Equals("license_cannonade"))
                    return ItemLicense.Cannonade;

                if (Equals("LICENSE_CENTRYGUN"))
                    return ItemLicense.Sentrygun;

                if (Equals("license_centi_force"))
                    return ItemLicense.SentiForce;

                if (Equals("LICENSE_SENTINEL"))
                    return ItemLicense.SentiNel;

                if (Equals("license_mine_gun"))
                    return ItemLicense.MineGun;

                if (Equals("LICENSE_MIND_ENERGY"))
                    return ItemLicense.MindEnergy;

                if (Equals("license_mind_shock"))
                    return ItemLicense.MindShock;

                // SKILLS

                if (Equals("LICENSE_ANCHORING"))
                    return ItemLicense.Anchoring;

                if (Equals("LICENSE_FLYING"))
                    return ItemLicense.Flying;

                if (Equals("LICENSE_INVISIBLE"))
                    return ItemLicense.Invisible;

                if (Equals("license_detect"))
                    return ItemLicense.Detect;

                if (Equals("LICENSE_SHIELD"))
                    return ItemLicense.Shield;

                if (Equals("LICENSE_BLOCK"))
                    return ItemLicense.Block;

                if (Equals("LICENSE_BIND"))
                    return ItemLicense.Bind;

                if (Equals("LICENSE_METALLIC"))
                    return ItemLicense.Metallic;

                throw new Exception("Invalid license " + license);
            }

            Gender ParseGender(string gender)
            {
                bool Equals(string str)
                {
                    return gender.Equals(str, StringComparison.InvariantCultureIgnoreCase);
                }

                if (Equals("all"))
                    return Gender.None;

                if (Equals("woman"))
                    return Gender.Female;

                if (Equals("man"))
                    return Gender.Male;

                throw new Exception("Invalid gender " + gender);
            }

            ItemInfo LoadAction(ItemNumber id, ItemInfoItemDto itemDto)
            {
                if (itemDto.action == null)
                {
                    _logger.Warning("Missing action for item {id}", id);
                    return new ItemInfoAction();
                }

                var item = new ItemInfoAction
                {
                    RequiredMP = float.Parse(itemDto.action.ability.required_mp, CultureInfo.InvariantCulture),
                    DecrementMP = float.Parse(itemDto.action.ability.decrement_mp, CultureInfo.InvariantCulture),
                    DecrementMPDelay = float.Parse(itemDto.action.ability.decrement_mp_delay, CultureInfo.InvariantCulture)
                };

                if (itemDto.action.@float != null)
                {
                    item.ValuesF = itemDto.action.@float
                        .Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();
                }

                if (itemDto.action.integer != null)
                    item.Values = itemDto.action.integer.Select(i => i.value).ToList();

                return item;
            }

            ItemInfo LoadWeapon(ItemNumber id, ItemInfoItemDto itemDto)
            {
                if (itemDto.weapon == null)
                {
                    _logger.Warning("Missing weapon for item {id}", id);
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
                    Range = string.IsNullOrWhiteSpace(ability.range)
                        ? 0
                        : float.Parse(ability.range, CultureInfo.InvariantCulture),
                    SupportSniperMode = ability.support_sniper_mode > 0,
                    SniperModeFov = ability.sniper_mode_fov > 0,
                    AutoTargetDistance = ability.auto_target_distance == null
                        ? 0
                        : float.Parse(ability.auto_target_distance, CultureInfo.InvariantCulture)
                };

                if (itemDto.weapon.@float != null)
                {
                    item.ValuesF = itemDto.weapon.@float
                        .Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();
                }

                if (itemDto.weapon.integer != null)
                    item.Values = itemDto.weapon.integer.Select(i => i.value).ToList();

                return item;
            }
        }

        public void LoadDefaultItems()
        {
            _logger.Information("Loading default items...");
            var dto = Deserialize<DefaultItemDto>("xml/default_item.x7");
            DefaultItems = Transform().ToImmutableArray();
            _logger.Information("Loaded {Count} default items", DefaultItems.Length);

            IEnumerable<DefaultItem> Transform()
            {
                foreach (var itemDto in dto.male.item)
                {
                    var item = new DefaultItem
                    {
                        ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
                        Gender = CharacterGender.Male,
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
                        Variation = itemDto.variation
                    };
                    yield return item;
                }
            }
        }

        public async Task LoadShop()
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                _logger.Information("Loading effect groups...");
                var effects = await db.EffectGroups.Include(x => x.ShopEffects).ToArrayAsync();
                ShopEffects = effects.ToImmutableDictionary(x => x.Id, x => new ShopEffectGroup(x));
                _logger.Information("Loaded {Count} effect groups", ShopEffects.Count);

                _logger.Information("Loading price groups...");
                var prices = await db.PriceGroups.Include(x => x.ShopPrices).ToArrayAsync();
                ShopPrices = prices.ToImmutableDictionary(x => x.Id, x => new ShopPriceGroup(x));
                _logger.Information("Loaded {Count} price groups", ShopPrices.Count);

                _logger.Information("Loading shop items...");
                var items = await db.Items.Include(x => x.ItemInfos).ToArrayAsync();
                ShopItems = items.ToImmutableDictionary(x => (ItemNumber)x.Id, x => new ShopItem(x, this));
                _logger.Information("Loaded {Count} shop items", ShopItems.Count);

                _logger.Information("Loading license rewards...");
                var licenseRewards = await db.LicenseRewards.ToArrayAsync();
                LicenseRewards = licenseRewards.ToImmutableDictionary(x => (ItemLicense)x.Id, x => new LicenseReward(x, this));
                _logger.Information("Loaded {Count} license rewards", LicenseRewards.Count);

                var version = await db.ShopVersion.FirstOrDefaultAsync();
                if (version == null)
                {
                    _logger.Warning("No shop version found in database! Using current timestamp");
                    version = new ShopVersionEntity { Version = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() };
                }

                ShopVersion = version.Version;
                _logger.Information("Loaded shop version {Version}", ShopVersion);
            }
        }

        public void LoadGameTempos()
        {
            _logger.Information("Loading default game tempos...");
            var dto = Deserialize<ConstantInfoDto>("xml/constant_info.x7");
            GameTempos = Transform().ToImmutableDictionary(x => x.Name, x => x);
            _logger.Information("Loaded {Count} game tempos", GameTempos.Count);

            IEnumerable<GameTempo> Transform()
            {
                foreach (var gameTempoDto in dto.GAMEINFOLIST)
                {
                    var gameTempo = new GameTempo
                    {
                        Name = gameTempoDto.TEMPVALUE.value
                    };

                    var values = gameTempoDto.GAMETEPMO_COMMON_TOTAL_VALUE;
                    gameTempo.ActorDefaultHPMax =
                        float.Parse(values.GAMETEMPO_actor_default_hp_max, CultureInfo.InvariantCulture);
                    gameTempo.ActorDefaultMPMax =
                        float.Parse(values.GAMETEMPO_actor_default_mp_max, CultureInfo.InvariantCulture);
                    gameTempo.ActorDefaultMoveSpeed = values.GAMETEMPO_fastrun_required_mp;

                    yield return gameTempo;
                }
            }
        }
    }
}
