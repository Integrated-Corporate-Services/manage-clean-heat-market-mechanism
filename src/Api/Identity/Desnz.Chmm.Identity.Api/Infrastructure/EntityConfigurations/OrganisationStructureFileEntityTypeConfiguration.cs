using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationStructureFileEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationStructureFile>
    {
        public void Configure(EntityTypeBuilder<OrganisationStructureFile> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.FileKey).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.FileName).HasColumnType("varchar(100)").IsRequired();

            entity.HasOne(c => c.Organisation).WithMany(o => o.OrganisationStructureFiles).HasForeignKey(c => c.OrganisationId).IsRequired();
        }
    }
}
