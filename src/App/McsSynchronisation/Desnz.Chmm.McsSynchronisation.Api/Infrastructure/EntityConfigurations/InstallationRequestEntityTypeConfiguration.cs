using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations
{
    public class InstallationRequestEntityTypeConfiguration : IEntityTypeConfiguration<InstallationRequest>
    {
        public void Configure(EntityTypeBuilder<InstallationRequest> entity)
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id).HasColumnType("uuid");
            entity.Property(a => a.RequestDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(a => a.StartDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(a => a.EndDate).HasColumnType("timestamptz").IsRequired();

            entity.Property(p => p.IsNewBuildIds).HasColumnType("integer[]").IsRequired(false);
            entity.Property(p => p.TechnologyTypeIds).HasColumnType("integer[]").IsRequired(false);

            entity.HasMany(o => o.HeatPumpInstallations).WithOne(c => c.InstallationRequest).HasForeignKey(c => c.InstallationRequestId).IsRequired();
        }

        protected virtual void ConfigureHeatPumpsProperty(EntityTypeBuilder<InstallationRequest> entity)
        {
            entity.Property(p => p.TechnologyTypeIds).HasColumnType("integer[]").IsRequired(false);
        }
    }
}
