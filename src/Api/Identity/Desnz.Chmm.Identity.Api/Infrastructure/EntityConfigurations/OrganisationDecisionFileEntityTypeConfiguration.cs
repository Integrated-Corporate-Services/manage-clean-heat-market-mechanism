using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationDecisionFileEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationDecisionFile>
    {
        public void Configure(EntityTypeBuilder<OrganisationDecisionFile> entity)
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnType("uuid");
            entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(u => u.FileKey).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.FileName).HasColumnType("varchar(100)").IsRequired();

            entity.Property(c => c.Decision).HasColumnType("text").IsRequired();

            entity.HasOne(c => c.Organisation).WithMany(o => o.OrganisationDecisionFiles).HasForeignKey(c => c.OrganisationId).IsRequired();
        }
    }
}
