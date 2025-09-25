using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Abstractions;
using Shared.Contracts.Enums;
using Shared.Contracts.Models;

namespace DriverWindowsService.Handlers
{
    [PackageType("NonFiscal")]
    public sealed class NonFiscalReceiptHandler : IPackageHandler
    {
        private readonly ILogger<NonFiscalReceiptHandler> _logger;
        private readonly IDriverService _driver;

        public NonFiscalReceiptHandler(ILogger<NonFiscalReceiptHandler> logger, IDriverService driver)
        {
            _logger = logger;
            _driver = driver;
        }

        public string PackageType => "NonFiscal";
        public bool CanHandle(string packageType) => packageType.Equals(PackageType, System.StringComparison.OrdinalIgnoreCase);

        public async Task<Package> HandleAsync(Package package, CancellationToken ct = default)
        {
            _logger.LogInformation("Handle NonFiscal {Id}", package.PackageId);

            foreach (var cmd in package.Commands)
            {
                var r = await _driver.ExecuteAsync(cmd, ct);
                if (!r.Success)
                {
                    package.Status = PackageStatus.Failed;
                    _logger.LogWarning("Command {Cmd} failed: {Err}", cmd.Name, r.Error);
                    return package;
                }
            }

            package.Status = PackageStatus.Done;
            package.ProcessedAt = System.DateTime.UtcNow;
            return package;
        }
    }
}