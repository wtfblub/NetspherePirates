using System;
using System.Collections.Generic;

namespace ProudNet
{
    /// <summary>
    /// Finds <see cref="IHandle{TMessage}"/> implementations
    /// </summary>
    public interface IHandleResolver
    {
        IEnumerable<Type> GetImplementations();
    }
}
