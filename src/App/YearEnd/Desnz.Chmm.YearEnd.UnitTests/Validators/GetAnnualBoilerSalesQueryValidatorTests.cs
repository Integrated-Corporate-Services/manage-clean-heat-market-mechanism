using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.YearEnd.Api.Controllers;
using Desnz.Chmm.YearEnd.Api.Validators.Commands;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Annual;

public class RollbackEndOfYearCommandValidatorTests
{
    private readonly RollbackEndOfYearCommandValidator _validator;

    public RollbackEndOfYearCommandValidatorTests()
    {
        _validator = new RollbackEndOfYearCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_SchemeYearId_IsEmpty(string inString)
    {
        Guid guidFromString;
        var guid = Guid.TryParse(inString, out guidFromString);

        var query = new RollbackEndOfYearCommand(guidFromString);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SchemeYearId_IsNotEmpty()
    {
        var query = new RollbackEndOfYearCommand(Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearId);
    }
}