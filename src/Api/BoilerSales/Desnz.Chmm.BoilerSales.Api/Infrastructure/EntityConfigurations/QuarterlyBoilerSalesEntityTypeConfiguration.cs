using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.EntityConfigurations;

public class QuarterlyBoilerSalesEntityTypeConfiguration : IEntityTypeConfiguration<QuarterlyBoilerSales>
{
    void IEntityTypeConfiguration<QuarterlyBoilerSales>.Configure(EntityTypeBuilder<QuarterlyBoilerSales> entity)
    {
        entity.HasKey(u => u.Id);

        entity.HasIndex(u => new { u.SchemeYearQuarterId, u.OrganisationId, });
        entity.HasIndex(u => new { u.SchemeYearId, u.OrganisationId, });

        entity.Property(u => u.Id).HasColumnType("uuid");
        entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
        entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

        entity.Property(u => u.SchemeYearId).HasColumnType("uuid").IsRequired();
        entity.Property(u => u.SchemeYearQuarterId).HasColumnType("uuid").IsRequired();
        entity.Property(u => u.OrganisationId).HasColumnType("uuid").IsRequired();
        entity.Property(u => u.Gas).HasColumnType("integer").IsRequired();
        entity.Property(u => u.Oil).HasColumnType("integer").IsRequired();
        entity.Property(u => u.Status).HasColumnType("varchar(50)").IsRequired();

        entity.HasMany(u => u.Files);
        entity.HasMany(u => u.Changes);
    }
}
