using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Validators.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.CreditLedger.UnitTests.Validators.Commands
{
    public class CreditCalculationDtoValidatorTests
    {
        private readonly CreditCalculationDtoValidator _validator;

        public CreditCalculationDtoValidatorTests()
        {
            _validator = new CreditCalculationDtoValidator();
        }


        [Fact]
        public void ShouldHaveError_When_CommissioningDate_IsEmpty()
        {
            var command = new McsInstallationDto
            {
                CommissioningDate = DateTime.MinValue,
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CommissioningDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_MidId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                MidId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.MidId);
        }

        [Theory(Skip ="Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_TotalCapacity_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                TotalCapacity = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.TotalCapacity);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_AirTypeTechnologyId_IsEmpty(string inString)
        {
            var query = new McsInstallationDto
            {
                CommissioningDate = new DateTime(2024, 1, 1),
                MidId = 1,
                TotalCapacity = 1,

                HeatPumpProducts = new List<McsProductDto>
                {
                    new McsProductDto
                    {
                        Id = 1,
                        Code = "code",
                        ManufacturerId = 1,
                        ManufacturerName = "name",
                        Name = "name"
                    }
                }
            };

            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            query.AirTypeTechnologyId = intFromString;

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.AirTypeTechnologyId);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_AlternativeHeatingFuelId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                AlternativeHeatingFuelId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.AlternativeHeatingFuelId);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_AlternativeHeatingSystemId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                AlternativeHeatingSystemId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.AlternativeHeatingSystemId);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_IsNewBuildId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                IsNewBuildId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.IsNewBuildId);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_RenewableSystemDesignId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                RenewableSystemDesignId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.RenewableSystemDesignId);
        }

        [Theory(Skip = "Only include to test if zero values are not acceptable")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_TechnologyTypeId_IsEmpty(string inString)
        {
            int intFromString;
            var i = Int32.TryParse(inString, out intFromString);
            var query = new McsInstallationDto
            {
                TechnologyTypeId = intFromString
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(c => c.TechnologyTypeId);
        }
     }
}