using System;
using DriverWindowsService;
using Serilog;
using Topshelf;

class Program
{
    static int Main()
    {
        // Serilog конфигурация
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/driver-.log", rollingInterval: RollingInterval.Day)
            .WriteTo.EventLog("PilotFiscalDriverService", manageEventSource: true, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .CreateLogger();

        try
        {
            Log.Information("Host starting (Topshelf)");

            var rc = HostFactory.Run(x =>
            {
                x.Service<DriverServiceHost>(s =>
                {
                    s.ConstructUsing(name => new DriverServiceHost(comPort: 1, baudRate: 115200));
                    s.WhenStarted(instance => instance.Start());
                    s.WhenStopped(instance => instance.Stop());
                });

                x.RunAsLocalSystem(); // или задать учётную запись
                x.SetServiceName("PilotFiscalDriverService");
                x.SetDisplayName("Pilot Fiscal Driver Service");
                x.SetDescription("Service that hosts fiscal driver and WCF endpoints");
                
                // Optional: allow service to be run as console for debugging:
                x.StartAutomatically(); // auto start when installed
            });

            Log.Information("Host finished with rc = {rc}", rc);
            return (int)Convert.ChangeType(rc, rc.GetTypeCode());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return -1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}