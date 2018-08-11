using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;

namespace ProudNet.Hosting.Services
{
    internal class SchedulerService : ISchedulerService, IHostedService
    {
        private readonly ThreadingOptions _options;
        private IEventLoop _workerThread;

        public SchedulerService(IOptions<ThreadingOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_workerThread != null)
                throw new InvalidOperationException("Scheduler is already running");

            _workerThread = _options.WorkerThreadFactory?.Invoke() ?? new SingleThreadEventLoop();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_workerThread == null)
                return;

            await _workerThread.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(2));
            _workerThread = null;
        }

        public void Execute(Action action)
        {
            ThrowIfNotRunning();
            _workerThread.Execute(action);
        }

        public void Execute(Action<object, object> action, object context, object state)
        {
            ThrowIfNotRunning();
            _workerThread.Execute(action, context, state);
        }

        public Task ScheduleAsync(Action action, TimeSpan delay)
        {
            ThrowIfNotRunning();
            return _workerThread.ScheduleAsync(action, delay);
        }

        public Task ScheduleAsync(Action<object, object> action, object context, object state, TimeSpan delay)
        {
            ThrowIfNotRunning();
            return _workerThread.ScheduleAsync(action, context, state, delay);
        }

        public Task ScheduleAsync(Action<object, object> action, object context, object state, TimeSpan delay,
            CancellationToken cancellationToken)
        {
            ThrowIfNotRunning();
            return _workerThread.ScheduleAsync(action, context, state, delay, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<T> func)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func);
        }

        public Task<T> SubmitAsync<T>(Func<T> func, CancellationToken cancellationToken)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<object, T> func, object state)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func, state);
        }

        public Task<T> SubmitAsync<T>(Func<object, T> func, object state, CancellationToken cancellationToken)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func, state, cancellationToken);
        }

        public Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func, context, state);
        }

        public Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state,
            CancellationToken cancellationToken)
        {
            ThrowIfNotRunning();
            return _workerThread.SubmitAsync(func, context, state, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfNotRunning()
        {
            if (_workerThread == null)
                throw new InvalidOperationException("Scheduler is not running");
        }
    }
}
