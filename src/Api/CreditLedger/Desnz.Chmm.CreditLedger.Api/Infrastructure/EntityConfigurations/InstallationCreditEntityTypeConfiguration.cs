using Desnz.Chmm.CreditLedger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity Framework configuration for <see cref="InstallationCredit">Installation Credits</see>
    /// </summary>
    public class InstallationCreditEntityTypeConfiguration : IEntityTypeConfiguration<InstallationCredit>
    {
        void IEntityTypeConfiguration<InstallationCredit>.Configure(EntityTypeBuilder<InstallationCredit> entity)
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(new[] { "LicenceHolderId", "HeatPumpInstallationId", "SchemeYearId", "IsHybrid" }, "IX_UNIQUE_INSTALLATION_CREDIT")
            .IsUnique();
            entity.HasIndex(u => new { u.SchemeYearId, u.LicenceHolderId });

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.LicenceHolderId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.HeatPumpInstallationId).HasColumnType("int").IsRequired();
            entity.Property(u => u.SchemeYearId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.DateCreditGenerated).HasColumnType("date").IsRequired();
            entity.Property(u => u.Value).HasColumnType("decimal").IsRequired();
            entity.Property(u => u.IsHybrid).HasColumnType("boolean").IsRequired();
        }
    }
}
