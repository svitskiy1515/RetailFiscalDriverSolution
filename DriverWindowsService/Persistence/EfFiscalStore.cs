using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using DriverWindowsService.Persistence.Entities;

namespace DriverWindowsService.Persistence
{
    public class EfFiscalStore : IFiscalStore
    {
        private readonly FiscalDbContext _db;

        public EfFiscalStore(FiscalDbContext db) => _db = db;

        public async Task<long> AddOperationAsync(FiscalOperation op, CancellationToken ct)
        {
            _db.FiscalOperations.Add(op);
            await _db.SaveChangesAsync();
            return op.RecordId;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync();

        public async Task WithTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct)
        {
            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    await work(ct);
                    await _db.SaveChangesAsync();
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}