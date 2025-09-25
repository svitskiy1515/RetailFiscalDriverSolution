using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Models;

namespace Shared.Contracts.Abstractions;

/// <summary>Обработчик конкретной команды внутри пакета.</summary>
public interface ICommandHandler
{
    string CommandName { get; }              // например: "BeginReceipt"
    bool CanHandle(Command command);         // по имени/контексту
    Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default);
}
