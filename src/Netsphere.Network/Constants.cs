namespace Netsphere.Network
{
    public enum AuthLoginResult : byte
    {
        OK = 0,
        WrongLogin = 1,
        Banned = 2,
        Failed = 3 // Used for permanent ban
    }

    public enum GameLoginResult : uint
    {
        OK = 0,
        ServerFull = 1,
        TerminateOtherConnection = 2,
        ExistingExit = 3,
        ServerFull2 = 4,
        WrongVersion = 5,
        ChooseNickname = 6,

        FailedAndRestart = 7,
        SessionTimeout = 8
    }

    public enum ServerResult : uint
    {
        ServerError = 0,
        QuickJoinFailed = 1,
        AlreadyPlaying = 2,
        NonExistingChannel = 3,
        ChannelLimitReached = 4,
        ChannelEnter = 5,
        ServerLimitReached = 6,
        PlayerLimitReached = 7,
        RoomChangingRules = 8,
        ChannelLeave = 9,
        PlayerNotFound = 10,
        CreateCharacterFailed = 11,
        DeleteCharacterFailed = 12,
        SelectCharacterFailed = 13,
        CreateNicknameSuccess = 14,
        NicknameUnavailable = 15,
        NicknameAvailable = 16,
        PasswordError = 17,
        WelcomeToS4World = 18,
        IPLocked = 19,
        ForbiddenToConnectFor5Min = 20,
        UserAlreadyExist = 21,
        DBError = 22,
        CreateCharacterFailed2 = 23,
        JoinChannelFailed = 24,
        RequiredChannelLicense = 25,
        WearingUnusableItem = 26,
        CannotSellWearingItem = 27,
        CantEnterRoom = 29,
        ImpossibleToEnterRoom = 30,
        CantReadClanInfo = 31,
        TaskCompensationError = 32,
        FailedToRequestTask = 33,
        ItemExchangeFailed = 34,
        ItemExchangeFailed2 = 35,
        SelectGameMode = 36,
        EnteringFailed = 38, // You should clear the low level first
        HackingTrialDetected = 43,
        CantEnterBecauseKicked = 44,
        CantEnterBecauseVoteKick = 45,
        InternetSlow = 47,
        NetworkCheck = 48,
        CantKickThisPlayer = 49,
        WeaponNotAllowed = 50,
        FailedToCreateRoom = 56
    }

    public enum ChannelInfoRequest : byte
    {
        RoomList = 3,
        RoomList2 = 4,
        ChannelList = 5
    }

    public enum ChangeTeamResult : byte
    {
        Full = 0,
        AlreadyReady = 1
    }
}
