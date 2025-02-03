using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity Framework configuration for <see cref="TransactionEntry">Transactions</see>
    /// </summary>
    public class TransactionEntryEntityTypeConfiguration : IEntityTypeConfiguration<TransactionEntry>
    {
        void IEntityTypeConfiguration<TransactionEntry>.Configure(EntityTypeBuilder<TransactionEntry> entity)
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.OrganisationId);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.OrganisationId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.Value).HasColumnType("decimal").IsRequired();

            entity.HasOne(u => u.Transaction).WithMany(u => u.Entries).HasForeignKey(u => u.TransactionId).IsRequired();
        }
    }
}
