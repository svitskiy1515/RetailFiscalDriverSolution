using DriverWindowsService.Hosting;
using DriverWindowsService.Persistence;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
// из Shared.Contracts
using DriverWindowsService.Processing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Models;

namespace DriverWindowsService.Transport
{
    public sealed class TcpJsonServer : IDisposable
    {
        private readonly ILogger<TcpJsonServer> _logger;
        private readonly PackageProcessor _processor;
        private readonly int _port;
        private TcpListener _listener;
        private readonly IServiceProvider _serviceProvider;

        public TcpJsonServer(IServiceProvider serviceProvider, ILogger<TcpJsonServer> logger, PackageProcessor processor, int port = 5055)
        {
            _logger = logger;
            _processor = processor;
            _port = port;
            _serviceProvider=serviceProvider;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            _logger.LogInformation("TCP server listening on 127.0.0.1:{Port}", _port);

            while (!ct.IsCancellationRequested)
            {
                if (!_listener.Pending())
                {
                    await Task.Delay(50, ct);
                    continue;
                }

                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client, ct), ct);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
        {
            using (client)
            using (var ns = client.GetStream())
            using (var sr = new StreamReader(ns))
            using (var sw = new StreamWriter(ns) { AutoFlush = true })
            {
                var json = string.Empty;
                try
                {
                  
                     json  = await sr.ReadLineAsync();
                    _logger.LogDebug("Request: {Json}", json);
                    var (ok, err) = PackageValidator.Validate(json);
                    if (!ok) {
                        await sw.WriteLineAsync(JsonConvert.SerializeObject(
                            CommandResponse.Fail($"Invalid package: {err}")
                        ));
                        return;
                    }
                    var package = JsonConvert.DeserializeObject<Package>(json);
                    var idStore = _serviceProvider.GetRequiredService<IIdempotencyStore>(); // создавай scope здесь или используй фабрику
                    var (done, cached) = await idStore.TryGetAsync(package.PackageId, ct);
                    if (done)
                    {
                        await sw.WriteLineAsync(cached);
                        return;
                    }

                    var result = await _processor.ProcessAsync(package, ct);

                    var resp = JsonConvert.SerializeObject(result);
                    await sw.WriteLineAsync(resp);
                    _logger.LogDebug("Response: {Resp}", resp);
                }
                catch (Exception ex)
                {
                   
                        _logger.LogError(ex, "Processing failed, spooling");
                        var spooler = _serviceProvider.GetRequiredService<SpoolWorker>();
                        spooler.EnqueueRaw(json);
                        await sw.WriteLineAsync(JsonConvert.SerializeObject(
                            CommandResponse.Fail("Временная недоступность. Заявка поставлена в очередь.",
                                "QUEUED", "Повтор не требуется — обработаем автоматически")));
                   
                    
                    //_logger.LogError(ex, "TCP handler error");
                    //await sw.WriteLineAsync(JsonConvert.SerializeObject(new
                    //{
                    //    Success = false,
                    //    Error = ex.Message
                    //}));

                }
            }
        }

        public void Dispose() => _listener?.Stop();
    }
}
