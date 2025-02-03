using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Alternative System Fuel Type Weightings
    /// </summary>
    public class AlternativeSystemFuelTypeWeightingValueEntityTypeConfiguration : IEntityTypeConfiguration<AlternativeSystemFuelTypeWeightingValue>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<AlternativeSystemFuelTypeWeightingValue> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.Value).HasColumnType("decimal").IsRequired();
            entity.Property(u => u.Type).HasColumnType("varchar(100)").IsRequired();

            entity.HasMany(o => o.AlternativeSystemFuelTypeWeightings).WithOne(a => a.AlternativeSystemFuelTypeWeightingValue).HasForeignKey(a => a.AlternativeSystemFuelTypeWeightingValueId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
