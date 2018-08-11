using System;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    /// <summary>
    /// Serializes <see cref="Version"/> as a byte array of length 4
    /// </summary>
    public class VersionSerializer : ISerializerCompiler
    {
        private const int ArraySize = 4;
        private readonly ArrayWithIntPrefixSerializer _arraySerializer = new ArrayWithIntPrefixSerializer();

        public bool CanHandle(Type type)
        {
            return type == typeof(Version);
        }

        public void EmitSerialize(CompilerContext context, Local value)
        {
            var writeLabel = context.Emit.DefineLabel();

            // if (value != null) goto write
            context.Emit.LoadLocal(value);
            context.Emit.LoadNull();
            context.Emit.CompareEqual();
            context.Emit.BranchIfFalse(writeLabel);

            // value = new Version()
            context.Emit.NewObject<Version>();
            context.Emit.StoreLocal(value);

            context.Emit.MarkLabel(writeLabel);

            // w.Write(new[] { (ushort)Version.Major, (ushort)Version.Minor, (ushort)Version.Build, (ushort)Version.Revision });
            using (var array = context.Emit.DeclareLocal<ushort[]>("array"))
            {
                context.Emit.LoadConstant(ArraySize);
                context.Emit.NewArray<ushort>();
                context.Emit.StoreLocal(array);

                context.Emit.LoadLocal(array);
                context.Emit.LoadConstant(0);
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(Version).GetProperty(nameof(Version.Major)).GetMethod);
                context.Emit.Convert<ushort>();
                context.Emit.StoreElement<ushort>();

                context.Emit.LoadLocal(array);
                context.Emit.LoadConstant(1);
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(Version).GetProperty(nameof(Version.Minor)).GetMethod);
                context.Emit.Convert<ushort>();
                context.Emit.StoreElement<ushort>();

                context.Emit.LoadLocal(array);
                context.Emit.LoadConstant(2);
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(Version).GetProperty(nameof(Version.Build)).GetMethod);
                context.Emit.Convert<ushort>();
                context.Emit.StoreElement<ushort>();

                context.Emit.LoadLocal(array);
                context.Emit.LoadConstant(3);
                context.Emit.LoadLocal(value);
                context.Emit.Call(typeof(Version).GetProperty(nameof(Version.Revision)).GetMethod);
                context.Emit.Convert<ushort>();
                context.Emit.StoreElement<ushort>();

                _arraySerializer.EmitSerialize(context, array);
            }
        }

        public void EmitDeserialize(CompilerContext context, Local value)
        {
            using (var array = context.Emit.DeclareLocal<ushort[]>("array"))
            {
                _arraySerializer.EmitDeserialize(context, array);

                // value = new Version(array[0], array[1], array[2], array[3])
                for (var i = 0; i < ArraySize; i++)
                {
                    context.Emit.LoadLocal(array);
                    context.Emit.LoadConstant(i);
                    context.Emit.LoadElement<ushort>();
                }

                context.Emit.NewObject(typeof(Version).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int) }));
                context.Emit.StoreLocal(value);
            }
        }
    }
}
