using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Configuration.Api.Infrastructure;

/// <summary>
/// The DB Context for Configuration
/// </summary>
public class ConfigurationContext : DbContext, IConfigurationContext
{
    public DbSet<SchemeYear> SchemeYears { get; set; }
    public DbSet<SchemeYearQuarter> SchemeYearQuarters { get; set; }
    public DbSet<CreditWeighting> CreditWeightings { get; set; }
    public DbSet<ObligationCalculations> ObligationCalculations { get; set; }
    public DbSet<AlternativeSystemFuelTypeWeighting> AlternativeSystemFuelTypeWeightings { get; set; }
    public DbSet<AlternativeSystemFuelTypeWeightingValue> AlternativeSystemFuelTypeWeightingValues { get; set; }
    public DbSet<HeatPumpTechnologyTypeWeighting> HeatPumpTechnologyTypeWeightings { get; set; }


    /// <summary>
    /// Constructor taking all dependencies
    /// </summary>
    /// <param name="options">The options for the Configuration Context</param>
    public ConfigurationContext(DbContextOptions<ConfigurationContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures Elements in the schema
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure elements</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new SchemeYearEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SchemeYearQuarterEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CreditWeightingsEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ObligationCalculationsEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AlternativeSystemFuelTypeWeightingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AlternativeSystemFuelTypeWeightingValueEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new HeatPumpTechnologyTypeWeightingEntityTypeConfiguration());
    }
}
