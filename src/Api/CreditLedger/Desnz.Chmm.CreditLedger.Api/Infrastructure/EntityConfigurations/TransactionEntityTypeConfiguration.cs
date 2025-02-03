using Desnz.Chmm.CreditLedger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity Framework configuration for <see cref="Transaction">Transactions</see>
    /// </summary>
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        void IEntityTypeConfiguration<Transaction>.Configure(EntityTypeBuilder<Transaction> entity)
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => new { u.SchemeYearId, u.TransactionType });
            entity.HasIndex(u => u.SchemeYearId);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.InitiatedBy).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.TransactionType).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.SchemeYearId).HasColumnType("uuid").IsRequired();

            entity.HasOne(u => u.CreditTransfer).WithOne(u => u.Transaction).IsRequired(false);
            entity.HasMany(u => u.Entries).WithOne(u => u.Transaction).HasForeignKey(u => u.TransactionId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        }
    }
}
