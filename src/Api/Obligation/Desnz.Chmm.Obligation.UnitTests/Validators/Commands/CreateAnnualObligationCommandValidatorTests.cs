using Desnz.Chmm.Obligation.Api.Validators.Commands;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Testing.Common;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Validators.Commands;

public class CreateAnnualObligationCommandValidatorTests
{
    private readonly CreateAnnualObligationCommandValidator _validator;

    public CreateAnnualObligationCommandValidatorTests()
    {
        _validator = new CreateAnnualObligationCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_TransactionDate_IsEmpty()
    {
        var command = new CreateAnnualObligationCommand
        {
            TransactionDate = DateTime.MinValue,
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.TransactionDate);
    }
    [Fact]
    public void ShouldHaveError_When_OrganisationId_IsEmpty()
    {
        var command = new CreateAnnualObligationCommand
        {
            OrganisationId = Guid.Empty,
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new CreateAnnualObligationCommand
        {
            SchemeYearId = Guid.Empty,
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void Should_Not_Have_Validation_Errors_When_All_Properties_AreValid()
    {
        var command = new CreateAnnualObligationCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = SchemeYearConstants.Id,
            TransactionDate = DateTime.UtcNow,
            Gas = 5_000,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearId);
        result.ShouldNotHaveValidationErrorFor(c => c.TransactionDate);
        result.ShouldNotHaveValidationErrorFor(c => c.Gas);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
    }

}
