using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Scheme Year
    /// </summary>
    public class ObligationCalculationsEntityTypeConfiguration : IEntityTypeConfiguration<ObligationCalculations>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<ObligationCalculations> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.GasCreditsCap).HasColumnType("int").IsRequired();
            entity.Property(u => u.OilCreditsCap).HasColumnType("int").IsRequired();
            entity.Property(u => u.PercentageCap).HasColumnType("decimal").IsRequired();
            entity.Property(u => u.TargetMultiplier).HasColumnType("decimal").IsRequired();

            entity.Property(u => u.GasBoilerSalesThreshold).HasColumnType("int").IsRequired();
            entity.Property(u => u.OilBoilerSalesThreshold).HasColumnType("int").IsRequired();
            entity.Property(u => u.TargetRate).HasColumnType("decimal").IsRequired();

            entity.HasOne(u => u.SchemeYear).WithOne(u => u.ObligationCalculations).IsRequired(false);

        }
    }
}
