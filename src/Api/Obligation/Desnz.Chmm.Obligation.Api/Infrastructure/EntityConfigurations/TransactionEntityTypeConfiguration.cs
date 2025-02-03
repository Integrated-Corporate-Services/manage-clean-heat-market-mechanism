using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Obligation.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity Framework configuration for <see cref="Transaction">Credit Transfers</see>
    /// </summary>
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Entities.Transaction>
    {
        void IEntityTypeConfiguration<Entities.Transaction>.Configure(EntityTypeBuilder<Entities.Transaction> entity)
        {
            entity.HasKey(u => u.Id);

            
            entity.HasIndex(u => new { u.SchemeYearId, u.OrganisationId, u.TransactionType })
                .HasFilter("\"TransactionType\" = 'CarryForward' OR \"TransactionType\" = 'BroughtForward'")
                .IsUnique();
            entity.HasIndex(u => new { u.SchemeYearId, u.SchemeYearQuarterId, u.OrganisationId, u.TransactionType, u.DateOfTransaction }).IsDescending();
            entity.HasIndex(u => new { u.SchemeYearId, u.OrganisationId, u.TransactionType, u.DateOfTransaction}).IsDescending();
            entity.HasIndex(u => new { u.SchemeYearId, u.OrganisationId, u.IsExcluded, u.DateOfTransaction }).IsDescending();
            entity.HasIndex(u => new { u.SchemeYearId, u.TransactionType, u.DateOfTransaction }).IsDescending();

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.UserId).HasColumnType("uuid");
            entity.Property(o => o.OrganisationId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.TransactionType).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.DateOfTransaction).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.SchemeYearId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.SchemeYearQuarterId).HasColumnType("uuid").IsRequired(false);
            entity.Property(u => u.IsExcluded).HasColumnType("boolean").IsRequired();
            entity.Property(u => u.Obligation).HasColumnType("integer").IsRequired();
        }
    }
}
