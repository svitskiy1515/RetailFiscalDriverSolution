using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriverWindowsService.Persistence
{
    public interface IIdempotencyStore
    {
        Task<(bool exists, string responseJson)> TryGetAsync(string packageId, CancellationToken ct);
        Task SaveAsync(string packageId, int status, string responseJson, CancellationToken ct);
    }

}
