using System;
using System.Collections.Generic;
using IdGen;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;

namespace Netsphere.Common
{
    public class IdGeneratorService : IService
    {
        private readonly IDictionary<IdKind, IIdGenerator<long>> _generators;

        public IdGeneratorService(IOptions<IdGeneratorOptions> options)
        {
            if (options.Value.Id > 31 || options.Value.Id < 0)
                throw new ArgumentOutOfRangeException(nameof(options), "serviceId must be between 0 and 31");

            _generators = new Dictionary<IdKind, IIdGenerator<long>>();
            foreach (IdKind kind in Enum.GetValues(typeof(IdKind)))
                _generators[kind] = new IdGenerator((byte)kind << 5 | options.Value.Id, new MaskConfig(41, 10, 12));
        }

        public long GetNextId(IdKind kind)
        {
            return _generators[kind].CreateId();
        }
    }

    public enum IdKind
    {
        Item,
        Character,
        License,
        Deny,
        Setting,
        Mail
    }
}
