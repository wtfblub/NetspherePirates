using System;
using ProudNet.Hosting.Services;

namespace ProudNet
{
    /// <summary>
    /// Instructs the message handler to execute on the sessions IO thread
    /// by default message handlers get executed on the <see cref="ISchedulerService"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InlineAttribute : Attribute
    {
    }
}
