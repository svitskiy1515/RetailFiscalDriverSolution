using System;

namespace DriverWindowsService.Persistence.Entities
{
    public class FiscalOperation
    {
        public long RecordId { get; set; }               // Identity
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string TransactionId { get; set; } = "";  // корреляция с POS
        public string BatchId { get; set; } = "";
        public string TerminalId { get; set; } = "";

        public int OperationType { get; set; }           // map из FiscalOperationType
        public int PaymentType { get; set; }             // map из PaymentType

        public decimal Amount { get; set; }              // итог по чеку
        public bool IsSuccess { get; set; }              // успешность фискализации
        public string Error { get; set; }                // текст ошибки (если была)

        public string DocumentNumber { get; set; }       // № фискального документа (если есть)
        public DateTime? CompletedAt { get; set; }       // время завершения операции

        public string FiscalDataJson { get; set; }       // любые расширенные данные (JSON)
    }
}