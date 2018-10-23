using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProudNet
{
    /// <summary>
    /// Finds <see cref="IHandle{TMessage}"/> implementations
    /// </summary>
    public interface IMessageHandlerResolver
    {
        Assembly[] Assemblies { get; }
        Type[] BaseTypes { get; }

        IEnumerable<Type> GetImplementations();
    }
}
