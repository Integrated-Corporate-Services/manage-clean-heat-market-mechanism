using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Heat Pump Technology Type Weighting
    /// </summary>
    public class HeatPumpTechnologyTypeWeightingEntityTypeConfiguration : IEntityTypeConfiguration<HeatPumpTechnologyTypeWeighting>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<HeatPumpTechnologyTypeWeighting> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.Code).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.Value).HasColumnType("decimal").IsRequired();

            entity.HasOne(s => s.CreditWeighting).WithMany(o => o.HeatPumpTechnologyTypeWeightings).HasForeignKey(a => a.CreditWeightingId).IsRequired();
        }
    }
}
