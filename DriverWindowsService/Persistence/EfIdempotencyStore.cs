using DriverWindowsService.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Persistence
{
    public sealed class EfIdempotencyStore : IIdempotencyStore
    {
        private readonly FiscalDbContext _db;
        public EfIdempotencyStore(FiscalDbContext db) => _db = db;

        public async Task<(bool, string)> TryGetAsync(string packageId, CancellationToken ct)
        {
            var e = await _db.ProcessedPackages.FirstOrDefaultAsync(x => x.PackageId == packageId, ct);
            return e == null ? (false, null) : (e.Status == 1, e.ResponseJson);
        }

        public async Task SaveAsync(string packageId, int status, string responseJson, CancellationToken ct)
        {
            var e = await _db.ProcessedPackages.FirstOrDefaultAsync(x => x.PackageId == packageId, ct);
            if (e == null)
            {
                e = new ProcessedPackage { PackageId = packageId };
                _db.ProcessedPackages.Add(e);
            }
            e.Status = status;
            e.ResponseJson = responseJson;
            e.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

}
