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
        RoomChangingRules = 7,
        ChannelLeave = 8,
        PlayerNotFound = 9,
        CreateCharacterFailed = 10,
        DeleteCharacterFailed = 11,
        SelectCharacterFailed = 12,
        CreateNicknameSuccess = 13,
        NicknameUnavailable = 14,
        NicknameAvailable = 15,
        PasswordError = 16,
        WelcomeToS4World2 = 17,
        IPLocked = 18, // You can only connect with a specified ip
        ForbiddenToConnectFor5Min = 19,
        UserAlreadyExist = 20,
        DBError = 21,
        CreateCharacterFailed2 = 22,
        JoinChannelFailed = 23,
        RequiredChannelLicense = 24,
        WearingUnusableItem = 25,
        CannotSellWearingItem = 26,
        CantEnterRoom = 28,
        ImpossibleToEnterRoom = 29,
        TaskCompensationError = 31,
        FailedToRequestTask = 32,
        ItemExchangeFailed = 33,
        ItemExchangeFailed2 = 34,
        SelectGameMode = 35,

        //36 -> db error
        //37 -> entering failed you should clear the low level first
        WelcomeToS4World = 39
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
