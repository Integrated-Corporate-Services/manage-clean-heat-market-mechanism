using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Scheme Year Quarter
    /// </summary>
    public class SchemeYearQuarterEntityTypeConfiguration : IEntityTypeConfiguration<SchemeYearQuarter>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<SchemeYearQuarter> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.Name).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.StartDate).HasColumnType("date").IsRequired();
            entity.Property(u => u.EndDate).HasColumnType("date").IsRequired();
            
            entity.HasOne(s => s.SchemeYear).WithMany(o => o.Quarters).HasForeignKey(a => a.SchemeYearId).IsRequired();
        }
    }
}
