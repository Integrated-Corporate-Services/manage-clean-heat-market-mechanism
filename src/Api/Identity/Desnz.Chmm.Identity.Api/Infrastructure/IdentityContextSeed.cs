using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.EntityFrameworkCore;
using Desnz.Chmm.Identity.Api.Infrastructure.Setup;

namespace Desnz.Chmm.Identity.Api.Infrastructure;

public class IdentityContextSeed
{
    public async Task SeedAsync(IdentityContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();

            if (!context.ChmmRoles.Any())
            {
                await SeedRoles(context);
            }

            if (!context.ChmmUsers.Any())
            {
                await SeedAdminUsers(context);
            }

            if (!context.Organisations.Any())
            {
                await SeedOrganisations(context);
            }
        }
    }

    private async Task SeedRoles(IdentityContext context)
    {
        var roleNames = new List<string>()
        {
            IdentityConstants.Roles.Manufacturer,
            IdentityConstants.Roles.RegulatoryOfficer,
            IdentityConstants.Roles.SeniorTechnicalOfficer,
            IdentityConstants.Roles.PrincipalTechnicalOfficer
        };
        foreach (var name in roleNames)
        {
            var role = new ChmmRole(name);
            context.Add(role);
        }
        await context.SaveChangesAsync();
    }

    private async Task SeedAdminUsers(IdentityContext context)
    {
        var adminRole = await context.ChmmRoles.SingleAsync(r => r.Name == IdentityConstants.Roles.PrincipalTechnicalOfficer);
        foreach (var adminUser in Users.Admins)
        {
            var user = new ChmmUser(adminUser.Item1, adminUser.Item2, new List<ChmmRole> { adminRole });
            user.Activate();
            context.Add(user);
        }
        await context.SaveChangesAsync();
    }

    private async Task SeedOrganisations(IdentityContext context)
    {
        var manufacturerRole = await context.ChmmRoles.SingleAsync(r => r.Name == IdentityConstants.Roles.Manufacturer);
        var organisationDtos = new List<CreateOrganisationDto>();
        for (var idx = 0; idx < Users.Manufacturers.Count; idx++)
        {
            organisationDtos.Add(Organisations.GetOrganisationDto(
                idx, Organisations.Addresses, Users.Manufacturers[idx]
            ));
        }
        foreach (var organisationDto in organisationDtos)
        {
            var organisation = new Organisation(organisationDto, new List<ChmmRole> { manufacturerRole });
            context.Add(organisation);
        }
        await context.SaveChangesAsync();
    }
}
