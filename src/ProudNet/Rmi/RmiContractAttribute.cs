using System;

namespace ProudNet.Rmi
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class RmiContractAttribute : Attribute
    { }
}
