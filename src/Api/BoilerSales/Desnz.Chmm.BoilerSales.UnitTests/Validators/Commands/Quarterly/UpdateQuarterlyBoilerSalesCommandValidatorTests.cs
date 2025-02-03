using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using FluentValidation.TestHelper;
using System.Security.Cryptography;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Quarterly.SupportingEvidence;

public class UpdateQuarterlyBoilerSalesCommandValidatorTests
{
    private readonly UpdateQuarterlyBoilerSalesCommandValidator _validator;

    public UpdateQuarterlyBoilerSalesCommandValidatorTests()
    {
        _validator = new UpdateQuarterlyBoilerSalesCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 5_000, 6_000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), 5_000, 6_000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 5_000, 6_000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }

    [Fact]
    public void ShouldHaveError_When_Gas_IsNotValid()
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -1, 6_000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Gas);
    }

    [Fact]
    public void ShouldHaveError_When_Oil_IsNotValid()
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5_000, -1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Oil);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5_000)]
    public void ShouldNotHaveError_When_Gas_IsValid(int gas)
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), gas, 6_000);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Gas);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5_000)]
    public void ShouldNotHaveError_When_Oil_IsValid(int oil)
    {
        var command = new EditQuarterlyBoilerSalesCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5_000, oil);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Oil);
    }
}
