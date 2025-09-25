using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DriverWindowsService.Handlers
{
    public sealed class HandlerRegistry
    {
        private readonly ILogger<HandlerRegistry> _logger;
        private readonly IServiceProvider _sp;
        private readonly Dictionary<string, Type> _byType = new(StringComparer.OrdinalIgnoreCase);

        public HandlerRegistry(ILogger<HandlerRegistry> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
            Scan();
        }

        private void Scan()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException rtle) { types = rtle.Types.Where(t => t != null).ToArray(); }

                foreach (var t in types.Where(t => typeof(IPackageHandler).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface))
                {
                    var attr = t.GetCustomAttribute<PackageTypeAttribute>();
                    if (attr == null) continue;
                    _byType[attr.Name] = t;
                    _logger.LogInformation("Registered package handler: {Type} -> {Impl}", attr.Name, t.FullName);
                }
            }
        }

        public IPackageHandler Resolve(string packageType)
        {
            if (_byType.TryGetValue(packageType, out var impl))
                return (IPackageHandler)_sp.GetRequiredService(impl);

            throw new InvalidOperationException($"No handler for package type '{packageType}'.");
        }
    }
}