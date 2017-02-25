﻿using System;
using System.IO;
using BlubLib.Reflection;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNet.Serialization.Serializers
{
    public class StringSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadProudString()));
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            var write = emiter.DefineLabel(nameof(StringSerializer) + "Write" + Guid.NewGuid());

            // if (value != null) goto write
            emiter.LoadLocal(value);
            emiter.LoadNull();
            emiter.CompareEqual();
            emiter.BranchIfFalse(write);

            // value = string.Empty
            emiter.LoadField(typeof(string).GetField(nameof(string.Empty)));
            emiter.StoreLocal(value);

            // ProudNetBinaryWriterExtensions.WriteProudString(writer, value, false)
            emiter.MarkLabel(write);
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.LoadConstant(false);
            emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.WriteProudString(default(string), default(bool))));
        }
    }
}
