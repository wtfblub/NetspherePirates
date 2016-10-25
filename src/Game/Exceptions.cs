using System;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    public class CharacterException : Exception
    {
        public CharacterException(string message)
            : base(message)
        { }
    }

    public class ChannelException : Exception
    {
        public ChannelException()
        { }
        public ChannelException(string message)
            : base(message)
        { }
    }

    public class ChannelLimitReachedException : ChannelException
    { }

    public class LicenseException : Exception
    {
        public LicenseException(string message)
            : base(message)
        { }
    }

    public class LicenseNotFoundException : LicenseException
    {
        public LicenseNotFoundException(string message)
            : base(message)
        { }
    }

    public class InventoryException : Exception
    {
        public InventoryException(string message)
            : base(message)
        { }
    }

    public class RoomException : Exception
    {
        public RoomException()
        { }
        public RoomException(string message)
            : base(message)
        { }
    }

    public class RoomLimitReachedException : RoomException
    { }

    public class RoomAccessDeniedException : RoomException
    { }

    public class TeamLimitReachedException : RoomException
    { }
}
