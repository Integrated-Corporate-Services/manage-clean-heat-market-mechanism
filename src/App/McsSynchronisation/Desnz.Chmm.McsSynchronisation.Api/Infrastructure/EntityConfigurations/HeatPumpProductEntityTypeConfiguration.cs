using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations
{
    public class HeatPumpProductEntityTypeConfiguration : IEntityTypeConfiguration<HeatPumpProduct>
    {
        public void Configure(EntityTypeBuilder<HeatPumpProduct> entity)
        {
            entity.HasKey(cr => cr.Id);

            entity.Property(p => p.Id).HasColumnType("integer").IsRequired();
            entity.Property(p => p.Code).HasColumnType("varchar(100)").IsRequired();
            entity.Property(p => p.Name).HasColumnType("varchar(500)").IsRequired();
            entity.Property(p => p.ManufacturerId).HasColumnType("integer").IsRequired();
            entity.Property(p => p.ManufacturerName).HasColumnType("varchar(500)").IsRequired();

            entity.HasMany(u => u.HeatPumpInstallations)
                .WithMany(r => r.HeatPumpProducts)
                .UsingEntity<HeatPumpInstallationProduct>(
                    "HeatPumpInstallationProducts",
                    l => l.HasOne<HeatPumpInstallation>().WithMany().HasForeignKey(e => e.InstallationId),
                    r => r.HasOne<HeatPumpProduct>().WithMany().HasForeignKey(e => e.ProductId),
                    j => j.HasKey("Id"));
        }
    }
}