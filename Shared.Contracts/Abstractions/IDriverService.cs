using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Models;

namespace Shared.Contracts.Abstractions;

/// <summary>Тонкая абстракция под конкретный драйвер ККТ (Pilot/Atol/…)</summary>
public interface IDriverService
{
    Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default);
}
