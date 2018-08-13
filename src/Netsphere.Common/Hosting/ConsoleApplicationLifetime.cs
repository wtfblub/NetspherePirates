using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Netsphere.Common.Hosting
{
    public class ConsoleApplicationLifetime : IHostLifetime
    {
        public void RegisterDelayStartCallback(Action<object> callback, object state)
        {
            callback(state);
        }

        public void RegisterStopCallback(Action<object> callback, object state)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => callback(state);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                callback(state);
            };
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
