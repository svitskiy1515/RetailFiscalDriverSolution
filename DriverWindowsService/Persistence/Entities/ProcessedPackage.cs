using System;

namespace DriverWindowsService.Persistence.Entities
{
    public class ProcessedPackage
    {
        public long RecordId { get; set; }
        public string PackageId { get; set; } = "";
        public int Status { get; set; } // 0 New, 1 Done, 2 Failed
        public string ResponseJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}