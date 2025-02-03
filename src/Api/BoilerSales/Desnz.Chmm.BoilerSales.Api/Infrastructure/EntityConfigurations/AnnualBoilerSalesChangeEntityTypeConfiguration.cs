using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.EntityConfigurations;

public class AnnualBoilerSalesChangeEntityTypeConfiguration : IEntityTypeConfiguration<AnnualBoilerSalesChange>
{
    void IEntityTypeConfiguration<AnnualBoilerSalesChange>.Configure(EntityTypeBuilder<AnnualBoilerSalesChange> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Id).HasColumnType("uuid");
        entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
        entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

        entity.Property(u => u.OldGas).HasColumnType("integer").IsRequired(false);
        entity.Property(u => u.OldOil).HasColumnType("integer").IsRequired(false);
        entity.Property(u => u.OldStatus).HasColumnType("varchar(50)").IsRequired(false);

        entity.Property(u => u.NewGas).HasColumnType("integer").IsRequired();
        entity.Property(u => u.NewOil).HasColumnType("integer").IsRequired();
        entity.Property(u => u.NewStatus).HasColumnType("varchar(50)").IsRequired();

        entity.Property(u => u.Note).HasColumnType("text").IsRequired();

        entity.HasOne(u => u.AnnualBoilerSales).WithMany(u => u.Changes).HasForeignKey(u => u.AnnualBoilerSalesId).IsRequired();
    }
}
