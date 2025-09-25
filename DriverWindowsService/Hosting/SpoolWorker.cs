using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Contracts.Enums;
using Shared.Contracts.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Hosting
{
    public sealed class SpoolWorker
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly FileSpool _spool;
        private readonly ILogger<SpoolWorker> _logger;
        private CancellationTokenSource _cts;

        public SpoolWorker(IServiceScopeFactory scopes, ILogger<SpoolWorker> logger)
        {
            _scopes = scopes; _logger = logger;
            _spool = new FileSpool(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spool"));
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => Loop(_cts.Token));
        }
        public void Stop() => _cts.Cancel();

        private async Task Loop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var file in _spool.DequeueBatch())
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file, ct);
                        var pkg = JsonConvert.DeserializeObject<Package>(json);
                        using var scope = _scopes.CreateScope();
                        var proc = scope.ServiceProvider.GetRequiredService<DriverWindowsService.Processing.PackageProcessor>();
                        var result = await proc.ProcessAsync(pkg, ct);
                        if (result.Status == PackageStatus.Done) File.Delete(file);
                    }
                    catch (Exception ex) { _logger.LogWarning(ex, "Spool process failed"); }
                }
                await Task.Delay(1000, ct);
            }
        }

        public string EnqueueRaw(string json) => _spool.Enqueue(json);
    }
}