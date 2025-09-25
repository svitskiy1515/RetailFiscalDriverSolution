using DriverWindowsService.Persistence;
using DriverWindowsService.Persistence.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RetailFiscalDriver.Shared.Contracts;
using Shared.Contracts.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Handlers
{
    [PackageType("Fiscal")]
    public sealed class FiscalReceiptHandler : IPackageHandler
    {
        private readonly ILogger<FiscalReceiptHandler> _logger;
        private readonly IDriverService _driver;
        private readonly IFiscalStore _store;

        public FiscalReceiptHandler(ILogger<FiscalReceiptHandler> logger,
                                    IDriverService driver,
                                    IFiscalStore store)
        {
            _logger = logger;
            _driver = driver;
            _store = store;
        }

        public string PackageType => "Fiscal";
        public bool CanHandle(string packageType) =>
            packageType.Equals(PackageType, StringComparison.OrdinalIgnoreCase);

        public async Task<Package> HandleAsync(Package package, CancellationToken ct = default)
        {
            _logger.LogInformation("Handle Fiscal {Id}", package.PackageId);

            // по умолчанию считаем, что операция неуспешна
            var op = new FiscalOperation
            {
                TransactionId = package.PackageId,  // или пакетные метаданные/ид транзакции POS
                BatchId = package.BatchId,
                TerminalId = package.TerminalId,
                OperationType = (int)FiscalOperationType.Sale, // при желании маппить из Args
                PaymentType = (int)PaymentType.Other,
                Amount = 0m,
                IsSuccess = false
            };

            try
            {
                await _store.WithTransactionAsync(async _ =>
                {
                    foreach (var cmd in package.Commands)
                    {
                        var r = await _driver.ExecuteAsync(cmd, ct);
                        if (!r.Success)
                        {
                            op.Error = $"Command {cmd.Name} failed: {r.Error}";
                            package.Status = PackageStatus.Failed;
                            await _store.AddOperationAsync(op, ct);
                            return; // выходим, транзакция откатится в catch выше? — нет, мы не бросили
                        }

                        // собираем полезные данные из ответов
                        if (cmd.Name.Equals("EndReceipt", StringComparison.OrdinalIgnoreCase))
                        {
                            var docNo = r.Data?.Value<string>("fiscalDocNo");
                            if (!string.IsNullOrEmpty(docNo)) op.DocumentNumber = docNo;
                            op.CompletedAt = DateTime.UtcNow;
                            op.IsSuccess = true;
                            package.Status = PackageStatus.Done;
                        }

                        if (cmd.Name.Equals("SetPayment", StringComparison.OrdinalIgnoreCase))
                        {
                            op.PaymentType = (int)(r.Data?.Value<int?>("paymentType") ?? (int)PaymentType.Other);
                            op.Amount = r.Data?.Value<decimal?>("amount") ?? 0m;
                        }
                    }

                    // финальная запись
                    await _store.AddOperationAsync(op, ct);
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fiscal processing failed");
                op.IsSuccess = false;
                op.Error = ex.Message;
                package.Status = PackageStatus.Failed;
                // вне транзакции фиксируем факт ошибки
                await _store.AddOperationAsync(op, ct);
            }

            package.ProcessedAt = DateTime.UtcNow;
            return package;
        }
    }
}
