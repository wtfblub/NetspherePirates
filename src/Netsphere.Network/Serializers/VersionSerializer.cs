using System;
using BlubLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Netsphere.Network.Serializers
{
    internal class VersionSerializer : ISerializerCompiler
    {
        private readonly ArrayWithIntPrefixSerializer _arraySerializer = new ArrayWithIntPrefixSerializer();

        public bool CanHandle(Type type)
        {
            throw new NotImplementedException();
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            //w.Write(new[] { (ushort)Version.Major, (ushort)Version.Minor, (ushort)Version.Build, (ushort)Version.Revision });

            using (var array = emiter.DeclareLocal<ushort[]>("array"))
            {
                emiter.LoadConstant(4);
                emiter.NewArray<ushort>();
                emiter.StoreLocal(array);

                emiter.LoadLocal(array);
                emiter.LoadConstant(0);
                emiter.LoadLocal(value);
                emiter.Call(typeof (Version).GetProperty(nameof(Version.Major)).GetMethod);
                emiter.Convert<ushort>();
                emiter.StoreElement<ushort>();

                emiter.LoadLocal(array);
                emiter.LoadConstant(1);
                emiter.LoadLocal(value);
                emiter.Call(typeof(Version).GetProperty(nameof(Version.Minor)).GetMethod);
                emiter.Convert<ushort>();
                emiter.StoreElement<ushort>();

                emiter.LoadLocal(array);
                emiter.LoadConstant(2);
                emiter.LoadLocal(value);
                emiter.Call(typeof(Version).GetProperty(nameof(Version.Build)).GetMethod);
                emiter.Convert<ushort>();
                emiter.StoreElement<ushort>();

                emiter.LoadLocal(array);
                emiter.LoadConstant(3);
                emiter.LoadLocal(value);
                emiter.Call(typeof(Version).GetProperty(nameof(Version.Revision)).GetMethod);
                emiter.Convert<ushort>();
                emiter.StoreElement<ushort>();

                _arraySerializer.EmitSerialize(emiter, array);
            }
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            var ok = emiter.DefineLabel(nameof(VersionSerializer) + "OK" + Guid.NewGuid());

            using (var array = emiter.DeclareLocal<ushort[]>("array"))
            {
                _arraySerializer.EmitDeserialize(emiter, array);

                // if(array.Length != 4) throw new Exception("Invalid count for version")
                emiter.LoadLocal(array);
                emiter.Call(typeof(Array).GetProperty(nameof(Array.Length)).GetMethod);
                emiter.LoadConstant(4);
                emiter.BranchIfEqual(ok);

                emiter.LoadConstant("Invalid count for version");
                emiter.NewObject(typeof(Exception).GetConstructor(new[] { typeof(string) }));
                emiter.Throw();

                emiter.MarkLabel(ok);

                // value = new Version(array[0], array[1], array[2], array[3])
                for (var i = 0; i < 4; i++)
                {
                    emiter.LoadLocal(array);
                    emiter.LoadConstant(i);
                    emiter.LoadElement<ushort>();
                }
                emiter.NewObject(typeof(Version).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int) }));
                emiter.StoreLocal(value);
            }
        }
    }
}
