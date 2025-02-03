using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class OrganisationAddressDtoValidatorTests
    {
        private readonly OrganisationAddressDtoValidator _validator;

        public OrganisationAddressDtoValidatorTests()
        {
            _validator = new OrganisationAddressDtoValidator();
        }

        #region LineOne tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_LineOne_IsEmpty(string lineOne)
        {
            var command = GetEditOrganisationAddressDto();
            command.LineOne = lineOne;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.LineOne);
        }

        [Fact]
        public void Should_Have_Error_When_LineOneLength_Exceeds_Maximum()
        {
            var command = GetEditOrganisationAddressDto();
            command.LineOne = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.LineOne);
        }

        [Theory]
        [InlineData("1 London Road")]
        [InlineData("The Swan")]
        [InlineData("Raspbery Lane")]
        public void Should_Not_Have_Error_When_LineOne(string lineOne)
        {
            var command = GetEditOrganisationAddressDto();
            command.LineOne = lineOne;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.LineOne);
        }
        #endregion

        #region City tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_City_IsEmpty(string city)
        {
            var command = GetEditOrganisationAddressDto();
            command.City = city;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.City);
        }

        [Fact]
        public void Should_Have_Error_When_CityLength_Exceeds_Maximum()
        {
            var command = GetEditOrganisationAddressDto();
            command.City = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.City);
        }

        [Theory]
        [InlineData("Milton Keynes")]
        [InlineData("Donlon")]
        [InlineData("Aix-en-Provence")]
        [InlineData("Barton-le-Clay")]
        public void Should_Not_Have_Error_When_City(string city)
        {
            var command = GetEditOrganisationAddressDto();
            command.City = city;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.City);
        }
        #endregion

        #region County tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_County_IsEmpty(string county)
        {
            var command = GetEditOrganisationAddressDto();
            command.County = county;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.County);
        }

        [Fact]
        public void Should_Have_Error_When_CountyLength_Exceeds_Maximum()
        {
            var command = GetEditOrganisationAddressDto();
            command.LineOne = "z";
            command.City = "z";
            command.Postcode = "z";
            command.County = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.County);
        }

        [Theory]
        [InlineData("IT Consultant")]
        [InlineData("MD")]
        [InlineData("Secretary")]
        public void Should_Not_Have_Error_When_County(string county)
        {
            var command = GetEditOrganisationAddressDto();
            command.County = county;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.County);
        }
        #endregion

        #region LineTwo tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_NotHave_Error_When_LineTwo_IsEmpty(string lineTwo)
        {
            var command = GetEditOrganisationAddressDto();
            command.LineTwo = lineTwo;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.LineTwo);
        }

        [Fact]
        public void Should_Have_Error_When_LineTwoLength_Exceeds_Maximum()
        {
            var command = GetEditOrganisationAddressDto();
            command.LineOne = "z";
            command.City = "z";
            command.Postcode = "z";
            command.LineTwo = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.LineTwo);
        }

        [Theory]
        [InlineData("IT Consultant")]
        [InlineData("MD")]
        [InlineData("Secretary")]
        public void Should_Not_Have_Error_When_LineTwo(string lineTwo)
        {
            var command = GetEditOrganisationAddressDto();
            command.LineTwo = lineTwo;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.LineTwo);
        }
        #endregion

        #region Postcode tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Postcode_IsEmpty(string postcode)
        {
            var command = GetEditOrganisationAddressDto();
            command.Postcode = postcode;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Postcode);
        }

        [Fact]
        public void Should_Have_Error_When_PostcodeLength_Exceeds_Maximum()
        {
            var command = GetEditOrganisationAddressDto();
            command.Postcode = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Postcode);
        }

        [Theory]
        [InlineData("1 London Road")]
        [InlineData("The Swan")]
        [InlineData("Raspbery Lane")]
        public void Should_Not_Have_Error_When_Postcode(string postcode)
        {
            var command = GetEditOrganisationAddressDto();
            command.Postcode = postcode;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Postcode);
        }
        #endregion

        private EditOrganisationAddressDto GetEditOrganisationAddressDto()
        {
            return new EditOrganisationAddressDto();
        }
    }
}