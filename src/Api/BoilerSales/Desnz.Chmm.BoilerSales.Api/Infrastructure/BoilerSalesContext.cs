using Desnz.Chmm.BoilerSales.Api.Infrastructure.EntityConfigurations;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Services;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure;

/// <summary>
/// Databae context giving access to boiler sales details
/// </summary>
public class BoilerSalesContext : EntityDbContext, IBoilerSalesContext
{
    /// <summary>
    /// Details of annual boiler sales
    /// </summary>
    public DbSet<AnnualBoilerSales> AnnualBoilerSales { get; set; }

    /// <summary>
    /// Files relating to annual boiler sales
    /// </summary>
    public DbSet<AnnualBoilerSalesFile> AnnualBoilerSalesFiles { get; set; }

    /// <summary>
    /// Changes/adjustments to annual boiler sales figures
    /// </summary>
    public DbSet<AnnualBoilerSalesChange> AnnualBoilerSalesChanges { get; set; }

    /// <summary>
    /// Details of quarterly boiler sales
    /// </summary>
    public DbSet<QuarterlyBoilerSales> QuarterlyBoilerSales { get; set; }

    /// <summary>
    /// Files relating to quarterly boiler sales
    /// </summary>
    public DbSet<QuarterlyBoilerSalesFile> QuarterlyBoilerSalesFiles { get; set; }

    /// <summary>
    /// Changes/adjustments to quarterly sales figures
    /// </summary>
    public DbSet<QuarterlyBoilerSalesChange> QuarterlyBoilerSalesChanges { get; set; }

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="options">Database options</param>
    public BoilerSalesContext(DbContextOptions<BoilerSalesContext> options, ICurrentUserService currentUserService) : base(options, currentUserService)
    {
    }

    /// <summary>
    /// Configures database model
    /// </summary>
    /// <param name="modelBuilder">Builder for database model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new AnnualBoilerSalesEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AnnualBoilerSalesFileEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AnnualBoilerSalesChangeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QuarterlyBoilerSalesEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QuarterlyBoilerSalesFileEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QuarterlyBoilerSalesChangeEntityTypeConfiguration());
    }
}
