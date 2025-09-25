using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RetailFiscalDriver.Shared.Contracts; // из Shared.Contracts
using DriverWindowsService.Processing;

namespace DriverWindowsService.Transport
{
    public sealed class TcpJsonServer : IDisposable
    {
        private readonly ILogger<TcpJsonServer> _logger;
        private readonly PackageProcessor _processor;
        private readonly int _port;
        private TcpListener _listener;

        public TcpJsonServer(ILogger<TcpJsonServer> logger, PackageProcessor processor, int port = 5055)
        {
            _logger = logger;
            _processor = processor;
            _port = port;
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
                try
                {
                    var json = await sr.ReadLineAsync();
                    _logger.LogDebug("Request: {Json}", json);
                    var package = JsonConvert.DeserializeObject<Package>(json);

                    var result = await _processor.ProcessAsync(package, ct);

                    var resp = JsonConvert.SerializeObject(result);
                    await sw.WriteLineAsync(resp);
                    _logger.LogDebug("Response: {Resp}", resp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TCP handler error");
                    await sw.WriteLineAsync(JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Error = ex.Message
                    }));
                }
            }
        }

        public void Dispose() => _listener?.Stop();
    }
}
