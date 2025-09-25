using System;

namespace Shared.Contracts
{
    [Serializable]
    public class PosCommand
    {
        public Guid CommandId { get; set; } = Guid.NewGuid();
        public string PosId { get; set; } = "POS01";
        public PilotCommand Command { get; set; }
    }

    [Serializable]
    public class PosCommandResult
    {
        public Guid CommandId { get; set; }
        public string PosId { get; set; }
        public Error Error { get; set; }
        public bool IsCompleted { get; set; }
    }

    // ===== Stubs to make the sample compile without external libs =====
    [Serializable]
    public abstract class PilotCommand
    {
        public Error Error { get; set; } = new Error(0, ErrorCodes.ERR_SUCCESS);
        public abstract string Name { get; }
        public virtual void Run(object driver)
        {
            // simulate success by default
            Error = new Error(0, ErrorCodes.ERR_SUCCESS);
        }
    }

    [Serializable]
    public class SaleCommand : PilotCommand, IFiscalCommand
    {
        public decimal Amount { get; set; }
        public override string Name => "Sale";
    }

    public interface IFiscalCommand
    {
        decimal Amount { get; }
    }

    [Serializable]
    public class Error
    {
        public int CommandCode { get; set; }
        public ErrorCodes Result { get; set; }
        public string Description { get; set; }

        public Error(int code, ErrorCodes result)
        {
            CommandCode = code;
            Result = result;
            Description = result.ToString();
        }
    }

    public enum ErrorCodes
    {
        ERR_SUCCESS = 0,
        ERR_FAIL = 1,
        ERR_UNKNOWN = 2
    }
}
