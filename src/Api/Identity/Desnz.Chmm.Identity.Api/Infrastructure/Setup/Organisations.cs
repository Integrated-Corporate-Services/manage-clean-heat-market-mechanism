using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Setup
{
    public class Organisations
    {
        public static CreateOrganisationDto GetOrganisationDto(int idx, List<CreateOrganisationAddressDto> addresses, List<CreateManufacturerUserDto> users)
        {
            return new CreateOrganisationDto()
            {
                IsOnBehalfOfGroup = true,
                ResponsibleUndertaking = new()
                {
                    Name = $"Test Organisation {idx}",
                    CompaniesHouseNumber = $"0228504{idx}"
                },
                LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress,
                Addresses = addresses,
                IsFossilFuelBoilerSeller = true,
                HeatPumpBrands = new[] { "Worcester Bosch", "Viessmann" },
                Users = users,
                CreditContactDetails = new()
                {
                    Name = "Test Contact",
                    Email = "test.contact@example.com",
                    TelephoneNumber = "01908278450"
                }
            };
        }

        public static List<CreateOrganisationAddressDto> Addresses = new()
        {
            new()
            {
                LineOne = "Weyside Park, Catteshall Lane",
                City = "Godalming",
                County = "Surrey",
                Postcode = "GU7 1XE",
                IsUsedAsLegalCorrespondence = true,
            },
            new()
            {
                LineOne = "Weyside Park, Catteshall Lane",
                City = "Godalming",
                County = "Surrey",
                Postcode = "GU7 1XE",
            }
        };
    }
}
