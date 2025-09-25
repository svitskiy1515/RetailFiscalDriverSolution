using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Models;

namespace Shared.Contracts.Abstractions;

/// <summary>Обработчик всего пакета (по типу пакета: "Fiscal", "NonFiscal"...)</summary>
public interface IPackageHandler
{
    string PackageType { get; }              // "Fiscal" / "NonFiscal"
    bool CanHandle(string packageType);
    Task<Package> HandleAsync(Package package, CancellationToken ct = default);
}
