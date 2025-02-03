using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Scheme Year
    /// </summary>
    public class SchemeYearEntityTypeConfiguration : IEntityTypeConfiguration<SchemeYear>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<SchemeYear> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.Name).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.Year).HasColumnType("int").IsRequired();
            entity.Property(u => u.StartDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.EndDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.TradingWindowStartDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.TradingWindowEndDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.CreditGenerationWindowStartDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.CreditGenerationWindowEndDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.BoilerSalesSubmissionEndDate).HasColumnType("date").IsRequired();

            entity.HasMany(o => o.Quarters).WithOne(a => a.SchemeYear).HasForeignKey(a => a.SchemeYearId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(o => o.CreditWeightings).WithOne(a => a.SchemeYear).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(o => o.ObligationCalculations).WithOne(a => a.SchemeYear).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
