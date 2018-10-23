using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProudNet
{
    public class DefaultMessageHandlerResolver : IMessageHandlerResolver
    {
        public Assembly[] Assemblies { get; }
        public Type[] BaseTypes { get; }

        public DefaultMessageHandlerResolver(Assembly[] assemblies, params Type[] baseTypes)
        {
            Assemblies = assemblies ?? Array.Empty<Assembly>();
            BaseTypes = baseTypes ?? Array.Empty<Type>();
        }

        public IEnumerable<Type> GetImplementations()
        {
            return Assemblies.SelectMany(SelectTypes);
        }

        private static IEnumerable<Type> SelectTypes(Assembly assembly)
        {
            return assembly.DefinedTypes
                .Where(x => !x.IsInterface && x.GetCustomAttribute<GeneratedCodeAttribute>() == null)

                // Implements the IHandle<> interface?
                .Where(x => x.ImplementsMessageHandler());
        }
    }
}
