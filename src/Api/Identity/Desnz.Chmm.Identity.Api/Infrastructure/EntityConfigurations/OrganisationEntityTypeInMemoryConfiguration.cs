using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationEntityTypeInMemoryConfiguration : OrganisationEntityTypeConfiguration, IEntityTypeConfiguration<Organisation>
    {
        public new void Configure(EntityTypeBuilder<Organisation> entity)
        {
            ConfigureCommon(entity);
            ConfigureHeatPumpsProperty(entity);
        }

        protected override void ConfigureHeatPumpsProperty(EntityTypeBuilder<Organisation> entity)
        {
            entity.Property(p => p.HeatPumpBrands).HasConversion(
                        v => string.Join("'", v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
