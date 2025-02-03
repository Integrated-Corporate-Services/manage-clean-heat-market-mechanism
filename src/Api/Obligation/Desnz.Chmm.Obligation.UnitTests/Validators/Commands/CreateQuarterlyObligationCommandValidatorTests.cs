using Desnz.Chmm.Obligation.Api.Validators.Commands;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Testing.Common;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Validators.Commands;

public class CreateQuarterlyObligationCommandValidatorTests
{
    private readonly CreateQuarterlyObligationCommandValidator _validator;

    public CreateQuarterlyObligationCommandValidatorTests()
    {
        _validator = new CreateQuarterlyObligationCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var command = new CreateQuarterlyObligationCommand
        {
            SchemeYearQuarterId = Guid.Empty,
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }

    [Fact]
    public void ShouldNotHaveError_When_Oil_IsValid()
    {
        var quarterlyGuids = new List<Guid>()
            {
                SchemeYearConstants.QuarterOneId,
                SchemeYearConstants.QuarterTwoId,
                SchemeYearConstants.QuarterThreeId,
                SchemeYearConstants.QuarterFourId
            };

        quarterlyGuids.ForEach((x) =>
        {
            var command = new CreateQuarterlyObligationCommand
            {
                SchemeYearQuarterId = x
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearQuarterId);
        });
    }

    [Fact]
    public void Should_Not_Have_Validation_Errors_When_All_Properties_AreValid()
    {
        var command = new CreateQuarterlyObligationCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = SchemeYearConstants.Id,
            TransactionDate = DateTime.UtcNow,
            SchemeYearQuarterId = SchemeYearConstants.QuarterOneId,
            Gas = 5_000,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearId);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearQuarterId);
        result.ShouldNotHaveValidationErrorFor(c => c.TransactionDate);
        result.ShouldNotHaveValidationErrorFor(c => c.Gas);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
    }

}
