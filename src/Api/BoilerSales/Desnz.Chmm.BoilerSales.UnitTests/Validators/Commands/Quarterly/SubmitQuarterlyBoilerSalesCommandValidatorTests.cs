using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Quarterly.SupportingEvidence;

public class SubmitQuarterlyBoilerSalesCommandValidatorTests
{
    private readonly SubmitQuarterlyBoilerSalesCommandValidator _validator;

    public SubmitQuarterlyBoilerSalesCommandValidatorTests()
    {
        _validator = new SubmitQuarterlyBoilerSalesCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.Empty,
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = 6_000
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.Empty,
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.Empty,
            Gas = 5_000,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }

    [Fact]
    public void ShouldHaveError_When_Gas_IsNotValid()
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = -1,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Gas);
    }

    [Fact]
    public void ShouldHaveError_When_Oil_IsNotValid()
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = -1
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Oil);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5_000)]
    public void ShouldNotHaveError_When_Gas_IsValid(int gas)
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = gas,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Gas);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5_000)]
    public void ShouldNotHaveError_When_Oil_IsValid(int oil)
    {
        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            SchemeYearQuarterId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = oil
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Oil);
    }
}
