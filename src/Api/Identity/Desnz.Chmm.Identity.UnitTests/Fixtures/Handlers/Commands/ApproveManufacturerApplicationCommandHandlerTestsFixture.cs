using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Commands
{
    public static class ApproveManufacturerApplicationCommandHandlerTestsFixture
    {
        public static EditOrganisationDto GetEditOrganisationDto(Guid? addressId = null, Guid? userId = null)
        {
            var editOrganisationDto = new EditOrganisationDto()
            {
                Id = Guid.NewGuid(),
                Addresses = new List<EditOrganisationAddressDto>()
                {
                    new()
                    {
                        Id = addressId ?? Guid.NewGuid(),
                        LineOne = "Test line one",
                        City = "Test city",
                        Postcode = "Test postcode",
                        IsUsedAsLegalCorrespondence = false
                    }
                },
                Users = new List<EditManufacturerUserDto>()
                {
                    new()
                    {
                        Id = userId ?? Guid.NewGuid(),
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Test name",
                        Email = "test@test",
                        JobTitle = "Test job title"
                    }
                },
                IsOnBehalfOfGroup = false,
                ResponsibleUndertaking = new ResponsibleUndertakingDto()
                {
                    Name = "Test name",
                },
                IsFossilFuelBoilerSeller = false,
                CreditContactDetails = new CreditContactDetailsDto(),
            };
            return editOrganisationDto;
        }

        public static Organisation GetMockOrganisation()
        {
            var createOrganisationDto = new CreateOrganisationDto()
            {
                Addresses = new List<CreateOrganisationAddressDto>()
                {
                    new()
                    {
                        LineOne = "Test line one",
                        City = "Test city",
                        Postcode = "Test postcode",
                        IsUsedAsLegalCorrespondence = false
                    }
                },
                Users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Test name",
                        Email = "test@test",
                        JobTitle = "Test job title"
                    }
                },
                IsOnBehalfOfGroup = false,
                ResponsibleUndertaking = new ResponsibleUndertakingDto()
                {
                    Name = "Test name",
                },
                IsFossilFuelBoilerSeller = false,
                CreditContactDetails = new CreditContactDetailsDto(),
            };
            var roles = new List<ChmmRole>() { new ChmmRole(IdentityConstants.Roles.Manufacturer) };
            
            var organisation = new Organisation(createOrganisationDto, roles);
            return organisation;
        }

        public static ChmmUser GetMockAdminUser()
        {
            return new ChmmUser("Test Name", "test@test", new List<ChmmRole>());
        }

        public static ClaimsPrincipal GetMockUser(string email)
        {
            var httpContext = new DefaultHttpContext();
            var adminEmail = email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, adminEmail),
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString())
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return httpContext.User;
        }
    }
}
