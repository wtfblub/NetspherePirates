using System;
using System.Collections.Generic;
using System.IO;
using BlubLib;
using BlubLib.IO;
using BlubLib.Serialization;

namespace ProudNet.Serialization
{
    public class MessageFactory
    {
        private readonly Dictionary<ushort, Type> _typeLookup = new Dictionary<ushort, Type>();
        private readonly Dictionary<Type, ushort> _opCodeLookup = new Dictionary<Type, ushort>();

        protected void Register<T>(ushort opCode)
            where T : new()
        {
            var type = typeof(T);
            _opCodeLookup.Add(type, opCode);
            _typeLookup.Add(opCode, type);
        }

        public ushort GetOpCode(Type type)
        {
            if (_opCodeLookup.TryGetValue(type, out var opCode))
                return opCode;

            throw new ProudException($"No opcode found for type {type.FullName}");
        }

        public object GetMessage(BlubSerializer serializer, ushort opCode, Stream stream)
        {
            if (!_typeLookup.TryGetValue(opCode, out var type))
                throw new ProudException($"No type found for opcode {opCode}");

            return serializer.Deserialize(stream, type);
        }

        public object GetMessage(BlubSerializer serializer, ushort opCode, BinaryReader reader)
        {
            if (!_typeLookup.TryGetValue(opCode, out var type))
#if DEBUG
                throw new ProudBadOpCodeException(opCode, reader.ReadToEnd());
#else
            throw new ProudBadOpCodeException(opCode);
#endif

            return serializer.Deserialize(reader, type);
        }

        public bool ContainsType(Type type)
        {
            return _opCodeLookup.ContainsKey(type);
        }

        public bool ContainsOpCode(ushort opCode)
        {
            return _typeLookup.ContainsKey(opCode);
        }
    }

    public class MessageFactory<TOpCode, TMessage> : MessageFactory
    {
        protected void Register<T>(TOpCode opCode)
            where T : TMessage, new()
        {
            Register<T>(DynamicCast<ushort>.From(opCode));
        }

        public new TOpCode GetOpCode(Type type)
        {
            return DynamicCast<TOpCode>.From(base.GetOpCode(type));
        }

        public TMessage GetMessage(BlubSerializer serializer, TOpCode opCode, Stream stream)
        {
            return (TMessage)GetMessage(serializer, DynamicCast<ushort>.From(opCode), stream);
        }

        public TMessage GetMessage(BlubSerializer serializer, TOpCode opCode, BinaryReader reader)
        {
            return (TMessage)GetMessage(serializer, DynamicCast<ushort>.From(opCode), reader);
        }

        public bool ContainsOpCode(TOpCode opCode)
        {
            return ContainsOpCode(DynamicCast<ushort>.From(opCode));
        }
    }
}
