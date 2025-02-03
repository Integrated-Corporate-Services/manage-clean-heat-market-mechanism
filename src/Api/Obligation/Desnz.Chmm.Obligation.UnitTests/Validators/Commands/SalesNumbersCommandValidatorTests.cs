using Desnz.Chmm.Obligation.Api.Validators.Commands;
using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Validators.Commands;

public class SalesNumbersCommandValidatorTests
{
    private readonly SalesNumbersCommandValidator _validator;

    public SalesNumbersCommandValidatorTests()
    {
        _validator = new SalesNumbersCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_Gas_IsNotValid()
    {
        var command = new SalesNumbersCommand
        {
            Gas = -1,
            Oil = 6_000
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Gas);
    }

    [Fact]
    public void ShouldHaveError_When_Oil_IsNotValid()
    {
        var command = new SalesNumbersCommand
        {
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
        var command = new SalesNumbersCommand
        {
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
        var command = new SalesNumbersCommand
        {
            Gas = 5_000,
            Oil = oil
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Oil);
    }
}
