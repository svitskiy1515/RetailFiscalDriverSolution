using System.Threading;
using System.Threading.Tasks;

namespace RetailFiscalDriver.Shared.Contracts;

/// <summary>Обработчик конкретной команды внутри пакета.</summary>
public interface ICommandHandler
{
    string CommandName { get; }              // например: "BeginReceipt"
    bool CanHandle(Command command);         // по имени/контексту
    Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default);
}
