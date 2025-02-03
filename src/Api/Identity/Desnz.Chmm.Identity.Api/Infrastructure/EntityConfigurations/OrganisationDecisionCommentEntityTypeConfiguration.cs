using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationDecisionCommentEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationDecisionComment>
    {
        public void Configure(EntityTypeBuilder<OrganisationDecisionComment> entity)
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnType("uuid");
            entity.Property(c => c.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(c => c.Comment).HasColumnType("text").IsRequired();
            entity.Property(c => c.Decision).HasColumnType("text").IsRequired();

            entity.HasOne(c => c.ChmmUser).WithMany(u => u.Comments).HasForeignKey(c => c.ChmmUserId).IsRequired();
            entity.HasOne(c => c.Organisation).WithMany(o => o.Comments).HasForeignKey(c => c.OrganisationId).IsRequired();
        }
    }
}
