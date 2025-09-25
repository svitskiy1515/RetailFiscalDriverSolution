using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using DriverWindowsService.Hosting;
using DriverWindowsService.Transport;
using DriverWindowsService.Processing;
using DriverWindowsService.Drivers;
using DriverWindowsService.Handlers;
using DriverWindowsService.Persistence;
using RetailFiscalDriver.Shared.Contracts;
using System;
using System.Configuration;
namespace DriverWindowsService.Composition
{
    public static class CompositionRoot
    {
        public static ServiceProvider BuildServiceProvider()
        {
            // Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\service.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            var services = new ServiceCollection();

            // Logging
            services.AddSingleton<ILoggerFactory>(sp => new SerilogLoggerFactory());
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            // DbContext: создаём scoped
            services.AddScoped<FiscalDbContext>();
            services.AddScoped<IFiscalStore, EfFiscalStore>();
            services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();

            // Processing: HandlerRegistry и PackageProcessor делаем SCOPED
            services.AddScoped<HandlerRegistry>();
            services.AddScoped<PackageProcessor>();
          
            services.AddScoped<NonFiscalReceiptHandler>();
            services.AddScoped<FiscalReceiptHandler>();
            
            // Hosting
            services.AddSingleton<Worker>();

            // Transport & processing
            services.AddSingleton<TcpJsonServer>();
            services.AddScoped<HandlerRegistry>();
            services.AddScoped<PackageProcessor>();

            // Drivers (по профилю можно выбрать другой адаптер)
           // ...
            var profile = ConfigurationManager.AppSettings["DriverProfile"] ?? "Pilot";
            services.AddScoped<IDriverService>(sp =>
            {
                switch (profile.ToLowerInvariant())
                {
                    case "pilot":
                        return ActivatorUtilities.CreateInstance<DriverWindowsService.Drivers.PilotDriverAdapter>(sp);
                    case "atol":
                        return ActivatorUtilities.CreateInstance<DriverWindowsService.Drivers.AtolDriverAdapter>(sp);
                    default:
                        throw new InvalidOperationException($"Unknown DriverProfile '{profile}'");
                }
            });

            
            services.AddScoped<IDriverService, PilotDriverAdapter>();
            
            return services.BuildServiceProvider();
        }
    }
}