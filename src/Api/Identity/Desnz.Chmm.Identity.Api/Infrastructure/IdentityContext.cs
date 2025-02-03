using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Identity.Api.Infrastructure;

public class IdentityContext : EntityDbContext, IUnitOfWork
{
    public DbSet<ChmmUser> ChmmUsers { get; set; }
    public DbSet<ChmmRole> ChmmRoles { get; set; }
    public DbSet<OrganisationAddress> OrganisationAddresss { get; set; }
    public DbSet<OrganisationDecisionComment> OrganisationDecisionComments { get; set; }
    public DbSet<OrganisationStructureFile> OrganisationStructureFiles { get; set; }
    public DbSet<OrganisationDecisionFile> OrganisationDecisionFiles { get; set; }
    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<LicenceHolder> LicenceHolders { get; set; }
    public DbSet<LicenceHolderLink> LicenceHolderLinks { get; set; }

    public IdentityContext(DbContextOptions<IdentityContext> options, ICurrentUserService currentUserService) : base(options, currentUserService)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new ChmmUserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ChmmRoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationAddressEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationDecisionCommentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationStructureFileEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationDecisionFileEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LicenceHolderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LicenceHolderLinkEntityTypeConfiguration());

        modelBuilder.Entity<Organisation>(builder =>
        {
            if (!Database.IsNpgsql())
            {
                modelBuilder.ApplyConfiguration(new OrganisationEntityTypeInMemoryConfiguration());
            }
            else
            {
                modelBuilder.ApplyConfiguration(new OrganisationEntityTypeConfiguration());
            }
        });
    }
}
