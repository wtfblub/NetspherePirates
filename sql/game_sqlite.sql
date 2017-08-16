PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for channels
-- ----------------------------
DROP TABLE IF EXISTS "main"."channels";
CREATE TABLE "channels" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Name"  TEXT NOT NULL,
"Description"  TEXT NOT NULL,
"PlayerLimit"  INTEGER NOT NULL,
"MinLevel"  INTEGER NOT NULL,
"MaxLevel"  INTEGER NOT NULL,
"Color"  INTEGER NOT NULL,
"TooltipColor"  INTEGER NOT NULL
);

-- ----------------------------
-- Table structure for license_rewards
-- ----------------------------
DROP TABLE IF EXISTS "main"."license_rewards";
CREATE TABLE "license_rewards" (
"Id"  INTEGER NOT NULL DEFAULT 0,
"ShopItemInfoId"  INTEGER NOT NULL,
"ShopPriceId"  INTEGER NOT NULL,
"Color"  INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY ("Id" ASC),
FOREIGN KEY ("ShopItemInfoId") REFERENCES "shop_iteminfos" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("ShopPriceId") REFERENCES "shop_prices" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for player_characters
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_characters";
CREATE TABLE "player_characters" (
"Id"  INTEGER NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"Slot"  INTEGER NOT NULL DEFAULT 0,
"Gender"  INTEGER NOT NULL DEFAULT 0,
"BasicHair"  INTEGER NOT NULL DEFAULT 0,
"BasicFace"  INTEGER NOT NULL DEFAULT 0,
"BasicShirt"  INTEGER NOT NULL DEFAULT 0,
"BasicPants"  INTEGER NOT NULL DEFAULT 0,
"Weapon1Id"  INTEGER DEFAULT NULL,
"Weapon2Id"  INTEGER DEFAULT NULL,
"Weapon3Id"  INTEGER DEFAULT NULL,
"SkillId"  INTEGER DEFAULT NULL,
"HairId"  INTEGER DEFAULT NULL,
"FaceId"  INTEGER DEFAULT NULL,
"ShirtId"  INTEGER DEFAULT NULL,
"PantsId"  INTEGER DEFAULT NULL,
"GlovesId"  INTEGER DEFAULT NULL,
"ShoesId"  INTEGER DEFAULT NULL,
"AccessoryId"  INTEGER DEFAULT NULL,
PRIMARY KEY ("Id" ASC),
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("Weapon1Id") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("Weapon2Id") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("Weapon3Id") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("SkillId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("HairId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("FaceId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("ShirtId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("PantsId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("GlovesId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("ShoesId") REFERENCES "player_items" ("Id") ON DELETE SET NULL,
FOREIGN KEY ("AccessoryId") REFERENCES "player_items" ("Id") ON DELETE SET NULL
);

-- ----------------------------
-- Table structure for player_deny
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_deny";
CREATE TABLE "player_deny" (
"Id"  INTEGER NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"DenyPlayerId"  INTEGER NOT NULL,
PRIMARY KEY ("Id" ASC),
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("DenyPlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for player_items
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_items";
CREATE TABLE "player_items" (
"Id"  INTEGER NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"ShopItemInfoId"  INTEGER NOT NULL,
"ShopPriceId"  INTEGER NOT NULL,
"Effect"  INTEGER NOT NULL DEFAULT 0,
"Color"  INTEGER NOT NULL DEFAULT 0,
"PurchaseDate"  INTEGER NOT NULL DEFAULT 0,
"Durability"  INTEGER NOT NULL DEFAULT 0,
"Count"  INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY ("Id" ASC),
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("ShopItemInfoId") REFERENCES "shop_iteminfos" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("ShopPriceId") REFERENCES "shop_prices" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for player_licenses
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_licenses";
CREATE TABLE "player_licenses" (
"Id"  INTEGER NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"License"  INTEGER NOT NULL DEFAULT 0,
"FirstCompletedDate"  INTEGER NOT NULL DEFAULT 0,
"CompletedCount"  INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY ("Id" ASC),
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for player_mails
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_mails";
CREATE TABLE "player_mails" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"SenderPlayerId"  INTEGER NOT NULL,
"SentDate"  INTEGER NOT NULL DEFAULT 0,
"Title"  TEXT NOT NULL,
"Message"  TEXT NOT NULL,
"IsMailNew"  INTEGER NOT NULL DEFAULT 0,
"IsMailDeleted"  INTEGER NOT NULL DEFAULT 0,
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("SenderPlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for player_settings
-- ----------------------------
DROP TABLE IF EXISTS "main"."player_settings";
CREATE TABLE "player_settings" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"PlayerId"  INTEGER NOT NULL,
"Setting"  TEXT(512) NOT NULL COLLATE NOCASE ,
"Value"  TEXT(512) NOT NULL COLLATE NOCASE ,
FOREIGN KEY ("PlayerId") REFERENCES "players" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for players
-- ----------------------------
DROP TABLE IF EXISTS "main"."players";
CREATE TABLE "players" (
"Id"  INTEGER NOT NULL,
"TutorialState"  INTEGER NOT NULL DEFAULT 0,
"Level"  INTEGER NOT NULL DEFAULT 0,
"TotalExperience"  INTEGER NOT NULL DEFAULT 0,
"PEN"  INTEGER NOT NULL DEFAULT 0,
"AP"  INTEGER NOT NULL DEFAULT 0,
"Coins1"  INTEGER NOT NULL DEFAULT 0,
"Coins2"  INTEGER NOT NULL DEFAULT 0,
"CurrentCharacterSlot"  INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY ("Id")
);

-- ----------------------------
-- Table structure for shop_effect_groups
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_effect_groups";
CREATE TABLE "shop_effect_groups" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Name"  TEXT(20) NOT NULL COLLATE NOCASE 
);

-- ----------------------------
-- Table structure for shop_effects
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_effects";
CREATE TABLE "shop_effects" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"EffectGroupId"  INTEGER NOT NULL,
"Effect"  INTEGER NOT NULL DEFAULT 0,
FOREIGN KEY ("EffectGroupId") REFERENCES "shop_effect_groups" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for shop_iteminfos
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_iteminfos";
CREATE TABLE "shop_iteminfos" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"ShopItemId"  INTEGER NOT NULL,
"PriceGroupId"  INTEGER NOT NULL,
"EffectGroupId"  INTEGER NOT NULL,
"DiscountPercentage"  INTEGER NOT NULL DEFAULT 0,
"IsEnabled"  INTEGER NOT NULL DEFAULT 0,
FOREIGN KEY ("ShopItemId") REFERENCES "shop_items" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("PriceGroupId") REFERENCES "shop_price_groups" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("EffectGroupId") REFERENCES "shop_effect_groups" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for shop_items
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_items";
CREATE TABLE "shop_items" (
"Id"  INTEGER NOT NULL,
"RequiredGender"  INTEGER NOT NULL DEFAULT 0,
"RequiredLicense"  INTEGER NOT NULL DEFAULT 0,
"Colors"  INTEGER NOT NULL DEFAULT 0,
"UniqueColors"  INTEGER NOT NULL DEFAULT 0,
"RequiredLevel"  INTEGER NOT NULL DEFAULT 0,
"LevelLimit"  INTEGER NOT NULL DEFAULT 0,
"RequiredMasterLevel"  INTEGER NOT NULL DEFAULT 0,
"IsOneTimeUse"  INTEGER NOT NULL DEFAULT 0,
"IsDestroyable"  INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY ("Id")
);

-- ----------------------------
-- Table structure for shop_price_groups
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_price_groups";
CREATE TABLE "shop_price_groups" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Name"  TEXT(20) NOT NULL COLLATE NOCASE ,
"PriceType"  INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for shop_prices
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_prices";
CREATE TABLE "shop_prices" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"PriceGroupId"  INTEGER NOT NULL,
"PeriodType"  INTEGER NOT NULL DEFAULT 0,
"Period"  INTEGER NOT NULL DEFAULT 0,
"Price"  INTEGER NOT NULL DEFAULT 0,
"IsRefundable"  INTEGER NOT NULL DEFAULT 0,
"Durability"  INTEGER NOT NULL DEFAULT 0,
"IsEnabled"  INTEGER NOT NULL DEFAULT 0,
FOREIGN KEY ("PriceGroupId") REFERENCES "shop_price_groups" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for shop_version
-- ----------------------------
DROP TABLE IF EXISTS "main"."shop_version";
CREATE TABLE "shop_version" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Version"  TEXT(40) NOT NULL COLLATE NOCASE 
);

-- ----------------------------
-- Table structure for start_items
-- ----------------------------
DROP TABLE IF EXISTS "main"."start_items";
CREATE TABLE "start_items" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"ShopItemInfoId"  INTEGER NOT NULL,
"ShopPriceId"  INTEGER NOT NULL,
"ShopEffectId"  INTEGER NOT NULL,
"Color"  INTEGER NOT NULL DEFAULT 0,
"Count"  INTEGER NOT NULL DEFAULT 0,
"RequiredSecurityLevel"  INTEGER NOT NULL DEFAULT 0,
FOREIGN KEY ("ShopItemInfoId") REFERENCES "shop_iteminfos" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("ShopPriceId") REFERENCES "shop_prices" ("Id") ON DELETE CASCADE,
FOREIGN KEY ("ShopEffectId") REFERENCES "shop_effects" ("Id") ON DELETE CASCADE
);
PRAGMA foreign_keys = ON;
