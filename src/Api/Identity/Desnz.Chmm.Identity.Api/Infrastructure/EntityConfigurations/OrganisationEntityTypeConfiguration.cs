using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations
{
    public class OrganisationEntityTypeConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> entity)
        {
            ConfigureCommon(entity);
            ConfigureHeatPumpsProperty(entity);
        }

        protected static void ConfigureCommon(EntityTypeBuilder<Organisation> entity)
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Id).HasColumnType("uuid");
            entity.Property(o => o.CreationDate).HasColumnType("timestamptz").IsRequired();
            entity.Property(p => p.CreatedBy).HasColumnType("varchar(100)").IsRequired();
            entity.Property(o => o.Name).HasColumnType("varchar(100)").IsRequired();
            entity.Property(o => o.IsGroupRegistration).HasColumnType("boolean").IsRequired();
            entity.Property(o => o.IsFossilFuelBoilerSeller).HasColumnType("boolean").IsRequired();
            entity.Property(o => o.IsNonSchemeParticipant).HasColumnType("boolean").IsRequired();
            entity.Property(o => o.LegalAddressType).HasColumnType("varchar(100)").IsRequired();
            entity.Property(o => o.Status).HasColumnType("varchar(100)").IsRequired();
            entity.Property(u => u.ResponsibleOfficerId).HasColumnType("uuid").IsRequired();
            entity.Property(u => u.ApplicantId).HasColumnType("uuid").IsRequired();
            entity.Property(o => o.CompaniesHouseNumber).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(o => o.ContactName).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(o => o.ContactEmail).HasColumnType("varchar(100)").IsRequired(false);
            entity.Property(o => o.ContactTelephoneNumber).HasColumnType("varchar(100)").IsRequired(false);

            entity.HasIndex(u => u.Name).IsUnique(false);

            entity.HasMany(o => o.Addresses).WithOne(a => a.Organisation).HasForeignKey(a => a.OrganisationId).IsRequired();
            entity.HasMany(o => o.OrganisationStructureFiles).WithOne(a => a.Organisation).HasForeignKey(a => a.OrganisationId).IsRequired();
            entity.HasMany(o => o.OrganisationDecisionFiles).WithOne(a => a.Organisation).HasForeignKey(a => a.OrganisationId).IsRequired();
            entity.HasMany(o => o.Comments).WithOne(c => c.Organisation).HasForeignKey(c => c.OrganisationId).IsRequired();
            entity.HasMany(o => o.ChmmUsers).WithOne(u => u.Organisation).HasForeignKey(u => u.OrganisationId).IsRequired(false);
            entity.HasMany(o => o.LicenceHolderLinks).WithOne(a => a.Organisation).HasForeignKey(a => a.OrganisationId).IsRequired(false);
        }

        protected virtual void ConfigureHeatPumpsProperty(EntityTypeBuilder<Organisation> entity)
        {
            entity.Property(p => p.HeatPumpBrands).HasColumnType("varchar(100)[]").IsRequired(false);
        }
    }
}
