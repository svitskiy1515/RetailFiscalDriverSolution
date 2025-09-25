using System.Threading;
using System.Threading.Tasks;

namespace RetailFiscalDriver.Shared.Contracts;

/// <summary>Тонкая абстракция под конкретный драйвер ККТ (Pilot/Atol/…)</summary>
public interface IDriverService
{
    Task<CommandResponse> ExecuteAsync(Command command, CancellationToken ct = default);
}
