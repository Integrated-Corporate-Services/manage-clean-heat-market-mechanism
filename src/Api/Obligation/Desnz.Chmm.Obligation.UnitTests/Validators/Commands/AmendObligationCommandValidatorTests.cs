using Desnz.Chmm.Obligation.Api.Validators.Commands;
using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Validators
{
    public class AmendObligationCommandValidatorTests
    {
        private readonly AmendObligationCommandValidator _validator;

        public AmendObligationCommandValidatorTests()
        {
            _validator = new AmendObligationCommandValidator();
        }

        [Theory, MemberData(nameof(EmptyGuidData))]
        internal void ShouldHaveError_When_OrganisationIdIsEmpty(Guid organisationId)
        {
            // Act
            var command = new AmendObligationCommand()
            {
                OrganisationId = organisationId
            };

            // Arrange
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
        }

        [Theory, MemberData(nameof(EmptyGuidData))]
        internal void ShouldHaveError_When_SchemeYearIdIsEmpty(Guid schemeYearId)
        {
            // Act
            var command = new AmendObligationCommand()
            {
                SchemeYearId = schemeYearId
            };

            // Arrange
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
        }

        [Theory]
        [InlineData(null)]
        internal void ShouldHaveError_When_ValueIsNull(int value)
        {
            // Act
            var command = new AmendObligationCommand()
            {
                Value = value
            };

            // Arrange
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
        }

        public static IEnumerable<object[]> EmptyGuidData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { Guid.Empty }
            };
    }
}
