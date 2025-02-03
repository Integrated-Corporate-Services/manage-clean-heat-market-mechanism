using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure;

/// <summary>
/// Db Context for Credit Ledger
/// </summary>
public class CreditLedgerContext : EntityDbContext, IUnitOfWork
{
    /// <summary>
    /// DbSet of Credit Transfers
    /// </summary>
    public DbSet<CreditTransfer> CreditTransfers { get; set; }
    /// <summary>
    /// DbSet of Installation Credits
    /// </summary>
    public DbSet<InstallationCredit> InstallationCredits { get; set; }
    /// <summary>
    /// DbSet of Transactions
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; }
    /// <summary>
    /// DbSet of Transaction Entires
    /// </summary>
    public DbSet<TransactionEntry> TransactionEntries { get; set; }

    public CreditLedgerContext(DbContextOptions<CreditLedgerContext> options, ICurrentUserService currentUserService) : base(options, currentUserService)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new CreditTransferEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InstallationCreditEntityTypeConfiguration())
            .Entity<InstallationCredit>()
            .HasIndex(new[] { "LicenceHolderId", "HeatPumpInstallationId", "SchemeYearId", "IsHybrid" }, "IX_UNIQUE_INSTALLATION_CREDIT")
            .IsUnique();
        modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionEntryEntityTypeConfiguration());
    }
}
