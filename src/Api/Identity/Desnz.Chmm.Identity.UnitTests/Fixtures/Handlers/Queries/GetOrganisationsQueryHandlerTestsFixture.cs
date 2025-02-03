using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Queries
{
    public static  class GetOrganisationsQueryHandlerTestsFixture
    {
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
                CreditContactDetails = new CreditContactDetailsDto {
                    Name = "Contact Name", 
                    Email = "contact@example.com", 
                    TelephoneNumber = "4392048243098"
                },
            };
            var roles = new List<ChmmRole>() { new ChmmRole(Roles.Manufacturer) };

            var organisation = new Organisation(createOrganisationDto, roles);
            return organisation;
        }

        public static HttpContext GetMockContext()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Roles.PrincipalTechnicalOfficer)
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return httpContext;
        }
    }
}
