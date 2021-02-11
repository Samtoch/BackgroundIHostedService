using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundIHostedService.BGService
{
    public abstract class HostedService : IHostedService
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.Info("StartAsync, Happened at: " + DateTime.Now);
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = ExecuteAsync(_cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            log.Info("StopAsync, Happened at: " + DateTime.Now);
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        //protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    //stoppingToken.Register(() =>
        //    //        _logger.LogDebug($" GracePeriod background task is stopping."));

        //    do
        //    {
        //        //await Process();
        //        log.Info("Doing Some Job here, Happened at: " + DateTime.Now);

        //        await Task.Delay(5000, stoppingToken); //5 seconds delay
        //    }
        //    while (!stoppingToken.IsCancellationRequested);
        //}
        // Derived classes should override this and execute a long running method until 
        // cancellation is requested
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
