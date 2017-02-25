﻿using System;
using System.Collections.Generic;
using System.IO;
using BlubLib;
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
            ushort opCode;
            if (_opCodeLookup.TryGetValue(type, out opCode))
                return opCode;

            throw new ProudException($"No opcode found for type {type.FullName}");
        }

        public object GetMessage(ushort opCode, Stream stream)
        {
            Type type;
            if (!_typeLookup.TryGetValue(opCode, out type))
                throw new ProudException($"No type found for opcode {opCode}");

            return Serializer.Deserialize(stream, type);
        }

        public object GetMessage(ushort opCode, BinaryReader reader)
        {
            Type type;
            if (!_typeLookup.TryGetValue(opCode, out type))
#if DEBUG
                throw new ProudBadOpCodeException(opCode, reader.ReadToEnd());
#else
            throw new ProudBadOpCodeException(opCode);
#endif

            return Serializer.Deserialize(reader, type);
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

        public TMessage GetMessage(TOpCode opCode, Stream stream)
        {
            return (TMessage)GetMessage(DynamicCast<ushort>.From(opCode), stream);
        }

        public TMessage GetMessage(TOpCode opCode, BinaryReader reader)
        {
            return (TMessage)GetMessage(DynamicCast<ushort>.From(opCode), reader);
        }

        public bool ContainsOpCode(TOpCode opCode)
        {
            return ContainsOpCode(DynamicCast<ushort>.From(opCode));
        }
    }
}
