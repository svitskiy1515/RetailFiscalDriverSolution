using Newtonsoft.Json.Linq;
using Shared.Contracts.Abstractions;
using Shared.Contracts.Models;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Handlers.Commands
{
    public sealed class SelfCheckHandler : ICommandHandler
    {
        private readonly IDriverService _driver;
        public SelfCheckHandler(IDriverService driver) => _driver = driver;
        public string CommandName => "SelfCheck";
        public bool CanHandle(Command c) => c.Name == CommandName;

        public async Task<CommandResponse> ExecuteAsync(Command c, CancellationToken ct = default)
        {
            var data = new JObject();
            // DB
            try {
                using var cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["FiscalDb"].ConnectionString);
                await cn.OpenAsync();
                data["db"] = "ok";
            } catch (Exception ex) { data["db"] = "fail: " + ex.Message; }

            // Driver ping (опционально)
            var ping = await _driver.ExecuteAsync(new Command{ Name="GetStatus", Args=new JObject() }, ct);
            data["driver"] = ping.Success ? "ok" : $"fail: {ping.Error}";

            // Права на каталог spool/logs
            try { File.AppendAllText("logs\\_w.txt", "."); data["fsLogs"] = "ok"; } catch (Exception ex) { data["fsLogs"] = "fail: " + ex.Message; }
            return CommandResponse.Ok(data);
        }
    }
}