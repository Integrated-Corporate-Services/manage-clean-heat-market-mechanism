using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class CreditContactDetailsDtoValidatorTests
    {
        private readonly CreditContactDetailsDtoValidator _validator;

        public CreditContactDetailsDtoValidatorTests()
        {
            _validator = new CreditContactDetailsDtoValidator();
        }

        #region Name
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Not_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = GeCreditContactDetailsDto();
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
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region Email
        [Theory]
        [InlineData(null, "z")]
        [InlineData("", "z")]
        public void Should_Not_Have_Error_When_Email_IsEmpty(string name, string number)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            command.TelephoneNumber = number;
            command.Email = null;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("z", null)]
        [InlineData("z", "")]
        public void Should_Have_Error_When_EmailLength_Exceeds_Maximum(string name, string number)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            command.TelephoneNumber = number;
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
            var command = GeCreditContactDetailsDto();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }
        #endregion

        #region TelephoneNumber
        [Theory]
        [InlineData(null, "z")]
        [InlineData("", "z")]
        public void Should_Not_Have_Error_When_TelephoneNumber_IsEmpty(string name, string number)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            command.TelephoneNumber = number;
            command.TelephoneNumber = null;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData("z", null)]
        [InlineData("z", "")]
        public void Should_Have_Error_When_TelephoneNumberLength_Exceeds_Maximum(string name, string email)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            command.Email = email;
            command.TelephoneNumber = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData(null, "z@a")]
        [InlineData("", "z@a")]
        public void Should_Not_Have_Error_When_TelephoneNumber_IsDigitsOnly(string name, string email)
        {
            var command = GeCreditContactDetailsDto();
            command.Name = name;
            command.Email = email;
            command.TelephoneNumber = "111";
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.TelephoneNumber);
        }
        #endregion

        private CreditContactDetailsDto GeCreditContactDetailsDto()
        {
            return new CreditContactDetailsDto();
        }
    }
}