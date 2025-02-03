using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Licence holder entity type configuration
    /// </summary>
    public class LicenceHolderLinkEntityTypeConfiguration : IEntityTypeConfiguration<LicenceHolderLink>
    {
        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="entity">The entity to configure</param>
        public void Configure(EntityTypeBuilder<LicenceHolderLink> entity)
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Id).HasColumnType("uuid");
            entity.Property(l => l.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(l => l.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(l => l.StartDate).HasColumnType("date").IsRequired();
            entity.Property(l => l.EndDate).HasColumnType("date").IsRequired(false);

            entity.HasOne(c => c.Organisation).WithMany(o => o.LicenceHolderLinks).HasForeignKey(c => c.OrganisationId).IsRequired();
            entity.HasOne(c => c.LicenceHolder).WithMany(o => o.LicenceHolderLinks).HasForeignKey(c => c.LicenceHolderId).IsRequired();
        }
    }
}
