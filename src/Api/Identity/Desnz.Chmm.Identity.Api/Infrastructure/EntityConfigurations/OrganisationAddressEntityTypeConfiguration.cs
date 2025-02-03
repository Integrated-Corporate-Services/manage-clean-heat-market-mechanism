using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationAddressEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationAddress>
    {
        public void Configure(EntityTypeBuilder<OrganisationAddress> entity)
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id).HasColumnType("uuid");
            entity.Property(a => a.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(a => a.Type).HasColumnType("varchar(100)").IsRequired();
            entity.Property(a => a.LineOne).HasColumnType("varchar(100)").IsRequired();
            entity.Property(a => a.LineTwo).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(a => a.City).HasColumnType("varchar(100)").IsRequired();
            entity.Property(a => a.County).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(a => a.PostCode).HasColumnType("varchar(100)").IsRequired();

            entity.HasOne(s => s.Organisation).WithMany(o => o.Addresses).HasForeignKey(a => a.OrganisationId).IsRequired();
        }
    }
}
