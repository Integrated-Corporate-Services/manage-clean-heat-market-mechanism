using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations;

public class ChmmUserEntityTypeConfiguration : IEntityTypeConfiguration<ChmmUser>
{
    public void Configure(EntityTypeBuilder<ChmmUser> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Subject).HasColumnType("varchar(100)").IsRequired(false);
        entity.Property(u => u.Id).HasColumnType("uuid");
        entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
        entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();
        entity.Property(u => u.Name).HasColumnType("varchar(100)").IsRequired();
        entity.Property(u => u.Email).HasColumnType("varchar(100)").IsRequired();
        entity.Property(u => u.Status).HasColumnType("varchar(100)").IsRequired();
        entity.Property(u => u.JobTitle).HasColumnType("varchar(100)").IsRequired(false);
        entity.Property(u => u.TelephoneNumber).HasColumnType("varchar(100)").IsRequired(false);

        entity.HasIndex(u => u.Name).IsUnique(false);
        entity.HasIndex(u => u.Email).IsUnique();
        entity.HasIndex(u => u.Subject).IsUnique();

        entity.HasMany(u => u.ChmmRoles).WithMany(r => r.ChmmUsers);
        entity.HasMany(u => u.Comments).WithOne(c => c.ChmmUser).HasForeignKey(c => c.ChmmUserId).IsRequired();
        entity.HasOne(u => u.Organisation).WithMany(o => o.ChmmUsers).HasForeignKey(u => u.OrganisationId).IsRequired(false);
    }
}
