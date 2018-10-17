using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProudNet
{
    public class HandleResolverByBaseType<TBaseType> : IHandleResolver
    {
        private readonly Assembly[] _assemblies;

        public HandleResolverByBaseType(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Type> GetImplementations()
        {
            return _assemblies.SelectMany(SelectTypes);
        }

        private static IEnumerable<Type> SelectTypes(Assembly assembly)
        {
            return assembly.DefinedTypes
                .Where(x => !x.IsInterface && x.GetCustomAttribute<GeneratedCodeAttribute>() == null)
                .Where(x => x.IsImplementingHandleInterface() && typeof(TBaseType).IsAssignableFrom(x));
        }
    }
}
