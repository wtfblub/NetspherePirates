using BlubLib.Network;

namespace ProudNet
{
    public class ProudSessionFactory : ISessionFactory
    {
        public ISession GetSession(IIOService service, IIOProcessor processor)
        {
            return new ProudSession(service, processor);
        }
    }
}
