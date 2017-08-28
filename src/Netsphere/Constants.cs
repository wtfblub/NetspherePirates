namespace Netsphere
{
    public enum SecurityLevel : byte
    {
        User = 0,
        GameMaster = 1,
        Developer = 2,
        Administrator = 3,
    }

    public enum ServerType : uint
    {
        Game = 3,
        Chat = 5,

        Relay = 10,
    }

    public enum Gender : byte
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public enum CharacterGender : byte
    {
        Male = 0,
        Female = 1
    }

    public enum CommunitySetting : byte
    {
        Allow = 0,
        FriendOnly = 1,
        Deny = 2
    }

    public enum RoomLeaveReason : byte
    {
        Left = 0,
        Kicked = 1,
        MasterAFK = 2,
        AFK = 3,
        ModeratorKick = 4,
        VoteKick = 5,
    }

    public enum VoteKickReason : byte
    {
        Hacking = 0,
        BadMannger = 1,
        BugUsing = 2,
        AFK = 3,
        Etc = 4
    }

    public enum MissionRewardType : byte
    {
        PEN = 1,
    }

    public enum ShopResourceType : byte
    {
        NewShopPrice = 1,
        NewShopEffect = 2,
        NewShopItem = 4,
        NewShopUniqueItem = 8
    }

//    public enum RandomShopResourceType : byte
//    {
//        EUNewRandomShop = 31,
//    }

    public enum CostumeSlot : byte
    {
        Hair = 0,
        Face = 1,
        Shirt = 2,
        Pants = 3,
        Gloves = 4,
        Shoes = 5,
        Accessory = 6,
        Pet = 7
    }

    public enum WeaponSlot : byte
    {
        Weapon1 = 0,
        Weapon2 = 1,
        Weapon3 = 2,
        None = 3,
    }

    public enum SkillSlot : byte
    {
        Skill = 0,
    }

    public enum ItemPriceType : uint
    {
        PEN = 1,
        AP = 2,
        Premium = 3,
        None = 4, //?
        CP = 5,
    }

    public enum ItemPeriodType : uint
    {
        None = 1,
        Hours = 2,
        Days = 3,
        Units = 4, // ?
    }

    public enum UseItemAction : byte
    {
        Equip = 1,
        UnEquip = 2,
        Unk4 = 4,
    }

    public enum InventoryAction : uint
    {
        Add = 1,
        Update = 2,
    }

    public enum ItemBuyResult : byte
    {
        DBError = 0,
        NotEnoughMoney = 1,
        UnkownItem = 2,
        OK = 3,
    }

    public enum ItemRepairResult : byte
    {
        Error0 = 0,
        Error1 = 1,
        Error2 = 2,
        Error3 = 3,
        NotEnoughMoney = 4,
        OK = 5,
        Error4 = 6,
    }

    public enum ItemRefundResult : byte
    {
        OK = 0,
        Failed = 1
    }

    public enum CapsuleRewardType : uint
    {
        PEN = 1,
        Item = 2,
    }

    public enum GameRule : uint
    {
        Deathmatch = 1,
        Touchdown = 2,
        Survival = 3,
        Practice = 4,
        Tutorial = 5,
        SemiTouchdown = 6, // Dev
        Arcade = 7,
        Chaser = 8,
        BattleRoyal = 9,
        Captain = 10,

        //Siege = 11,
        //Horde = 12,
        //Challenge = 13,
        //Random = 14,
    }

    public enum GameState : uint
    {
        Waiting = 1,
        Playing = 2,
        Result = 3
    }

    public enum GameTimeState : uint
    {
        FirstHalf = 1,
        HalfTime = 2,
        SecondHalf = 3,
    }

    public enum Team : byte
    {
        Neutral = 0,
        Alpha = 1,
        Beta = 2,
    }

    public enum PlayerGameMode : byte
    {
        Normal = 1,
        Spectate = 2,
    }

    public enum PlayerState : byte
    {
        Alive = 0,
        Dead = 1,
        Waiting = 2,
        Spectating = 3,
        Lobby = 4,
    }

    public enum GameEventMessage : byte
    {
        ChangedTeamTo = 1,
        EnteredRoom = 2,
        LeftRoom = 3,
        Kicked = 4,
        MasterAFK = 5,
        AFK = 6,
        KickedByModerator = 7,
        BallReset = 8,
        StartGame = 9,
        TouchdownAlpha = 10,
        TouchdownBeta = 11,
        ChatMessage = 13,
        TeamMessage = 14,
        ResetRound = 15,
        NextRoundIn = 16,
        ResultIn = 17,
        HalfTimeIn = 18,
        RespawnIn = 21,
        GodmodeForXSeconds = 22,
        PlayerRatioMsgBox = 23,
        CantStartGame = 24,
        UserEntering = 25,
        UserNotReady = 26,
        RoomModeIsChanging = 27,
        ChaserIn = 28,
    }

    //    public enum VoteKickReason : byte
    //    {
    //        Hacking = 0,
    //        BadMannger = 1,
    //        BugUsing = 2,
    //        AFK = 3,
    //        Etc = 4,
    //    }

    public enum ItemCategory : byte
    {
        Costume = 1,
        Weapon = 2,
        Skill = 3,
        OneTimeUse = 4,
        Boost = 5,
        Coupon = 6,
        EsperChip = 7
    }

    public enum ItemLicense : byte
    {
        None = 0,
        PlasmaSword = 1,
        CounterSword = 2,
        StormBat = 26,
        // AssssinClaw ToDo
        SubmachineGun = 3,
        Revolver = 4,
        SemiRifle = 25,
        // SMG3 - DualGun
        // HandGun
        // SMG4 BurstShotGun
        HeavymachineGun = 5,
        GaussRifle = 27,
        RailGun = 6,
        Cannonade = 7,
        Sentrygun = 8,
        SentiForce = 9,
        SentiNel = 28,
        MineGun = 10,
        MindEnergy = 11,
        MindShock = 12,

        // Skills
        Anchoring = 13,
        Flying = 14,
        Invisible = 15,
        Detect = 16,
        Shield = 17,
        Block = 18,
        Bind = 19,
        Metallic = 20,
        Berserk = 21,
    }

    public enum ChannelCategory : byte
    {
        Speed = 0,
        Club = 3,
    }

    public enum DenyAction : uint
    {
        Add = 0,
        Remove = 1,
    }

    public enum ActorState : byte
    {
        Spectate,
        Ghost,
        Respawn,
        Wait,
        Standby,
        Normal,
        Run,
        Sit,
        Jump,
        BoundJump,
        Fall,
        DodgeLeft,
        DodgeRight,
        Stun,
        Down,
        StandUp,
        Blow,
        BoundBlow,
        Damage,
        CounterAttackDamage,
        DownDamage,
        FastRun,
        DodgeLeftAfterStun,
        DodgeRightAfterStun,
        JumpAfterAnchoring,
        Reload,
        SocialAction,
        ResultAction,
        Death,
        Idle,
        Destruction,

        SkillFly,
        SkillAnchoring,
        SkillStealth,
        SkillShield,
        SkillWallCreation,
        SkillBind,
        SkillMetalic,
        SkillBerserk,

        UseWeapon1,
        UseWeapon2,
        UseWeapon2Weak,
        UseWeapon2Strong,
        UseWeapon1Weak,
        UseWeapon1Strong,
        UseWeapon1Jump,
        UseWeapon1Strong1,
        UseWeapon1Attack2,
        UseWeapon1Attack3,
        UseWeapon1Attack4,
        UseWeapon1Attack5,
        UseWeapon1CounterAttack,
        ArcadeResultAction,
        Total
    }

    public enum AttackAttribute : byte
    {
        PlasmaSwordCritical = 1,
        PlasmaSwordStandWeak,
        PlasmaSwordStandStrong,
        PlasmaSwordAttack2Weak,
        PlasmaSwordAttack2,
        PlasmaSwordJumpCritical,
        PlasmaSwordJump,
        SubmachineGun,
        MachineGunLower,
        MachineGunMiddle,
        MachineGunUpper,
        AimedShot,
        AimedShot2,
        MineLauncher,
        MindEnergy,
        SentryGunMachineGun,
        BoundBlow,
        KillOneSelf,
        SentiWallWave,
        SentiNelWave,
        Revolver,
        CannonadeShot,
        CannonadeShot2,
        CounterSwordCounterCritical,
        CounterSwordCounterAttack,
        CounterSwordCritical,
        CounterSwordAttack1,
        CounterSwordAttack2,
        CounterSwordAttack3,
        CounterSwordAttack4,
        CounterSwordJumpDash,
        MindStormAttack1,
        MindStormAttack2,
        Smg2,
        BatSwordStandWeak,
        BatSwordStandStrong,
        BatSwordAttack2Weak,
        BatSwordAttack2,
        BatSwordCritical,
        BatSwordJumpCritical,
        BatSwordJump,
        KatanaSwordCritical,
        KatanaSwordAttack1,
        KatanaSwordAttack2,
        KatanaSwordAttack3,
        KatanaSwordAttack4,
        KatanaSwordJumpCritical,
        KatanaSwordJump,
        CardAttack1,
        CardAttack2,
        CardAttack3,
        Mg2,
        AssassinClaw,
        Smg3,
        Revolver2,
        Smg4,
        Smg3Gun,
        Smg3Sword,
        SpyDaggerCritical,
        SpyDaggerAttack1,
        SpyDaggerAttack2,
        SpydaggerAttack3,
        SpyDaggerJumpCritical,
        SpyDaggerJump,
        DoubleSwordCritical,
        DoubleSwordAttack1,
        DoubleSwordAttack2,
        DoubleSwordAttack3,
        DoubleSwordAttack4,
        DoubleSwordJumpDash,
        Airgun,
        Smg2Homing,
        EarthBomber,
        LightBomber,
        ChainLightGun,
        SparkRifle,
        ChainLightGunExplosion,
        BossVirusKnuckle,
        BossVirusSmoke,
        BossVirusExplosion,
        BossVirusStun,
        BossShotaKnuckle,
        BossShotaAssault,
        BossShotaLaser,
        TRAAttack1,
        TRBAttack1Left,
        TRBAttack1Right,
        VirusAttack1,
        TRAAttack2Left,
        TRAAttack2Right,
        TRBAttack2,
        TrabigExplosion,
        TeamChange,
    }

    public enum Condition : uint
    {
        Blow = 2,
        Push = 4,
        Stun = 8,
        Bind = 16,
    }

    public enum Attribute
    {
        HP,
        MP,
        EXP,
        PEN,
        MeleeDefense,
        HeavyDefense,
        InstallDefense,
        ThrowDefense,
        MindDefense,
        MissileDefense,
        MissileDefenseHead,
        MissileDefenseUpper,
        MissileDefenseLower,
        SnipeDefense,
        SnipeDefenseHead,
        SnipeDefenseUpper,
        SnipeDefenseLower,
        AllWeaponDefense,
        PlasmaAttack,
        CounterAttack,
        StormAttack,
        KatanaAttack,
        HeavyAttack,
        SentryAttack,
        SentiWallAttack,
        SentiNelAttack,
        MineAttack,
        MindEnergy,
        MindShock,
        CardAttack,
        SubmachineAttack,
        SemiAttack,
        RevolverAttack,
        GaussrifleAttack,
        RailAttack,
        CannonadeAttack,
        ClawAttack,
        Smg3Attack,
        Revolver2Attack,
        Smg4Attack,
        SpydaggerAttack,
        MindOrora,
        DoubleswordAttack,
        AirgunAttack,
        Smg2HomingAttack,
        EarthBombAttack,
        LightBombAttack,
        ChainLightGunAttack,
        SparkRifleAttack,
        AllWeaponAttack,
        SubmachineReload,
        SemiReload,
        RevolverReload,
        Revolver2Reload,
        GaussrifleReload,
        MineReload,
        HeavyReload,
        RailReload,
        CannonadeReload,
        MindEnergyReload,
        MindShockReload,
        Smg3Reload,
        Smg4Reload,
        MindOroraReload,
        AirgunReload,
        Smg2HomingReload,
        EarthBombReload,
        LightBombReload,
        ChainLightGunReload,
        SparkRifleReload,
        SubmachineReloadammo,
        SemiReloadammo,
        RevolverReloadammo,
        Revolver2Reloadammo,
        GaussrifleReloadammo,
        MineReloadammo,
        HeavyReloadammo,
        RailReloadammo,
        CannonadeReloadammo,
        MindEnergyReloadammo,
        MindShockReloadammo,
        SentryReloadammo,
        SentiWallReloadammo,
        SentiNelReloadammo,
        Smg3Reloadammo,
        Smg4Reloadammo,
        MindOroraReloadammo,
        AirgunReloadammo,
        Smg2HomingReloadammo,
        EarthBombReloadammo,
        LightBombReloadammo,
        ChainLightGunReloadammo,
        SparkRifleReloadammo,
        MineMaxammo,
        EarthBombMaxammo,
        LightBombMaxammo,
        ChaserCastRate,
        ChaserMovespeed,
        Movespeed,
        SentryHP,
        SentryAtkDistance,
        SentryBuildTime
    }

    public enum ChatType : uint
    {
        Channel = 0,
        Club = 1
    }

    public enum WeaponCategory
    {
        Melee = 0,
        RifleGun = 1,
        HeavyGun = 2,
        Sniper = 3,
        Sentry = 4,
        Bomb = 5,
        Mind = 6
    }
}
