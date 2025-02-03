using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class EditManufacturerUserDtoValidatorTests
    {
        private readonly EditManufacturerUserDtoValidator _validator;

        public EditManufacturerUserDtoValidatorTests()
        {
            _validator = new EditManufacturerUserDtoValidator(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Id_IsEmpty(string inString)
        {
            var command = GetEditManufacturerUserDto();
            command.Name = "A name";
            command.Email = "email@example.com";
            command.IsResponsibleOfficer = false;

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
            var command = GetEditManufacturerUserDto();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = GetEditManufacturerUserDto();
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
            var command = GetEditManufacturerUserDto();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region Email
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_IsEmpty(string email)
        {
            var command = GetEditManufacturerUserDto();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Should_Have_Error_When_EmailLength_Exceeds_Maximum()
        {
            var command = GetEditManufacturerUserDto();
            command.Email = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("Joe.Bloggs@example.com")]
        [InlineData("Jean-Francois.Champollion@example.com")]
        [InlineData("AlexO'Neil@example.com")]

        public void Should_Not_Have_Error_When_Email(string email)
        {
            var command = GetEditManufacturerUserDto();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("Joe Bloggs")]
        [InlineData("@example.com")]
        [InlineData("@")]
        public void Should_Have_Error_When_Email(string email)
        {
            var command = GetEditManufacturerUserDto();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }
        #endregion


        #region JobTitle tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_JobTitle_IsEmpty(string jobTitle)
        {
            var command = GetEditManufacturerUserDto();
            command.JobTitle = jobTitle;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.JobTitle);
        }

        [Fact]
        public void Should_Have_Error_When_JobTitleLength_Exceeds_Maximum()
        {
            var command = GetEditManufacturerUserDto();
            command.JobTitle = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.JobTitle);
        }

        [Theory]
        [InlineData("IT Consultant")]
        [InlineData("MD")]
        [InlineData("Secretary")]
        public void Should_Not_Have_Error_When_JobTitle(string jobTitle)
        {
            var command = GetEditManufacturerUserDto();
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
            var command = GetEditManufacturerUserDto();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Fact]
        public void Should_Have_Error_When_TelephoneNumberLength_Exceeds_Maximum()
        {
            var command = GetEditManufacturerUserDto();
            command.TelephoneNumber = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData("020222222")]
        [InlineData("004402033334444")]
        [InlineData("111")]
        public void Should_Not_Have_Error_When_TelephoneNumber(string telephoneNumber)
        {
            var command = GetEditManufacturerUserDto();
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
            var command = GetEditManufacturerUserDto();
            command.TelephoneNumber = telephoneNumber;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }
        #endregion

        #region ResponsibleOfficerOrganisationName tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_ResponsibleOfficerOrganisationName_IsEmpty(string responsibleOfficerOrganisationName)
        {
            var command = GetEditManufacturerUserDto();
            command.Id = Guid.NewGuid();
            command.Name = "A name";
            command.Email = "email@email";
            command.JobTitle = "Test";
            command.TelephoneNumber = "0";
            command.IsResponsibleOfficer = false;
            command.ResponsibleOfficerOrganisationName = responsibleOfficerOrganisationName;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName);
        }

        [Fact]
        public void Should_Have_Error_When_ResponsibleOfficerOrganisationNameLength_Exceeds_Maximum()
        {
            var command = GetEditManufacturerUserDto();
            command.Id = Guid.NewGuid();
            command.Name = "A name";
            command.Email = "email@email";
            command.JobTitle = "Test";
            command.TelephoneNumber = "0";
            command.IsResponsibleOfficer = true;
                        
            command.ResponsibleOfficerOrganisationName = new string('*', 101);

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result
                .ShouldHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName);
        }

        [Theory]
        [InlineData("Their Limited Company Plc")]
        [InlineData("Apple")]
        [InlineData("Orange 99")]
        [InlineData("X")]
        public void Should_Not_Have_Error_When_ResponsibleOfficerOrganisationName(string responsibleOfficerOrganisationName)
        {
            var command = GetEditManufacturerUserDto();
            command.IsResponsibleOfficer = true;
            command.ResponsibleOfficerOrganisationName = responsibleOfficerOrganisationName;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.ResponsibleOfficerOrganisationName);
        }
        #endregion

        private EditManufacturerUserDto GetEditManufacturerUserDto()
        {
            return new EditManufacturerUserDto { Email = "email@example.com" };
        }
    }
}