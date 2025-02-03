using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Setup;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.UnitTests.Fixtures.Entities
{
    public static class OrganisationTestsFixture
    {
        public static List<ChmmRole> GetMockRoles(string name)
        {
            var role = new ChmmRole(name);
            return new List<ChmmRole>() { role };
        }

        public static CreateOrganisationDto GetCreateOrganisationDto(List<CreateOrganisationAddressDto> addresses)
        {
            var createOrganisationDto = new CreateOrganisationDto()
            {
                Addresses = addresses,
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
                IsNonSchemeParticipant = false
            };
            return createOrganisationDto;
        }

        public static CreateOrganisationDto GetCreateOrganisationDto()
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
                    },
                    new()
                    {
                        LineOne = "New test line 2",
                        City = "New test city 2",
                        Postcode = "New test postcode 2",
                        IsUsedAsLegalCorrespondence = true
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
                IsNonSchemeParticipant = false,
                LegalAddressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress
            };
            return createOrganisationDto;
        }

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
                        LineOne = "New test line one",
                        City = "New test city",
                        Postcode = "New test postcode",
                        IsUsedAsLegalCorrespondence = false
                    }
                },
                Users = new List<EditManufacturerUserDto>()
                {
                    new()
                    {
                        Id = userId ?? Guid.NewGuid(),
                        TelephoneNumber = "109876543210",
                        IsResponsibleOfficer = true,
                        Name = "New test name",
                        Email = "new@test",
                        JobTitle = "New test job title"
                    }
                },
                IsOnBehalfOfGroup = true,
                ResponsibleUndertaking = new ResponsibleUndertakingDto()
                {
                    Name = "New test name",
                    CompaniesHouseNumber = "New companies house number",
                },
                IsFossilFuelBoilerSeller = true,
                CreditContactDetails = new CreditContactDetailsDto()
                {
                    Name = "New test name",
                    Email = "new@test",
                    TelephoneNumber = "109876543210",
                },
                IsNonSchemeParticipant = true,
                LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress
            };
            return editOrganisationDto;
        }

        public static EditOrganisationAddressDto GetEditOrganisationAddressDto(bool isUsedAsLegalCorrespondence, Guid? id = null)
        {
            return new EditOrganisationAddressDto()
            {
                Id = id ?? Guid.NewGuid(),
                LineOne = "New test line one",
                City = "New test city",
                Postcode = "New test postcode",
                IsUsedAsLegalCorrespondence = isUsedAsLegalCorrespondence
            };
        }

        public static EditManufacturerUserDto GetEditManufacturerUserDto(Guid? id = null)
        {
            return new EditManufacturerUserDto()
            {
                Id = id ?? Guid.NewGuid(),
                TelephoneNumber = "109876543210",
                IsResponsibleOfficer = true,
                Name = "New test name",
                Email = "new@test",
                JobTitle = "New test job title",
            };
        }
    }
}
