using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using FluentAssertions;
using Xunit;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Entities.OrganisationTestsFixture;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Infrastructure.Setup;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.UnitTests.Entities
{
    public class OrganisationTests
    {
        public OrganisationTests() { }

        [Fact]
        public void ShouldCreateOrganisation()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();
            var addressDto = createOrganisationDto.Addresses.First();
            var userDto = createOrganisationDto.Users.Single();

            // Act
            var result = new Organisation(createOrganisationDto, roles);

            // Assert
            var orgId = result.Id;
            orgId.Should().NotBeEmpty();
            result.CreationDate.Should().NotBe(default);
            result.Name.Should().Be(createOrganisationDto.ResponsibleUndertaking.Name);
            result.CompaniesHouseNumber.Should().Be(createOrganisationDto.ResponsibleUndertaking.CompaniesHouseNumber);
            result.IsGroupRegistration.Should().Be(createOrganisationDto.IsOnBehalfOfGroup);
            result.IsFossilFuelBoilerSeller.Should().Be(createOrganisationDto.IsFossilFuelBoilerSeller);
            result.IsNonSchemeParticipant.Should().Be(createOrganisationDto.IsNonSchemeParticipant);
            result.LegalAddressType.Should().Be(createOrganisationDto.LegalAddressType);
            result.ContactName.Should().Be(createOrganisationDto.CreditContactDetails.Name);
            result.ContactEmail.Should().Be(createOrganisationDto.CreditContactDetails.Email);
            result.ContactTelephoneNumber.Should().Be(createOrganisationDto.CreditContactDetails.TelephoneNumber);
            result.Status.Should().Be(Common.Constants.OrganisationConstants.Status.Pending);
            result.ApplicantId.Should().Be(result.ApplicantId);
            result.ResponsibleOfficerId.Should().Be(result.ApplicantId);

            var address = result.Addresses.First();
            address.Id.Should().NotBeEmpty();
            address.CreationDate.Should().NotBe(default);
            address.Type.Should().Be(OrganisationAddressConstants.Type.OfficeAddress);
            address.LineOne.Should().Be(addressDto.LineOne);
            address.LineTwo.Should().Be(addressDto.LineTwo);
            address.City.Should().Be(addressDto.City);
            address.County.Should().Be(addressDto.County);
            address.PostCode.Should().Be(addressDto.Postcode);
            address.PostCode.Should().Be(addressDto.Postcode);

            var user = result.ChmmUsers.Single();
            user.Id.Should().NotBeEmpty();
            user.CreationDate.Should().NotBe(default);
            user.Name.Should().Be(userDto.Name);
            user.Email.Should().Be(userDto.Email);
            user.JobTitle.Should().Be(userDto.JobTitle);
            user.TelephoneNumber.Should().Be(userDto.TelephoneNumber);
            user.ResponsibleOfficerOrganisationName.Should().Be(userDto.ResponsibleOfficerOrganisationName);
            user.Status.Should().Be(UsersConstants.Status.Inactive);

            result.Comments.Should().BeNull();
        }

        [Fact]
        public void ShouldUpdateOrganisationDetailsWithoutComment()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var expectedOrgId = organisation.Id;
            var expectedAplicantId = organisation.ApplicantId;
            var expectedCreationDate = organisation.CreationDate;

            var address = organisation.Addresses.First();
            var expectedAddressId = address.Id;
            var expectedAddressCreationDate = address.CreationDate;

            var user = organisation.ChmmUsers.Single();
            var expectedUserId = user.Id;
            var expectedUserCreationDate = user.CreationDate;

            var editOrganisationDto = GetEditOrganisationDto(address.Id, user.Id);

            // Act
            organisation.UpdateOrganisationDetails(editOrganisationDto);

            // Assert
            AssertOrganisationDetails(organisation, expectedOrgId, expectedCreationDate, expectedAplicantId, 
                editOrganisationDto,expectedAddressId, expectedAddressCreationDate, expectedUserId, expectedUserCreationDate);
            organisation.Comments.Should().BeNull();
        }

        [Fact]
        public void ShouldSetResponsibleUndertakingDetails()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var responsibleUndertakingDto = new ResponsibleUndertakingDto()
            {
                Name = "New name",
                CompaniesHouseNumber = "New companies house number",
            };

            // Act
            organisation.SetResponsibleUndertakingDetails(responsibleUndertakingDto);

            // Assert
            organisation.Name.Should().Be(responsibleUndertakingDto.Name);
            organisation.CompaniesHouseNumber.Should().Be(responsibleUndertakingDto.CompaniesHouseNumber);
        }

        [Fact]
        public void ShouldSetSetContactDetails()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var creditContactDetailsDto = new CreditContactDetailsDto()
            {
                Name = "New name",
                Email = "new@test",
                TelephoneNumber = "109876543210"
            };

            // Act
            organisation.SetContactDetails(creditContactDetailsDto);

            // Assert
            organisation.ContactName.Should().Be(creditContactDetailsDto.Name);
            organisation.ContactEmail.Should().Be(creditContactDetailsDto.Email);
            organisation.ContactTelephoneNumber.Should().Be(creditContactDetailsDto.TelephoneNumber);
        }

        [Fact]
        public void ShouldActivate()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            // Act
            organisation.Activate();

            // Assert
            organisation.Status.Should().Be(Common.Constants.OrganisationConstants.Status.Active);
        }

        [Fact]
        public void ShouldUpdateAddressesOk()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var address = organisation.Addresses.Single(x=> x.Type == OrganisationAddressConstants.Type.OfficeAddress);
            var expectedAddressId = address.Id;
            var expectedAddressCreationDate = address.CreationDate;

            var editOrganisationAddressDto = GetEditOrganisationAddressDto(false, expectedAddressId);
            var editOrganisationAddress2Dto = GetEditOrganisationAddressDto(true);

            // Act
            organisation.UpdateAddresses(new List<EditOrganisationAddressDto>() { editOrganisationAddressDto, editOrganisationAddress2Dto });

            // Assert
            address.Id.Should().Be(expectedAddressId);
            address.CreationDate.Should().Be(expectedAddressCreationDate);
            address.Type.Should().Be(OrganisationAddressConstants.Type.OfficeAddress);
            address.LineOne.Should().Be(editOrganisationAddressDto.LineOne);
            address.LineTwo.Should().Be(editOrganisationAddressDto.LineTwo);
            address.City.Should().Be(editOrganisationAddressDto.City);
            address.County.Should().Be(editOrganisationAddressDto.County);
            address.PostCode.Should().Be(editOrganisationAddressDto.Postcode);
        }

        [Fact]
        public void UpdateAddressesShouldThrowException_When_AddressNotFoundById()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var address = organisation.Addresses.First();
            var expectedAddressId = address.Id;
            var expectedAddressCreationDate = address.CreationDate;

            var editOrganisationAddressDto = GetEditOrganisationAddressDto(true);

            // Act
            Assert.Throws<InvalidOperationException>(() => 
                organisation.UpdateAddresses(new List<EditOrganisationAddressDto>() { editOrganisationAddressDto }));
        }

        [Fact]
        public void ShouldUpdateUsersOk()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var user = organisation.ChmmUsers.Single();
            var expectedUserId = user.Id;
            var expectedUserCreationDate = user.CreationDate;

            var editManufacturerUserDto = GetEditManufacturerUserDto(expectedUserId);

            // Act
            organisation.UpdateUsers(new List<EditManufacturerUserDto>() { editManufacturerUserDto });

            // Assert
            user.Id.Should().Be(expectedUserId);
            user.CreationDate.Should().Be(expectedUserCreationDate);
            user.Name.Should().Be(editManufacturerUserDto.Name);
            user.Email.Should().Be(editManufacturerUserDto.Email);
            user.JobTitle.Should().Be(editManufacturerUserDto.JobTitle);
            user.TelephoneNumber.Should().Be(editManufacturerUserDto.TelephoneNumber);
            user.ResponsibleOfficerOrganisationName.Should().Be(editManufacturerUserDto.ResponsibleOfficerOrganisationName);
            user.Status.Should().Be(UsersConstants.Status.Inactive);
        }

        [Fact]
        public void UpdateUsers_ShouldThrowException_When_UserNotFoundById()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);
            var user = organisation.ChmmUsers.Single();
            var expectedUserId = user.Id;
            var expectedUserCreationDate = user.CreationDate;

            var editManufacturerUserDto = GetEditManufacturerUserDto();

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                organisation.UpdateUsers(new List<EditManufacturerUserDto>() { editManufacturerUserDto }));
        }

        private void AssertOrganisationDetails(Organisation organisation, Guid expectedOrgId, DateTime expectedCreationDate, 
            Guid expectedAplicantId, EditOrganisationDto editOrganisationDto,Guid expectedAddressId, DateTime expectedAddressCreationDate,
            Guid expectedUserId, DateTime expectedUserCreationDate)
        {
            var addressDto = editOrganisationDto.Addresses.First();
            var userDto = editOrganisationDto.Users.Single();

            organisation.Id.Should().Be(expectedOrgId);
            organisation.CreationDate.Should().Be(expectedCreationDate);
            organisation.Name.Should().Be(editOrganisationDto.ResponsibleUndertaking.Name);
            organisation.CompaniesHouseNumber.Should().Be(editOrganisationDto.ResponsibleUndertaking.CompaniesHouseNumber);
            organisation.IsGroupRegistration.Should().Be(editOrganisationDto.IsOnBehalfOfGroup);
            organisation.IsFossilFuelBoilerSeller.Should().Be(editOrganisationDto.IsFossilFuelBoilerSeller);
            organisation.ContactName.Should().Be(editOrganisationDto.CreditContactDetails.Name);
            organisation.ContactEmail.Should().Be(editOrganisationDto.CreditContactDetails.Email);
            organisation.ContactTelephoneNumber.Should().Be(editOrganisationDto.CreditContactDetails.TelephoneNumber);
            organisation.Status.Should().Be(Common.Constants.OrganisationConstants.Status.Pending);
            organisation.ApplicantId.Should().Be(expectedAplicantId);
            organisation.ResponsibleOfficerId.Should().Be(expectedAplicantId);

            var address = organisation.Addresses.First();
            address.Id.Should().Be(expectedAddressId);
            address.CreationDate.Should().Be(expectedAddressCreationDate);
            address.Type.Should().Be(OrganisationAddressConstants.Type.OfficeAddress);
            address.LineOne.Should().Be(addressDto.LineOne);
            address.LineTwo.Should().Be(addressDto.LineTwo);
            address.City.Should().Be(addressDto.City);
            address.County.Should().Be(addressDto.County);
            address.PostCode.Should().Be(addressDto.Postcode);

            var user = organisation.ChmmUsers.Single();
            user.Id.Should().Be(expectedUserId);
            user.CreationDate.Should().Be(expectedUserCreationDate);
            user.Name.Should().Be(userDto.Name);
            user.Email.Should().Be(userDto.Email);
            user.JobTitle.Should().Be(userDto.JobTitle);
            user.TelephoneNumber.Should().Be(userDto.TelephoneNumber);
            user.ResponsibleOfficerOrganisationName.Should().Be(userDto.ResponsibleOfficerOrganisationName);
            user.Status.Should().Be(UsersConstants.Status.Inactive);
        }


        [Fact]
        public void Should_UpdateCreditContactDetails()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New name";
            var email = "new@test";
            var telephoneNumber = "109876543210";

            // Act
            organisation.UpdateCreditContactDetails(name, email, telephoneNumber);

            // Assert
            organisation.ContactName.Should().Be(name);
            organisation.ContactEmail.Should().Be(email);
            organisation.ContactTelephoneNumber.Should().Be(telephoneNumber);
        }

        [Fact]
        public void Should_UpdateDetails()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New name";
            var companiesHouseNumber = "new@test";

            // Act
            organisation.UpdateDetails(name, companiesHouseNumber);

            // Assert
            organisation.Name.Should().Be(name);
            organisation.CompaniesHouseNumber.Should().Be(companiesHouseNumber);
        }

        [Fact]
        public void Should_UpdateFossilFuelSeller()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var fossilFuelSeller = !createOrganisationDto.IsFossilFuelBoilerSeller;

            // Act
            organisation.UpdateFossilFuelSeller(fossilFuelSeller);

            // Assert
            organisation.IsFossilFuelBoilerSeller.Should().Be(fossilFuelSeller);
        }

        [Fact]
        public void Should_UpdateHeatPumpDetails()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var heatPumpDetails = new[] {"One", "Two"} ;

            // Act
            organisation.UpdateHeatPumpDetails(heatPumpDetails);

            // Assert
            organisation.HeatPumpBrands.Should().BeEquivalentTo(heatPumpDetails);
        }

        [Fact]
        public void Should_UpdateApplicant()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New name";
            var jobTitle = "New job title";
            var telephoneNumber = "23940823094";

            // Act
            organisation.UpdateApplicant(name, jobTitle, telephoneNumber);

            // Assert
            var applicant = organisation.ChmmUsers.Single(i => i.Id == organisation.ApplicantId);
            applicant.Name.Should().Be(name);
            applicant.JobTitle.Should().Be(jobTitle);
            applicant.TelephoneNumber.Should().Be(telephoneNumber);
        }

        [Fact]
        public void Should_UpdateSeniorResponsibleOfficer_When_TwoUsersAndApplicantNotSRO()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = false,
                        Name = "Applicant",
                        Email = "applicant@test",
                        JobTitle = "Test job title"
                    },
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Responsible Officer",
                        Email = "srp@test",
                        JobTitle = "Test job title"
                    }
                };

            createOrganisationDto.Users = users;

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New SRO";
            var jobTitle = "New SRO";
            var telephoneNumber = "23940823094";

            // Act
            organisation.UpdateSeniorResponsibleOfficerIfExists(name, jobTitle, telephoneNumber);

            // Assert
            var applicant = organisation.ChmmUsers.Single(i => i.Id == organisation.ResponsibleOfficerId);
            applicant.Name.Should().Be(name);
            applicant.JobTitle.Should().Be(jobTitle);
            applicant.TelephoneNumber.Should().Be(telephoneNumber);
        }

        [Fact]
        public void ShouldNot_UpdateSeniorResponsibleOfficer_When_OneUsersAndApplicantIsSRO()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Applicant",
                        Email = "applicant@test",
                        JobTitle = "Test job title"
                    }
                };

            createOrganisationDto.Users = users;

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New SRO";
            var jobTitle = "New SRO";
            var telephoneNumber = "23940823094";

            // Act
            organisation.RemoveResponsibleOfficerIfExists();

            // Assert
            var applicant = organisation.ChmmUsers.Single(i => i.Id == organisation.ResponsibleOfficerId);
            applicant.Name.Should().NotBe(name);
            applicant.JobTitle.Should().NotBe(jobTitle);
            applicant.TelephoneNumber.Should().NotBe(telephoneNumber);
        }

        [Fact]
        public void ShouldNot_UpdateSeniorResponsibleOfficer_When_OneUsersAndApplicanNotSRO()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = false,
                        Name = "Applicant",
                        Email = "applicant@test",
                        JobTitle = "Test job title"
                    },
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Responsible Officer",
                        Email = "srp@test",
                        JobTitle = "Test job title"
                    }
                };

            createOrganisationDto.Users = users;

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New SRO";
            var jobTitle = "New SRO";
            var telephoneNumber = "23940823094";

            // Act
            organisation.UpdateSeniorResponsibleOfficerIfExists(name, jobTitle, telephoneNumber);

            // Assert
            var applicant = organisation.ChmmUsers.Single(i => i.Id == organisation.ResponsibleOfficerId);
            applicant.Name.Should().Be(name);
            applicant.JobTitle.Should().Be(jobTitle);
            applicant.TelephoneNumber.Should().Be(telephoneNumber);
        }

        [Fact]
        public void Should_UpdateSeniorResponsibleOfficer_When_TwoUsersAndApplicantIsSRO()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Applicant",
                        Email = "applicant@test",
                        JobTitle = "Test job title"
                    }
                };

            createOrganisationDto.Users = users;

            var organisation = new Organisation(createOrganisationDto, roles);

            var name = "New SRO";
            var jobTitle = "New SRO";
            var telephoneNumber = "23940823094";

            // Act
            organisation.RemoveResponsibleOfficerIfExists();

            // Assert
            var applicant = organisation.ChmmUsers.Single(i => i.Id == organisation.ResponsibleOfficerId);
            applicant.Name.Should().NotBe(name);
            applicant.JobTitle.Should().NotBe(jobTitle);
            applicant.TelephoneNumber.Should().NotBe(telephoneNumber);
        }

        [Fact]
        public void Should_UpdateLegalCorrespondenceAddress_When_TwoAddressesExist_And_UseSpecifiedAddress()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var addresses = new List<CreateOrganisationAddressDto>()
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
                };
            var createOrganisationDto = GetCreateOrganisationDto(addresses);

            var organisation = new Organisation(createOrganisationDto, roles);

            var lineOne = "New line 1";
            var lineTwo = "New line 2";
            var city = "New city";
            var county = "New county";
            var postcode = "PO24 5SE";

            // Act
            organisation.UpdateLegalCorrespondenceAddress(LegalCorrespondenceAddressType.UseSpecifiedAddress, lineOne, lineTwo, city, county, postcode);

            // Assert
            var address = organisation.Addresses.Single(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            address.LineOne.Should().Be(lineOne);
            address.LineTwo.Should().Be(lineTwo);
            address.City.Should().Be(city);
            address.County.Should().Be(county);
            address.PostCode.Should().Be(postcode);
            organisation.Addresses.Count.Should().Be(2);
        }

        [Fact]
        public void Should_UpdateLegalCorrespondenceAddress_When_TwoAddressesExist_And_NoLegalCorrespondenceAddress()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var addresses = new List<CreateOrganisationAddressDto>()
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
                };
            var createOrganisationDto = GetCreateOrganisationDto(addresses);

            var organisation = new Organisation(createOrganisationDto, roles);

            // Act
            organisation.RemoveLegalCorrespondenceAddressIfExists(LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress);

            // Assert
            var address = organisation.Addresses.SingleOrDefault(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            Assert.Null(address);
            organisation.Addresses.Count.Should().Be(1);
        }

        [Fact]
        public void Should_UpdateLegalCorrespondenceAddress_When_TwoAddressesExist_And_UseRegisteredOffice()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var addresses = new List<CreateOrganisationAddressDto>()
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
                };
            var createOrganisationDto = GetCreateOrganisationDto(addresses);

            var organisation = new Organisation(createOrganisationDto, roles);

            // Act
            organisation.RemoveLegalCorrespondenceAddressIfExists(LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress);

            // Assert
            var address = organisation.Addresses.SingleOrDefault(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            Assert.Null(address);
            organisation.Addresses.Count.Should().Be(1);
        }

        [Fact]
        public void Should_UpdateLegalCorrespondenceAddress_When_OneAddressExist_And_UseSpecifiedAddress()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var addresses = new List<CreateOrganisationAddressDto>()
                {
                    new()
                    {
                        LineOne = "Test line one",
                        City = "Test city",
                        Postcode = "Test postcode",
                        IsUsedAsLegalCorrespondence = true
                    }
                };
            var createOrganisationDto = GetCreateOrganisationDto(addresses);

            var organisation = new Organisation(createOrganisationDto, roles);

            var lineOne = "New line 1";
            var lineTwo = "New line 2";
            var city = "New city";
            var county = "New county";
            var postcode = "PO24 5SE";

            // Act
            organisation.UpdateLegalCorrespondenceAddress(LegalCorrespondenceAddressType.UseSpecifiedAddress, lineOne, lineTwo, city, county, postcode);

            // Assert
            var address = organisation.Addresses.Single(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            address.LineOne.Should().Be(lineOne);
            address.LineTwo.Should().Be(lineTwo);
            address.City.Should().Be(city);
            address.County.Should().Be(county);
            address.PostCode.Should().Be(postcode);
            organisation.Addresses.Count.Should().Be(1);
        }

        [Fact]
        public void Should_UpdateRegisteredOfficeAddress()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var lineOne = "New line 1";
            var lineTwo = "New line 2";
            var city = "New city";
            var county = "New county";
            var postcode = "PO24 5SE";

            // Act
            organisation.UpdateRegisteredOfficeAddress(lineOne, lineTwo, city, county, postcode);

            // Assert
            var address = organisation.Addresses.Single(x => x.Type == OrganisationAddressConstants.Type.OfficeAddress);
            address.LineOne.Should().Be(lineOne);
            address.LineTwo.Should().Be(lineTwo);
            address.City.Should().Be(city);
            address.County.Should().Be(county);
            address.PostCode.Should().Be(postcode);
        }

        [Fact]
        public void Should_UpdateOrganisationStructure()
        {
            // Arrange
            var roles = GetMockRoles(IdentityConstants.Roles.Manufacturer);
            var createOrganisationDto = GetCreateOrganisationDto();

            var organisation = new Organisation(createOrganisationDto, roles);

            var isOnBehalfOfGroup = !organisation.IsGroupRegistration;

            // Act
            organisation.UpdateOrganisationStructure(isOnBehalfOfGroup);

            // Assert
            organisation.IsGroupRegistration.Should().Be(isOnBehalfOfGroup);
        }
    }
}