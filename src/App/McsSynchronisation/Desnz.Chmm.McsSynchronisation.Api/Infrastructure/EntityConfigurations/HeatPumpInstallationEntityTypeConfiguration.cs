using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations;

public class HeatPumpInstallationEntityTypeConfiguration : IEntityTypeConfiguration<HeatPumpInstallation>
{
    public void Configure(EntityTypeBuilder<HeatPumpInstallation> entity)
    {
        entity.HasKey(a => a.Id);

        entity.Property(a => a.Id).HasColumnType("uuid");
        entity.Property(a => a.MidId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.TechnologyTypeId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.AirTypeTechnologyId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.IsAlternativeHeatingSystemPresent).HasColumnType("boolean").IsRequired(false);
        entity.Property(p => p.AlternativeHeatingSystemId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.AlternativeHeatingFuelId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.AlternativeHeatingAgeId).HasColumnType("integer").IsRequired(false);
        entity.Property(u => u.CommissioningDate).HasColumnType("timestamptz").IsRequired(false);
        entity.Property(p => p.Mpan).HasColumnType("varchar(200)").IsRequired(false);
        entity.Property(p => p.TotalCapacity).HasColumnType("decimal").IsRequired(false);
        entity.Property(p => p.CertificatesCount).HasColumnType("integer").IsRequired(false);
        entity.Property(u => u.IsHybrid).HasColumnType("boolean").IsRequired(false);
        entity.Property(p => p.IsNewBuildId).HasColumnType("integer").IsRequired(false);
        entity.Property(p => p.RenewableSystemDesignId).HasColumnType("integer").IsRequired(false);
        entity.Property(u => u.IsSystemSelectedAsMCSTechnology).HasColumnType("boolean").IsRequired(false);

        entity.HasMany(u => u.HeatPumpProducts)
            .WithMany(r => r.HeatPumpInstallations)
                .UsingEntity<HeatPumpInstallationProduct>(
                    "HeatPumpInstallationProducts",
                    r => r.HasOne<HeatPumpProduct>().WithMany().HasForeignKey(e => e.ProductId),
                    l => l.HasOne<HeatPumpInstallation>().WithMany().HasForeignKey(e => e.InstallationId),
                    j => j.HasKey("Id"));

        entity.Ignore(c => c.Credits);
    }
}