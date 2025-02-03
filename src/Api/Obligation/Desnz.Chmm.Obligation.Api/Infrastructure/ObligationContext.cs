using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Obligation.Api.Infrastructure;

public class ObligationContext : EntityDbContext, IUnitOfWork
{
    /// <summary>
    /// DbSet of Credit Transfers
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; }

    public ObligationContext(DbContextOptions<ObligationContext> options, ICurrentUserService currentUserService) : base(options, currentUserService)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration());

    }
}
