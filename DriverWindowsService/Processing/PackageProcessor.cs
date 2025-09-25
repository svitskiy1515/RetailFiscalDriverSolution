using DriverWindowsService.Handlers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetailFiscalDriver.Shared.Contracts;

namespace DriverWindowsService.Processing
{
    public sealed class PackageProcessor
    {
        private readonly ILogger<PackageProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public PackageProcessor(ILogger<PackageProcessor> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task<Package> ProcessAsync(Package package, CancellationToken ct)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var registry = scope.ServiceProvider.GetRequiredService<HandlerRegistry>();
                var handler = registry.Resolve(package.Type);
                return await handler.HandleAsync(package, ct);
            }
        }
    }
}