using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure;

public class McsSynchronisationContext : DbContext, IUnitOfWork
{
    public DbSet<InstallationRequest> InstallationRequests { get; set; }

    public DbSet<HeatPumpProduct> HeatPumpProducts { get; set; }

    public DbSet<HeatPumpInstallation> HeatPumpInstallations { get; set; }
    
    public DbSet<HeatPumpInstallationProduct> HeatPumpInstallationProducts { get; set; }

    // Reference types
    public DbSet<AirTypeTechnology> AirTypeTechnologies { get; set; }
    public DbSet<AlternativeSystemFuelType> AlternativeSystemFuelTypes { get; set; }
    public DbSet<AlternativeSystemType> AlternativeSystemTypes { get; set; }
    public DbSet<InstallationAge> InstallationAges { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<NewBuildOption> NewBuildOptions { get; set; }
    public DbSet<RenewableSystemDesign> RenewableSystemDesigns { get; set; }
    public DbSet<TechnologyType> TechnologyTypes { get; set; }


    public McsSynchronisationContext(DbContextOptions<McsSynchronisationContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new HeatPumpProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new HeatPumpInstallationEntityTypeConfiguration());

        // Reference types
        modelBuilder.ApplyConfiguration(new AirTypeTechnologyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AlternativeSystemFuelTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AlternativeSystemTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InstallationAgeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ManufacturerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new NewBuildOptionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RenewableSystemDesignEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TechnologyTypeEntityTypeConfiguration());

    }
}
