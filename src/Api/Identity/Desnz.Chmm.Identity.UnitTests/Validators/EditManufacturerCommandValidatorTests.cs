using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class EditManufacturerCommandValidatorTests
    {
        private readonly EditManufacturerCommandValidator _validator;
        private readonly EditManufacturerCommand _command;

        public EditManufacturerCommandValidatorTests()
        {
            _command = new EditManufacturerCommand();
            _validator = new EditManufacturerCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("z")]
        public void Should_Have_Error_When_OrganisationDetailsJson_IsEmpty(string inString)
        {
            _command.OrganisationDetailsJson = inString;
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.OrganisationDetailsJson);
        }

        [Fact]
        public void Should_Not_Have_Error_When_OrganisationDetailsJson_IsEmpty()
        {
            var applicant = new Common.Dtos.ManufacturerUser.CreateManufacturerUserDto { Name = "Simon Blacksmith", Email = "Simon.Blacksmith@example.com", TelephoneNumber = "0", IsResponsibleOfficer = false };
            var responsibleOfficer = new Common.Dtos.ManufacturerUser.CreateManufacturerUserDto { Name = "Joe Bloggs", Email = "joe.bloggs@example.com", TelephoneNumber = "0", IsResponsibleOfficer = true, ResponsibleOfficerOrganisationName = "z" };
            var dto = new CreateOrganisationDto()
            {
                IsOnBehalfOfGroup = true,
                ResponsibleUndertaking = new()
                {
                    Name = $"Test Organisation",
                    CompaniesHouseNumber = $"0228504"
                },
                Addresses = new List<Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto>
                { 
                    new Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto{ LineOne = "z", City = "z", Postcode = "z"}
                },
                IsFossilFuelBoilerSeller = true,
                HeatPumpBrands = new[] { "Worcester-Bosch", "Viessmann" },
                Users = new List<Common.Dtos.ManufacturerUser.CreateManufacturerUserDto>
                {
                    applicant, responsibleOfficer
                },
                CreditContactDetails = new()
                {
                    Name = "Test Contact",
                    Email = "test.contact@example.com",
                    TelephoneNumber = "01908278450"
                }
            };

            _command.OrganisationDetailsJson = System.Text.Json.JsonSerializer.Serialize(dto);
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.OrganisationDetailsJson);
        }

        [Fact]
        public void Should_Have_Error_When_ChildProperty_Has_Missing_Property()
        {
            var applicant = new Common.Dtos.ManufacturerUser.EditManufacturerUserDto { Id = Guid.NewGuid(), Name = "Simon Blacksmith", JobTitle = "z", Email = "Simon.Blacksmith@example.com", TelephoneNumber = "0", IsResponsibleOfficer = false };
            var responsibleOfficer = new Common.Dtos.ManufacturerUser.EditManufacturerUserDto {Id = Guid.NewGuid(), Name = "Joe Bloggs", JobTitle = "z", Email = "joe.bloggs@example.com", TelephoneNumber = "0", IsResponsibleOfficer = true, ResponsibleOfficerOrganisationName = "z" };
            var dto = new EditOrganisationDto()
            {
                Id = Guid.NewGuid(),
                IsOnBehalfOfGroup = true,
                ResponsibleUndertaking = new()
                {
                    //--->>> Missing required property:
                    //Name = $"Test Organisation",
                    CompaniesHouseNumber = $"0228504"
                },
                Addresses = new List<Common.Dtos.OrganisationAddress.EditOrganisationAddressDto>
                {
                    new Common.Dtos.OrganisationAddress.EditOrganisationAddressDto{ Id = Guid.NewGuid(), LineOne = "z", City = "z", Postcode = "z"}
                },
                IsFossilFuelBoilerSeller = true,
                HeatPumpBrands = new[] { "Worcester-Bosch", "Viessmann" },
                Users = new List<Common.Dtos.ManufacturerUser.EditManufacturerUserDto>
                {
                    applicant, responsibleOfficer
                },
                CreditContactDetails = new()
                {
                    Name = "Test Contact",
                    Email = "test.contact@example.com",
                    TelephoneNumber = "01908278450"
                }
            };

            _command.OrganisationDetailsJson = System.Text.Json.JsonSerializer.Serialize(dto);
            var result = _validator.TestValidate(_command);
            Assert.Equal(1, result.Errors.Count);
            Assert.Equal("ResponsibleUndertaking.Name", result.Errors[0].PropertyName);
        }
    }
}