using System;

namespace ProudNet
{
    /// <summary>
    /// Sets the priority for a message handler. Higher priorities get executed first.
    /// </summary>
    /// <remarks>Priority defaults to 10</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlePriorityAttribute : Attribute
    {
        public byte Priority { get; }

        public HandlePriorityAttribute(byte priority)
        {
            Priority = priority;
        }
    }
}
