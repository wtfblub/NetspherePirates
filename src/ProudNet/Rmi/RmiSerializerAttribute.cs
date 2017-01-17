using System;

namespace ProudNet.Rmi
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue,
        AllowMultiple = false, Inherited = false)]
    public class RmiSerializerAttribute : Attribute
    {
        public Type SerializerType { get; set; }
        public object[] SerializerParameters { get; set; }

        public RmiSerializerAttribute(Type serializerType, params object[] serializerParameters)
        {
            SerializerType = serializerType;
            SerializerParameters = serializerParameters;
        }
    }
}
