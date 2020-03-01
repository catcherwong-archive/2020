namespace DelayDemo
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class SubscribeTaskBgTask : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ITaskServices _taskServices;

        public SubscribeTaskBgTask(ILoggerFactory loggerFactory, ITaskServices taskServices)
        {
            this._logger = loggerFactory.CreateLogger<SubscribeTaskBgTask>();
            this._taskServices = taskServices;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _taskServices.SubscribeToDo("task:");

            return Task.CompletedTask;
        }
    }
}
