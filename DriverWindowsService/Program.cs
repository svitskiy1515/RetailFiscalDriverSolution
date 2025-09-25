using System;
using System.ServiceProcess;

namespace DriverWindowsService
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var console = Environment.UserInteractive || Array.Exists(args, a => a.Equals("--console", StringComparison.OrdinalIgnoreCase));
            if (console)
            {
                var host = new Hosting.ServiceHost();
                host.StartConsole();
                Console.WriteLine("Press Enter to stop...");
                Console.ReadLine();
                host.StopConsole();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new Hosting.ServiceHost() });
            }
        }
    }
}