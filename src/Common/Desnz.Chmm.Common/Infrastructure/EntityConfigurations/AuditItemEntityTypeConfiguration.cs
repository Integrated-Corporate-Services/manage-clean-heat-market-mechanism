using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.Common.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Entity type confiigration for an Audit Item entity
    /// </summary>
    public class AuditItemEntityTypeConfiguration : IEntityTypeConfiguration<AuditItem>
    {
        /// <summary>
        /// Map the audit item entity to it's respective data types
        /// </summary>
        /// <param name="entity"></param>
        public void Configure(EntityTypeBuilder<AuditItem> entity)
        {
            entity.HasKey(cr => cr.Id);

            entity.Property(p => p.Id).HasColumnType("uuid");
            entity.Property(p => p.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(p => p.TraceId).HasColumnType("varchar(100)").IsRequired();
            entity.Property(p => p.ResultMessage).HasColumnType("text").IsRequired();
            entity.Property(p => p.UserId).HasColumnType("uuid").IsRequired(false);
            entity.Property(p => p.Details).HasColumnType("text").IsRequired();
            entity.Property(p => p.WasSuccessful).HasColumnType("boolean").IsRequired();
            entity.Property(p => p.MillisecondsTaken).HasColumnType("bigint").IsRequired();

            entity.Property(p => p.FriendlyName).HasColumnType("varchar(100)").IsRequired();
            entity.Property(p => p.FullName).HasColumnType("varchar(512)").IsRequired();
        }
    }
}
