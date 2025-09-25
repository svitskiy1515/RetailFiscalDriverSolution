using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shared.Contracts.Abstractions;
using Shared.Contracts.Models;

namespace DriverWindowsService.Handlers.Commands
{
    public sealed class PingHandler : ICommandHandler
    {
        public string CommandName => "Ping";
        public bool CanHandle(Command command) => command.Name == CommandName;

        public Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default)
        {
            var data = new JObject
            {
                ["pong"] = true,
                ["serverTimeUtc"] = DateTime.UtcNow,
                ["version"] = typeof(PingHandler).Assembly.GetName().Version?.ToString()
            };
            return Task.FromResult(CommandResponse.Ok(data));
        }
    }
}