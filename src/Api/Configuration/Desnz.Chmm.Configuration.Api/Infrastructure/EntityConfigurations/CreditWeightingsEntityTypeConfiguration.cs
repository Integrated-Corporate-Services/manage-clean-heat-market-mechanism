using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Credit Weightings
    /// </summary>
    public class CreditWeightingsEntityTypeConfiguration : IEntityTypeConfiguration<CreditWeighting>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<CreditWeighting> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.TotalCapacity).HasColumnType("int").IsRequired();
            
            entity.HasOne(s => s.SchemeYear).WithOne(o => o.CreditWeightings).IsRequired(false);
        
            entity.HasMany(o => o.HeatPumpTechnologyTypeWeightings).WithOne(a => a.CreditWeighting).HasForeignKey(a => a.CreditWeightingId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(o => o.AlternativeSystemFuelTypeWeightings).WithOne(a => a.CreditWeighting).HasForeignKey(a => a.CreditWeightingId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
