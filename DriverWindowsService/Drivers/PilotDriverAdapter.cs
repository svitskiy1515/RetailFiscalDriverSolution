using Fw21;
using Fw21.Ecr;
using Fw21.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RetailFiscalDriver.Shared.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Drivers
{
    public sealed class PilotDriverAdapter : IDriverService
    {
        private readonly ILogger<PilotDriverAdapter> _logger;
        private readonly object _sync = new();      // драйвер синхронный — защищаемся
        private EcrCtrl _ecrCtrl;               // инициализируй по своему

        public PilotDriverAdapter(ILogger<PilotDriverAdapter> logger)
        {
            _logger = logger;
            // _service = new EcrService(...); // TODO: создать/подключить
        }

        public Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default)
        {
            try
            {
                lock (_sync)
                {
                    switch (command.Name)
                    {
                        case "BeginReceipt":
                            // var op = new EcrOperator(... из конфига/Args ...);
                            // _service.BeginReceipt(op, ReceiptKind.Sale, optionalRequisites);
                            return Task.FromResult(CommandResponse.Ok());

                        case "PrintLine":
                        {
                            var text = command.Args.Value<string>("text") ?? "";
                            // _service.PrintText(text);
                            return Task.FromResult(CommandResponse.Ok());
                        }

                        case "PrintBarcode":
                        {
                            var code = command.Args.Value<string>("barcode");
                            var sym = command.Args.Value<string>("symbology") ?? "Code128";
                            var w = command.Args.Value<int?>("width") ?? 300;
                            var h = command.Args.Value<int?>("height") ?? 120;
                            // var imager = _service.CodeImager.Create(CodeSymbology.Code128);
                            // imager.Width = w; imager.Height = h; imager.Print(code);
                            return Task.FromResult(CommandResponse.Ok());
                        }

                        case "SetPayment":
                        {
                            var paymentType = command.Args.Value<int?>("paymentType") ?? 1; // 0 cash / 1 card...
                            var amount = command.Args.Value<decimal?>("amount") ?? 0m;
                            // _service.SetPayment((PaymentKind)paymentType, amount);
                            var data = new JObject { ["paymentType"] = paymentType, ["amount"] = amount };
                            return Task.FromResult(CommandResponse.Ok(data));
                        }

                        case "EndReceipt":
                        {
                            // var fd = _service.EndReceipt();
                            var data = new JObject { ["fiscalDocNo"] = "12345" /* fd.Number */ };
                            return Task.FromResult(CommandResponse.Ok(data));
                        }

                        default:
                            return Task.FromResult(CommandResponse.Fail($"Unknown command '{command.Name}'"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pilot adapter error on {Cmd}", command.Name);
                return Task.FromResult(CommandResponse.Fail(ex.Message));
            }
        }
    }
}
