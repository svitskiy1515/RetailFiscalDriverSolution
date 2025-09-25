using Shared.Contracts.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Models;

namespace DriverWindowsService.Handlers.Commands
{
    public sealed class GetStatusHandler : ICommandHandler
    {
        private readonly IDriverService _driver;
        public GetStatusHandler(IDriverService driver) => _driver = driver;

        public string CommandName => "GetStatus";
        public bool CanHandle(Command command) => command.Name == CommandName;

        public Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default)
            => _driver.ExecuteAsync(command, ct);
    }
}