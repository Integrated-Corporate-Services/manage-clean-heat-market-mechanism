using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class CreateManufacturerUserDtoValidatorTests
    {
        private readonly CreateManufacturerUserDtoValidator _validator;

        public CreateManufacturerUserDtoValidatorTests()
        {
            _validator = new CreateManufacturerUserDtoValidator(2);
        }

        #region Name tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = GetCreateChmmUserCommandBase();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage("Name is a required field");
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = GetCreateChmmUserCommandBase();
            command.Name = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage("The Name field has a maximum length of 100 characters");
        }

        [Theory]
        [InlineData("Joe Bloggs")]
        [InlineData("Jean-Francois Champollion")]
        [InlineData("Alex O'Neil")]
        public void Should_Not_Have_Error_When_Name(string name)
        {
            var command = GetCreateChmmUserCommandBase();
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
            var command = GetCreateChmmUserCommandBase();
            command.JobTitle = jobTitle;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.JobTitle);
        }

        [Fact]
        public void Should_Have_Error_When_JobTitleLength_Exceeds_Maximum()
        {
            var command = GetCreateChmmUserCommandBase();
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
            var command = GetCreateChmmUserCommandBase();
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
            var command = GetCreateChmmUserCommandBase();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber).WithErrorMessage("TelephoneNumber is a required field");
        }

        [Fact]
        public void Should_Have_Error_When_TelephoneNumberLength_Exceeds_Maximum()
        {
            var command = GetCreateChmmUserCommandBase();
            command.TelephoneNumber = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber)
                .WithErrorMessage("The TelephoneNumber field has a maximum length of 100 characters");
        }

        [Theory]
        [InlineData("020222222")]
        [InlineData("004402033334444")]
        [InlineData("111")]
        public void Should_Not_Have_Error_When_TelephoneNumber(string telephoneNumber)
        {
            var command = GetCreateChmmUserCommandBase();
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
            var command = GetCreateChmmUserCommandBase();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber)
                .WithErrorMessage("The TelephoneNumber field should be numeric");
        }
        #endregion

        #region ResponsibleOfficerOrganisationName tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_ResponsibleOfficerOrganisationName_IsEmpty(string responsibleOfficerOrganisationName)
        {
            var command = GetCreateChmmUserCommandBase();
            command.IsResponsibleOfficer = false;
            command.ResponsibleOfficerOrganisationName = responsibleOfficerOrganisationName;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName);
        }

        [Fact]
        public void Should_Have_Error_When_ResponsibleOfficerOrganisationNameLength_Exceeds_Maximum()
        {
            var command = GetCreateChmmUserCommandBase();
            command.Name = "A name";
            command.Email = "email";
            command.IsResponsibleOfficer = true;
            command.ResponsibleOfficerOrganisationName = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName)
                .WithErrorMessage("The ResponsibleOfficerOrganisationName field has a maximum length of 100 characters");
        }

        [Theory]
        [InlineData("Their Limited Company Plc")]
        [InlineData("Apple")]
        [InlineData("Orange 99")]
        [InlineData("X")]
        public void Should_Not_Have_Error_When_ResponsibleOfficerOrganisationName(string responsibleOfficerOrganisationName)
        {
            var command = GetCreateChmmUserCommandBase();
            command.IsResponsibleOfficer = true;
            command.ResponsibleOfficerOrganisationName = responsibleOfficerOrganisationName;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName);
        }
        #endregion

        private CreateManufacturerUserDto GetCreateChmmUserCommandBase()
        {
            return new CreateManufacturerUserDto {
                        Email = "email@example.com"
            };
        }
    }
}