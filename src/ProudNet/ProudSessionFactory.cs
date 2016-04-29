using BlubLib.Network;

namespace ProudNet
{
    public class ProudSessionFactory : ISessionFactory
    {
        public ISession GetSession(IService service, ITransport processor)
        {
            return new ProudSession(service, processor);
        }
    }
}
