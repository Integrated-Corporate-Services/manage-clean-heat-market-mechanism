using Desnz.Chmm.CreditLedger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity Framework configuration for <see cref="CreditTransfer">Credit Transfers</see>
    /// </summary>
    public class CreditTransferEntityTypeConfiguration : IEntityTypeConfiguration<CreditTransfer>
    {
        void IEntityTypeConfiguration<CreditTransfer>.Configure(EntityTypeBuilder<CreditTransfer> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.SourceOrganisationId).HasColumnType("uuid");
            entity.Property(u => u.DestinationOrganisationId).HasColumnType("uuid");
            entity.Property(u => u.SchemeYearId).HasColumnType("uuid");
            entity.Property(u => u.Value).HasColumnType("decimal");
            entity.Property(u => u.Status).HasColumnType("varchar(100)");

            entity.HasOne(u => u.Transaction).WithOne(u => u.CreditTransfer).IsRequired(false);
        }
    }
}
