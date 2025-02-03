using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.EntityConfigurations;

public class RenewableSystemDesignEntityTypeConfiguration : IEntityTypeConfiguration<RenewableSystemDesign>
{
    public void Configure(EntityTypeBuilder<RenewableSystemDesign> entity)
    {
        entity.HasKey(cr => cr.Id);

        entity.Property(p => p.Id).HasColumnType("integer").IsRequired();
        entity.Property(p => p.Description).HasColumnType("varchar(255)").IsRequired();
    }
}
