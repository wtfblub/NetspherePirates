using System;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.Serialization;
using Microsoft.Extensions.Hosting;
using ProudNet.Configuration;
using ProudNet.Serialization;

namespace ProudNet.Hosting
{
    public interface IProudNetServerBuilder
    {
        IProudNetServerBuilder UseHostIdFactory(IHostIdFactory hostIdFactory);

        IProudNetServerBuilder UseHostIdFactory<THostIdFactory>()
            where THostIdFactory : class, IHostIdFactory;

        IProudNetServerBuilder UseSessionFactory(ISessionFactory sessionFactory);

        IProudNetServerBuilder UseSessionFactory<TSessionFactory>()
            where TSessionFactory : class, ISessionFactory;

        IProudNetServerBuilder ConfigureSerializer(Action<BlubSerializer> configure);

        IProudNetServerBuilder UseNetworkConfiguration(Action<HostBuilderContext, NetworkOptions> configure);

        IProudNetServerBuilder UseThreadingConfiguration(Action<HostBuilderContext, ThreadingOptions> configure);

        IProudNetServerBuilder AddMessageFactory<TMessageFactory>()
            where TMessageFactory : MessageFactory;

        IProudNetServerBuilder AddMessageHandler<TMessageHandler>()
            where TMessageHandler : class, IMessageHandler;
    }
}
