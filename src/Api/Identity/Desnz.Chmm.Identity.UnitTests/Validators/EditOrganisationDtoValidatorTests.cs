using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class EditOrganisationDtoValidatorTests
    {
        private readonly EditOrganisationDtoValidator _validator;
        private readonly EditOrganisationDto _command;

        public EditOrganisationDtoValidatorTests()
        {
            _command = new EditOrganisationDto();
            _command.ResponsibleUndertaking = new Common.Dtos.ResponsibleUndertakingDto { Name = "z" };
            _command.Addresses = new List<EditOrganisationAddressDto> { new EditOrganisationAddressDto { LineOne = "z", City = "z", Postcode = "z" } };
            _command.Users = new List<EditManufacturerUserDto> { new EditManufacturerUserDto { Name = "z", Email = "z@z", TelephoneNumber = "0" } };
            _command.CreditContactDetails = new Common.Dtos.CreditContactDetailsDto() { Name = "z", Email = "z@z", TelephoneNumber = "0" };

            _validator = new EditOrganisationDtoValidator();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Id_IsEmpty(string inString)
        {
            Guid guidFromString;
            var guid = Guid.TryParse(inString, out guidFromString);

            _command.Id = guidFromString;
            var result = _validator.TestValidate(_command);
            result
                .ShouldHaveValidationErrorFor(c => c.Id);
        }

        #region Flags
        [Fact]
        public void Should_Not_Have_Error_When_IsOnBehalfOfGroup_IsEmpty()
        {
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.IsOnBehalfOfGroup);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsFossilFuelBoilerSeller_IsEmpty()
        {
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.IsFossilFuelBoilerSeller);
        }

        #endregion

        #region Addresses
        [Fact]
        public void Should_Have_Error_When_Addresses_IsEmpty()
        {
            _command.Addresses = new List<EditOrganisationAddressDto>();
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.Addresses);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Addresses_IsNotEmpty()
        {
            _command.Addresses = new List<EditOrganisationAddressDto>() { new EditOrganisationAddressDto() };
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.Addresses);
        }

        [Fact]
        public void Test_Addresses_Validator_IsPresent()
        {
            _validator.ShouldHaveChildValidator(i => i.Addresses, typeof(OrganisationAddressDtoValidator));
        }
        #endregion

        #region Users
        [Fact]
        public void Should_Have_Error_When_Users_IsEmpty()
        {
            _command.Users = new List<EditManufacturerUserDto>();
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.Users);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Users_IsNotEmpty()
        {
            _command.Users = new List<EditManufacturerUserDto>() { new EditManufacturerUserDto { Email = "email@example.com" } };
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.Users);
        }
        
        [Fact]
        public void Test_Users_Validator_IsPresent()
        {
            _validator.ShouldHaveChildValidator (i => i.Users, typeof(EditManufacturerUserDtoValidator));
        }
        #endregion

        #region ResponsibleUndertaking
        [Fact]
        public void Should_Have_Error_When_ResponsibleUndertaking_IsEmpty()
        {
            _command.ResponsibleUndertaking = null;
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.ResponsibleUndertaking);
        }

        [Fact]
        public void Test_ResponsibleUndertaking_Validator_IsPresent()
        {
            _validator.ShouldHaveChildValidator(i => i.ResponsibleUndertaking, typeof(ResponsibleUndertakingDtoValidator));
        }
        #endregion

        #region CreditContactDetails
        [Fact]
        public void Should_Have_Error_When_CreditContactDetails_IsEmpty()
        {
            _command.CreditContactDetails = null;
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.CreditContactDetails);
        }

        [Fact]
        public void Test_CreditContactDetails_Validator_IsPresent()
        {
            _validator.ShouldHaveChildValidator(i => i.CreditContactDetails, typeof(CreditContactDetailsDtoValidator));
        }
        #endregion

        [Fact]
        public void Should_Not_Have_MissingProperty_Error_When_AllMandatory_Properties_ArePresent()
        {
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.ResponsibleUndertaking);
        }
    }
}