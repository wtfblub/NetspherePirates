using System;
using System.Threading;
using System.Threading.Tasks;
using ProudNet.Configuration;

namespace ProudNet.Hosting.Services
{
    /// <summary>
    /// Scheduler service which can be used to execute tasks on the <see cref="ThreadingOptions.WorkerThreadFactory"/>
    /// </summary>
    public interface ISchedulerService
    {
        /// <summary>
        /// Executes the given action
        /// </summary>
        /// <param name="action">The action to execute</param>
        void Execute(Action action);

        /// <summary>
        /// Executes the given action
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="context">A context which gets passed to the action</param>
        /// <param name="state">A state which gets passed to the action</param>
        void Execute(Action<object, object> action, object context, object state);

        /// <summary>
        /// Schedules the given action for execution after the specified delay would pass
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="delay">The time to wait before executing</param>
        Task ScheduleAsync(Action action, TimeSpan delay);

        /// <summary>
        /// Schedules the given action for execution after the specified delay would pass
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="context">A context which gets passed to the action</param>
        /// <param name="state">A state which gets passed to the action</param>
        /// <param name="delay">The time to wait before executing</param>
        Task ScheduleAsync(Action<object, object> action, object context, object state, TimeSpan delay);

        /// <summary>
        /// Schedules the given action for execution after the specified delay would pass
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="context">A context which gets passed to the action</param>
        /// <param name="state">A state which gets passed to the action</param>
        /// <param name="delay">The time to wait before executing</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can abort the scheduled action</param>
        Task ScheduleAsync(Action<object, object> action, object context, object state,
            TimeSpan delay, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        Task<T> SubmitAsync<T>(Func<T> func);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        Task<T> SubmitAsync<T>(Func<T> func, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        /// <param name="func">The action to execute</param>
        /// <param name="state">A state which gets passed to the action</param>
        Task<T> SubmitAsync<T>(Func<object, T> func, object state);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        /// <param name="func">The action to execute</param>
        /// <param name="state">A state which gets passed to the action</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can abort the scheduled action</param>
        Task<T> SubmitAsync<T>(Func<object, T> func, object state, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        /// <param name="func">The action to execute</param>
        /// <param name="context">A context which gets passed to the action</param>
        /// <param name="state">A state which gets passed to the action</param>
        Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state);

        /// <summary>
        /// Executes the given action and returns <see cref="T:System.Threading.Tasks.Task`1" /> indicating completion status and result of execution
        /// </summary>
        /// <param name="func">The action to execute</param>
        /// <param name="context">A context which gets passed to the action</param>
        /// <param name="state">A state which gets passed to the action</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can abort the scheduled action</param>
        Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state, CancellationToken cancellationToken);
    }
}
