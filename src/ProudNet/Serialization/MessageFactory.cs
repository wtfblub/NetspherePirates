using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace ProudNet.Serialization
{
    public class MessageFactory<TOpCode>
    {
        private readonly Dictionary<TOpCode, Type> _typeLookup = new Dictionary<TOpCode, Type>();
        private readonly Dictionary<Type, TOpCode> _opCodeLookup = new Dictionary<Type, TOpCode>();

        protected void Register<T>(TOpCode opCode)
            where T : new()
        {
            var type = typeof(T);
            _opCodeLookup.Add(type, opCode);
            _typeLookup.Add(opCode, type);
        }

        public TOpCode GetOpCode(Type type)
        {
            TOpCode opCode;
            if (_opCodeLookup.TryGetValue(type, out opCode))
                return opCode;

            throw new ProudException($"No opCode found for type {type.FullName}");
        }

        public object GetMessage(TOpCode opCode, Stream stream)
        {
            Type type;
            if (!_typeLookup.TryGetValue(opCode, out type))
                throw new ProudException($"No type found for opCode {opCode}");

            return Serializer.Deserialize(stream, type);
        }
    }
}
