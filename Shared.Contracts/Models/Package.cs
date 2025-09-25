using System;
using System.Collections.Generic;
using Shared.Contracts.Enums;

namespace Shared.Contracts.Models;

public sealed class Package
{
    public string PackageId { get; set; } = Guid.NewGuid().ToString("N");
    public string TerminalId { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Fiscal" / "NonFiscal" / др.
    public PackageStatus Status { get; set; } = PackageStatus.New;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    public List<Command> Commands { get; set; } = new();
}
