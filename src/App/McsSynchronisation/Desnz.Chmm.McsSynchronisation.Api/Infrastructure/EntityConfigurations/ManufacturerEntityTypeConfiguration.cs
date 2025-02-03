using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations
{
    public class ManufacturerEntityTypeConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> entity)
        {
            entity.HasKey(cr => cr.Id);

            entity.Property(p => p.Id).HasColumnType("integer").IsRequired();
            entity.Property(p => p.Description).HasColumnType("varchar(255)").IsRequired();
        }
    }
}