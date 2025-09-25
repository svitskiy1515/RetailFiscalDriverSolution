using System;
using System.ServiceModel;
using System.Threading;
using Serilog;
using Shared.Contracts;

namespace POS.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/pos-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            
            var binding = new NetTcpBinding(SecurityMode.None);
            var address = new EndpointAddress("net.tcp://localhost:9000/RemoteDriverService");
           // var factory = new ChannelFactory<IRemoteDriverService>(binding, address);
           // var client = factory.CreateChannel();

            //    var cmd = new SaleCommand { Amount = 123.45m };
            //    var posCmd = new PosCommand
            //    {
            //        PosId = "POS01",
            //        Command = cmd
            //    };

            //    var id = client.EnqueueCommand(posCmd);
            //    Console.WriteLine($"Sent command {id}, waiting for result...");

            //    PosCommandResult result = null;
            //    while (result == null || !result.IsCompleted)
            //    {
            //        Thread.Sleep(200);
            //        result = client.GetCommandResult(id);
            //    }

           // Console.WriteLine($"Completed: {result.IsCompleted}, Error={result.Error.Result}, Turnover={client.GetTurnover("POS01")}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
