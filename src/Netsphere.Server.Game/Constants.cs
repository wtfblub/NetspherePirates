namespace Netsphere.Server.Game
{
    public enum CharacterCreateResult
    {
        Success,
        LimitReached,
        InvalidGender,
        InvalidSlot,
        InvalidDefaultItem,
        SlotInUse
    }

    public enum CharacterInventoryError
    {
        OK,
        InvalidSlot,
        SlotAlreadyInUse,
        ItemNotAllowed,
        ItemAlreadyInUse
    }

    public enum ChannelJoinError
    {
        OK,
        AlreadyInChannel,
        ChannelFull
    }
}
