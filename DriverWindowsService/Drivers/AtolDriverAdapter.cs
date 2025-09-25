using Microsoft.Extensions.Logging;
using Shared.Contracts.Abstractions;
using Shared.Contracts.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Drivers
{
    public sealed class AtolDriverAdapter : IDriverService
    {
        private readonly ILogger<AtolDriverAdapter> _logger;
        public AtolDriverAdapter(ILogger<AtolDriverAdapter> logger) => _logger = logger;

        public Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default)
            => Task.FromResult(CommandResponse.Fail($"Not implemented for ATOL: {command.Name}"));
    }
}