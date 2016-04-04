using System.IO;
using BlubLib.Network;
using Netsphere.Network.Message.Auth;
using ProudNet.Message;

namespace Netsphere.Network.Message
{
    public interface INetsphereMessageFactory
    {
        ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r);
    }

    public class AuthMessageFactory : INetsphereMessageFactory
    {
        public ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r)
        {
            return AuthMapper.GetMessage((AuthOpCode)opCode, r);
        }
    }
}
