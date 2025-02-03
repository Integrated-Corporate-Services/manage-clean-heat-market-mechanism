using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class ChmmRoleEntityTypeConfiguration : IEntityTypeConfiguration<ChmmRole>
    {
        public void Configure(EntityTypeBuilder<ChmmRole> entity)
        {
            entity.HasKey(cr => cr.Id);
            
            entity.Property(p => p.Id).HasColumnType("uuid");
            entity.Property(p => p.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();

            entity.Property(p => p.Name).HasColumnType("varchar(100)").IsRequired();
            entity.HasIndex(cr => cr.Name).IsUnique();

            entity.HasIndex(cr => cr.Name).IsUnique();

            entity.HasMany(u => u.ChmmUsers).WithMany(r => r.ChmmRoles);
        }
    }
}
