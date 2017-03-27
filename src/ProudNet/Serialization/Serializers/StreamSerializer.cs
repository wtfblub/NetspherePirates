//using System;
//using System.IO;
//using BlubLib.IO;
//using BlubLib.Reflection;
//using BlubLib.Serialization;
//using Sigil;
//using Sigil.NonGeneric;

//namespace ProudNet.Serialization.Serializers
//{
//    public class StreamSerializer : ISerializerCompiler
//    {
//        public bool CanHandle(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public void EmitSerialize(Emit emiter, Local value)
//        {
//            // value.CopyTo(writer.BaseStream)
//            emiter
//                .LoadLocal(value)
//                .LoadArgument(1)
//                .CallVirtual(typeof(BinaryWriter).GetProperty(nameof(BinaryWriter.BaseStream)).GetMethod)
//                .Call(ReflectionHelper.GetMethod((Stream x) => x.CopyTo(default(Stream))));
//        }

//        public void EmitDeserialize(Emit emiter, Local value)
//        {
//            // value = new LimitedStream(reader.BaseStream)
//            emiter
//                .LoadArgument(1)
//                .CallVirtual(typeof(BinaryReader).GetProperty(nameof(BinaryReader.BaseStream)).GetMethod)
//                .NewObject(typeof(LimitedStream), typeof(Stream))
//                .StoreLocal(value);
//        }
//    }
//}
