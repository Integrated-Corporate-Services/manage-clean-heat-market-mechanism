using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Annual.SupportingEvidence;

public class SubmitAnnualBoilerSalesCommandValidatorTests
{
    private readonly SubmitAnnualBoilerSalesCommandValidator _validator;

    public SubmitAnnualBoilerSalesCommandValidatorTests()
    {
        _validator = new SubmitAnnualBoilerSalesCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.Empty,
            SchemeYearId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = 6_000
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.Empty,
            Gas = 5_000,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_Gas_IsNotValid()
    {
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            Gas = -1,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Gas);
    }

    [Fact]
    public void ShouldHaveError_When_Oil_IsNotValid()
    {
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
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
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
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
        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = Guid.NewGuid(),
            SchemeYearId = Guid.NewGuid(),
            Gas = 5_000,
            Oil = oil
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Oil);
    }
}
