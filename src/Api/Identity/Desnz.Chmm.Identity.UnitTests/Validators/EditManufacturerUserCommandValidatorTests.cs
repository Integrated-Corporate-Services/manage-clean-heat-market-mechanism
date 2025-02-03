using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Common.Services;
using Moq;
using System.Security.Principal;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class EditManufacturerUserCommandValidatorTests
    {
        private readonly EditManufacturerUserCommandValidator _validator;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GenericIdentity _identity;

        public EditManufacturerUserCommandValidatorTests()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
            var organisationId = organisation.Id;
            var role = new ChmmRole("Manufacturer");

            var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
            var user = new ChmmUser(dto, new List<ChmmRole> { role }, organisationId);

            user.Activate();
            _identity = new GenericIdentity("some name", "test");
            _identity.AddClaim(new Claim(ClaimTypes.Role, "Manufacturer"));
            _identity.AddClaim(new Claim(Claims.OrganisationId, Guid.NewGuid().ToString()));
            _identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()));
            var contextUser = new ClaimsPrincipal(_identity);
            var context = new DefaultHttpContext() { User = contextUser };
            _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(context.User);

            _validator = new EditManufacturerUserCommandValidator(_mockCurrentUserService.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Id_IsEmpty(string inString)
        {
            var command = new EditManufacturerUserCommand();
            command.Name = "A name";

            Guid guidFromString;
            var guid = Guid.TryParse(inString, out guidFromString);

            command.Id = guidFromString;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Id);
        }

        #region Name
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = new EditManufacturerUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = new EditManufacturerUserCommand();
            command.Name = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData("Joe Bloggs")]
        [InlineData("Jean-Francois Champollion")]
        [InlineData("Alex O'Neil")]
        public void Should_Not_Have_Error_When_Name(string name)
        {
            var command = new EditManufacturerUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region JobTitle tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_JobTitle_IsEmpty(string jobTitle)
        {
            var command = new EditManufacturerUserCommand();
            command.JobTitle = jobTitle;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.JobTitle);
        }

        [Fact]
        public void Should_Have_Error_When_JobTitleLength_Exceeds_Maximum()
        {
            var command = new EditManufacturerUserCommand();
            command.JobTitle = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.JobTitle)
                .WithErrorMessage("The JobTitle field has a maximum length of 100 characters");
        }

        [Theory]
        [InlineData("IT Consultant")]
        [InlineData("MD")]
        [InlineData("Secretary")]
        public void Should_Not_Have_Error_When_JobTitle(string jobTitle)
        {
            var command = new EditManufacturerUserCommand();
            command.JobTitle = jobTitle;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.JobTitle);
        }
        #endregion

        #region TelephoneNumber tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_TelephoneNumber_IsEmpty(string telephoneNumber)
        {
            var command = new EditManufacturerUserCommand();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Fact]
        public void Should_Have_Error_When_TelephoneNumberLength_Exceeds_Maximum()
        {
            var command = new EditManufacturerUserCommand();
            command.TelephoneNumber = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData("0202222222")]
        [InlineData("004402033334444")]
        public void Should_Not_Have_Error_When_TelephoneNumber(string telephoneNumber)
        {
            var command = new EditManufacturerUserCommand();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.TelephoneNumber);
        }


        [Theory]
        [InlineData("111o1")]
        [InlineData("11-11")]
        public void Should_Have_Error_When_TelephoneNumber_Is_Invalid(string telephoneNumber)
        {
            var command = new EditManufacturerUserCommand();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }
        #endregion


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_OrganisationId_IsEmpty(string inString)
        {
            Guid guidFromString;
            var guid = Guid.TryParse(inString, out guidFromString);

            var command = new EditManufacturerUserCommand();
            command.Id = Guid.NewGuid();
            command.TelephoneNumber = "123";
            command.Name = "Name";
            command.JobTitle = "JobTitle";
            command.OrganisationId = guidFromString;

            var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
            var organisationId = organisation.Id;
            var role = new ChmmRole("Manufacturer");

            var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
            var user = new ChmmUser(dto, new List<ChmmRole> { role }, organisationId);

            user.Activate();
            _identity = new GenericIdentity("some name", "test");
            _identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
            _identity.AddClaim(new Claim(Claims.OrganisationId, Guid.NewGuid().ToString()));
            _identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()));
            var contextUser = new ClaimsPrincipal(_identity);
            var context = new DefaultHttpContext() { User = contextUser };
            _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(context.User);

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
        }

        private static CreateOrganisationDto GetCreateOrganisationDto()
        {
            var editOrganisationDto = new CreateOrganisationDto()
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
            return editOrganisationDto;
        }
    }
}