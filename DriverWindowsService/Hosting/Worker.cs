using System.Threading;
using System.Threading.Tasks;
using DriverWindowsService.Transport;
using Microsoft.Extensions.Logging;

namespace DriverWindowsService.Hosting
{
    public sealed class Worker
    {
        private readonly ILogger<Worker> _logger;
        private readonly TcpJsonServer _server;
        private CancellationTokenSource _cts;

        public Worker(ILogger<Worker> logger, TcpJsonServer server)
        {
            _logger = logger;
            _server = server;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => _server.RunAsync(_cts.Token));
            _logger.LogInformation("Worker started");
        }

        public void Stop()
        {
            _cts.Cancel();
            _server.Dispose();
            _logger.LogInformation("Worker stopped");
        }
    }
}