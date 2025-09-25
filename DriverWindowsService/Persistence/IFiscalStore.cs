using System.Threading;
using System.Threading.Tasks;
using DriverWindowsService.Persistence.Entities;
using System;

namespace DriverWindowsService.Persistence
{
    public interface IFiscalStore
    {
        Task<long> AddOperationAsync(FiscalOperation op, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
        Task WithTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct);
    }
}