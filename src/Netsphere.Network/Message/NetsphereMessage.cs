using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Message.Auth;
using ProudNet.Message;

namespace Netsphere.Network.Message
{
    public abstract class AuthMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(AuthMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, this);
            }
        }
    }
}
