using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Infrastructure.EntityConfigurations;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Common.Infrastructure;

/// <summary>
/// Db context for System Audit
/// </summary>
public class AuditingContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Db Set of Audit Items
    /// </summary>
    public DbSet<AuditItem> AuditItems { get; set; }

    /// <summary>
    /// Default constructor for system audit context
    /// </summary>
    /// <param name="options">Db Context Options</param>
    public AuditingContext(DbContextOptions<AuditingContext> options) : base(options)
    {
    }

    /// <summary>
    /// Apply all configurations for the model builder
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new AuditItemEntityTypeConfiguration());
    }
}
