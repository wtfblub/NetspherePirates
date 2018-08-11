using System;

namespace ProudNet.Hosting.Services
{
    public interface IProudNetServerService
    {
        bool IsRunning { get; }

        bool IsShuttingDown { get; }

        event EventHandler Started;

        event EventHandler Stopping;

        event EventHandler Stopped;

        event EventHandler<ProudSession> Connected;

        event EventHandler<ProudSession> Disconnected;

        event EventHandler<ErrorEventArgs> Error;

        event EventHandler<UnhandledRmiEventArgs> UnhandledRmi;
    }
}
