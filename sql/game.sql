SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for license_rewards
-- ----------------------------
DROP TABLE IF EXISTS `license_rewards`;
CREATE TABLE `license_rewards` (
  `Id` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `ShopItemInfoId` int(11) NOT NULL,
  `ShopPriceId` int(11) NOT NULL,
  `Color` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ShopItemInfoId` (`ShopItemInfoId`),
  KEY `ShopPriceId` (`ShopPriceId`),
  CONSTRAINT `license_rewards_ibfk_1` FOREIGN KEY (`ShopItemInfoId`) REFERENCES `shop_iteminfos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `license_rewards_ibfk_2` FOREIGN KEY (`ShopPriceId`) REFERENCES `shop_prices` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_characters
-- ----------------------------
DROP TABLE IF EXISTS `player_characters`;
CREATE TABLE `player_characters` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Gender` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `BasicHair` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `BasicFace` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `BasicShirt` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `BasicPants` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Weapon1Id` int(11) DEFAULT NULL,
  `Weapon2Id` int(11) DEFAULT NULL,
  `Weapon3Id` int(11) DEFAULT NULL,
  `SkillId` int(11) DEFAULT NULL,
  `HairId` int(11) DEFAULT NULL,
  `FaceId` int(11) DEFAULT NULL,
  `ShirtId` int(11) DEFAULT NULL,
  `PantsId` int(11) DEFAULT NULL,
  `GlovesId` int(11) DEFAULT NULL,
  `ShoesId` int(11) DEFAULT NULL,
  `AccessoryId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  KEY `Weapon1Id` (`Weapon1Id`),
  KEY `Weapon2Id` (`Weapon2Id`),
  KEY `Weapon3Id` (`Weapon3Id`),
  KEY `SkillId` (`SkillId`),
  KEY `HairId` (`HairId`),
  KEY `FaceId` (`FaceId`),
  KEY `ShirtId` (`ShirtId`),
  KEY `PantsId` (`PantsId`),
  KEY `GlovesId` (`GlovesId`),
  KEY `ShoesId` (`ShoesId`),
  KEY `AccessoryId` (`AccessoryId`),
  CONSTRAINT `player_characters_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `player_characters_ibfk_10` FOREIGN KEY (`GlovesId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_11` FOREIGN KEY (`ShoesId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_12` FOREIGN KEY (`AccessoryId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_2` FOREIGN KEY (`Weapon1Id`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_3` FOREIGN KEY (`Weapon2Id`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_4` FOREIGN KEY (`Weapon3Id`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_5` FOREIGN KEY (`SkillId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_6` FOREIGN KEY (`HairId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_7` FOREIGN KEY (`FaceId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_8` FOREIGN KEY (`ShirtId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `player_characters_ibfk_9` FOREIGN KEY (`PantsId`) REFERENCES `player_items` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_deny
-- ----------------------------
DROP TABLE IF EXISTS `player_deny`;
CREATE TABLE `player_deny` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `DenyPlayerId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  KEY `DenyPlayerId` (`DenyPlayerId`),
  CONSTRAINT `player_deny_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `player_deny_ibfk_2` FOREIGN KEY (`DenyPlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_items
-- ----------------------------
DROP TABLE IF EXISTS `player_items`;
CREATE TABLE `player_items` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `ShopItemInfoId` int(11) NOT NULL,
  `ShopPriceId` int(11) NOT NULL,
  `Effect` int(11) NOT NULL DEFAULT '0',
  `Color` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `PurchaseDate` bigint(20) NOT NULL DEFAULT '0',
  `Durability` int(11) NOT NULL DEFAULT '0',
  `Count` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  KEY `ShopItemInfoId` (`ShopItemInfoId`),
  KEY `ShopPriceId` (`ShopPriceId`),
  CONSTRAINT `player_items_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `player_items_ibfk_2` FOREIGN KEY (`ShopItemInfoId`) REFERENCES `shop_iteminfos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `player_items_ibfk_3` FOREIGN KEY (`ShopPriceId`) REFERENCES `shop_prices` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_licenses
-- ----------------------------
DROP TABLE IF EXISTS `player_licenses`;
CREATE TABLE `player_licenses` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `License` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `FirstCompletedDate` bigint(20) NOT NULL DEFAULT '0',
  `CompletedCount` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  CONSTRAINT `player_licenses_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_mails
-- ----------------------------
DROP TABLE IF EXISTS `player_mails`;
CREATE TABLE `player_mails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PlayerId` int(11) NOT NULL,
  `SenderPlayerId` int(11) NOT NULL,
  `SentDate` bigint(20) NOT NULL DEFAULT '0',
  `Title` varchar(100) NOT NULL DEFAULT '',
  `Message` varchar(500) NOT NULL DEFAULT '',
  `IsMailNew` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `IsMailDeleted` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  KEY `SenderPlayerId` (`SenderPlayerId`),
  CONSTRAINT `player_mails_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `player_mails_ibfk_2` FOREIGN KEY (`SenderPlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for player_settings
-- ----------------------------
DROP TABLE IF EXISTS `player_settings`;
CREATE TABLE `player_settings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PlayerId` int(11) NOT NULL,
  `Setting` varchar(512) NOT NULL DEFAULT '',
  `Value` varchar(512) NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  KEY `PlayerId` (`PlayerId`),
  CONSTRAINT `player_settings_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `players` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for players
-- ----------------------------
DROP TABLE IF EXISTS `players`;
CREATE TABLE `players` (
  `Id` int(11) NOT NULL,
  `TutorialState` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Level` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `TotalExperience` int(11) NOT NULL DEFAULT '0',
  `PEN` int(11) NOT NULL DEFAULT '0',
  `AP` int(11) NOT NULL DEFAULT '0',
  `Coins1` int(11) NOT NULL DEFAULT '0',
  `Coins2` int(11) NOT NULL DEFAULT '0',
  `CurrentCharacterSlot` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- ----------------------------
-- Table structure for shop_effect_groups
-- ----------------------------
DROP TABLE IF EXISTS `shop_effect_groups`;
CREATE TABLE `shop_effect_groups` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_effects
-- ----------------------------
DROP TABLE IF EXISTS `shop_effects`;
CREATE TABLE `shop_effects` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `EffectGroupId` int(11) NOT NULL,
  `Effect` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `EffectGroupId` (`EffectGroupId`),
  CONSTRAINT `shop_effects_ibfk_1` FOREIGN KEY (`EffectGroupId`) REFERENCES `shop_effect_groups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_iteminfos
-- ----------------------------
DROP TABLE IF EXISTS `shop_iteminfos`;
CREATE TABLE `shop_iteminfos` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ShopItemId` int(11) unsigned NOT NULL,
  `PriceGroupId` int(11) NOT NULL,
  `EffectGroupId` int(11) NOT NULL,
  `DiscountPercentage` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `IsEnabled` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `PriceGroupId` (`PriceGroupId`),
  KEY `EffectGroupId` (`EffectGroupId`),
  KEY `ShopItemId` (`ShopItemId`),
  CONSTRAINT `shop_iteminfos_ibfk_2` FOREIGN KEY (`PriceGroupId`) REFERENCES `shop_price_groups` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `shop_iteminfos_ibfk_3` FOREIGN KEY (`EffectGroupId`) REFERENCES `shop_effect_groups` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `shop_iteminfos_ibfk_4` FOREIGN KEY (`ShopItemId`) REFERENCES `shop_items` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_items
-- ----------------------------
DROP TABLE IF EXISTS `shop_items`;
CREATE TABLE `shop_items` (
  `Id` int(10) unsigned NOT NULL,
  `RequiredGender` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `RequiredLicense` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Colors` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `UniqueColors` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `RequiredLevel` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `LevelLimit` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `RequiredMasterLevel` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `IsOneTimeUse` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `IsDestroyable` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_price_groups
-- ----------------------------
DROP TABLE IF EXISTS `shop_price_groups`;
CREATE TABLE `shop_price_groups` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) DEFAULT '',
  `PriceType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_prices
-- ----------------------------
DROP TABLE IF EXISTS `shop_prices`;
CREATE TABLE `shop_prices` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PriceGroupId` int(11) NOT NULL,
  `PeriodType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Period` int(11) NOT NULL DEFAULT '0',
  `Price` int(11) NOT NULL DEFAULT '0',
  `IsRefundable` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Durability` int(11) NOT NULL DEFAULT '0',
  `IsEnabled` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `PriceGroupId` (`PriceGroupId`),
  CONSTRAINT `shop_prices_ibfk_1` FOREIGN KEY (`PriceGroupId`) REFERENCES `shop_price_groups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_version
-- ----------------------------
DROP TABLE IF EXISTS `shop_version`;
CREATE TABLE `shop_version` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Version` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for start_items
-- ----------------------------
DROP TABLE IF EXISTS `start_items`;
CREATE TABLE `start_items` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ShopItemInfoId` int(11) NOT NULL,
  `ShopPriceId` int(11) NOT NULL,
  `ShopEffectId` int(11) NOT NULL,
  `Color` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Count` int(11) NOT NULL DEFAULT '0',
  `RequiredSecurityLevel` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ShopItemInfoId` (`ShopItemInfoId`),
  KEY `ShopPriceId` (`ShopPriceId`),
  KEY `ShopEffectId` (`ShopEffectId`),
  CONSTRAINT `start_items_ibfk_1` FOREIGN KEY (`ShopItemInfoId`) REFERENCES `shop_iteminfos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `start_items_ibfk_2` FOREIGN KEY (`ShopPriceId`) REFERENCES `shop_prices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `start_items_ibfk_3` FOREIGN KEY (`ShopEffectId`) REFERENCES `shop_effects` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
SET FOREIGN_KEY_CHECKS=1;
