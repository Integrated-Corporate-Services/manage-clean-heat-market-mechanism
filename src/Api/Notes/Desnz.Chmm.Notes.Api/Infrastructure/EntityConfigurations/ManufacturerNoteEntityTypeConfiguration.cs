using Desnz.Chmm.Notes.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Notes.Api.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Schema Configurations for Manufacturer Notes
    /// </summary>
    public class ManufacturerNoteEntityTypeConfiguration : IEntityTypeConfiguration<ManufacturerNote>
    {
        /// <summary>
        /// Configures the schema element
        /// </summary>
        /// <param name="entity">The entity type to configure</param>
        public void Configure(EntityTypeBuilder<ManufacturerNote> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.OrganisationId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.SchemeYearId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.Details).HasColumnType("text").IsRequired();
        }
    }
}
