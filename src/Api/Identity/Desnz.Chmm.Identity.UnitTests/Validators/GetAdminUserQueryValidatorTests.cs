using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Queries;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class GetAdminUserQueryValidatorTests
    {
        private readonly GetAdminUserQueryValidator _validator;
        private readonly GetAdminUserQuery _command;

        public GetAdminUserQueryValidatorTests()
        {
            _command = new GetAdminUserQuery();

            _validator = new GetAdminUserQueryValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Id_IsEmpty(string inString)
        {
            Guid guidFromString;
            var guid = Guid.TryParse(inString, out guidFromString);

            _command.UserId = guidFromString;
            var result = _validator.TestValidate(_command);
            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_IsEmpty()
        {
            _command.UserId = Guid.NewGuid();
            var result = _validator.TestValidate(_command);
            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
        }
    }
}