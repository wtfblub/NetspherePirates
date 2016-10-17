PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS "main"."accounts";
CREATE TABLE "accounts" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Username"  TEXT(40) NOT NULL COLLATE NOCASE ,
"Nickname"  TEXT(40) COLLATE NOCASE ,
"Password"  TEXT(40) COLLATE NOCASE ,
"Salt"  TEXT(40) COLLATE NOCASE ,
"SecurityLevel"  INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for bans
-- ----------------------------
DROP TABLE IF EXISTS "main"."bans";
CREATE TABLE "bans" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"AccountId"  INTEGER NOT NULL,
"Date"  INTEGER NOT NULL DEFAULT 0,
"Duration"  INTEGER,
"Reason"  TEXT(255) COLLATE NOCASE ,
CONSTRAINT "fkey0" FOREIGN KEY ("AccountId") REFERENCES "accounts" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for login_history
-- ----------------------------
DROP TABLE IF EXISTS "main"."login_history";
CREATE TABLE "login_history" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"AccountId"  INTEGER NOT NULL,
"Date"  INTEGER NOT NULL DEFAULT 0,
"IP"  TEXT(15),
CONSTRAINT "fkey0" FOREIGN KEY ("AccountId") REFERENCES "accounts" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for nickname_history
-- ----------------------------
DROP TABLE IF EXISTS "main"."nickname_history";
CREATE TABLE "nickname_history" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"AccountId"  INTEGER NOT NULL,
"Nickname"  TEXT(40) NOT NULL COLLATE NOCASE ,
"ExpireDate"  INTEGER,
CONSTRAINT "fkey0" FOREIGN KEY ("AccountId") REFERENCES "accounts" ("Id") ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for sqlite_sequence
-- ----------------------------
DROP TABLE IF EXISTS "main"."sqlite_sequence";
CREATE TABLE sqlite_sequence(name,seq);

-- ----------------------------
-- Indexes structure for table accounts
-- ----------------------------
CREATE UNIQUE INDEX "main"."Nickname"
ON "accounts" ("Nickname" ASC);
CREATE UNIQUE INDEX "main"."Username"
ON "accounts" ("Username" ASC);
PRAGMA foreign_keys = ON;
