using System.Data.Entity;
using DriverWindowsService.Persistence.Entities;

namespace DriverWindowsService.Persistence
{
    public class FiscalDbContext : DbContext
    {
        // имя строки подключения в App.config -> <connectionStrings>
        public FiscalDbContext() : base("name=FiscalDb") { }

        public DbSet<FiscalOperation> FiscalOperations { get; set; }
        public DbSet<ProcessedPackage> ProcessedPackages { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedPackage>().HasIndex(x => x.PackageId).IsUnique();
           
            var op = modelBuilder.Entity<FiscalOperation>();
            op.HasKey(x => x.RecordId);
            op.Property(x => x.TransactionId).HasMaxLength(64).IsRequired();
            op.Property(x => x.BatchId).HasMaxLength(64).IsRequired();
            op.Property(x => x.TerminalId).HasMaxLength(64).IsRequired();
            op.Property(x => x.DocumentNumber).HasMaxLength(64);
            op.Property(x => x.Error).IsOptional().IsMaxLength();
            op.Property(x => x.FiscalDataJson).IsOptional().IsMaxLength();
            base.OnModelCreating(modelBuilder);
        }
    }
}