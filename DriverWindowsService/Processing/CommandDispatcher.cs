using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Abstractions;
using Shared.Contracts.Models;

namespace DriverWindowsService.Processing
{
    public sealed class CommandDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _sp;
        private readonly Dictionary<string, Type> _handlers = new(StringComparer.OrdinalIgnoreCase);

        public CommandDispatcher(ILogger<CommandDispatcher> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
            Scan();
        }

        private void Scan()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

                foreach (var t in types.Where(t => typeof(ICommandHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
                {
                    var inst = (ICommandHandler)Activator.CreateInstance(t);
                    _handlers[inst.CommandName] = t;
                    _logger.LogInformation("Registered command handler: {Name} -> {Type}", inst.CommandName, t.FullName);
                }
            }
        }

        public async Task<CommandResponse> DispatchAsync(Command command, CancellationToken ct = default)
        {
            if (!_handlers.TryGetValue(command.Name, out var t))
                return CommandResponse.Fail($"No handler for command '{command.Name}'");

            var handler = (ICommandHandler)ActivatorUtilities.CreateInstance(_sp, t);
            if (!handler.CanHandle(command))
                return CommandResponse.Fail($"Handler '{t.Name}' cannot handle '{command.Name}'");

            return await handler.ExecuteAsync(command, ct);
        }
    }
}
