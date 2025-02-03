using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Licence holder entity type configuration
    /// </summary>
    public class LicenceHolderEntityTypeConfiguration : IEntityTypeConfiguration<LicenceHolder>
    {
        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="entity">The entity to configure</param>
        public void Configure(EntityTypeBuilder<LicenceHolder> entity)
        {
            entity.HasKey(o => o.Id);
            entity.HasIndex(o => o.McsManufacturerId, "IX_UNIQUE_LICENCE_HOLDER").IsUnique();

            entity.Property(o => o.Id).HasColumnType("uuid");
            entity.Property(o => o.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(o => o.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(o => o.Name).HasColumnType("varchar(100)").IsRequired();
            entity.Property(o => o.McsManufacturerId).HasColumnType("int").IsRequired();

            entity.HasMany(o => o.LicenceHolderLinks).WithOne(l => l.LicenceHolder).HasForeignKey(l => l.LicenceHolderId).IsRequired(false);
        }
    }
}
