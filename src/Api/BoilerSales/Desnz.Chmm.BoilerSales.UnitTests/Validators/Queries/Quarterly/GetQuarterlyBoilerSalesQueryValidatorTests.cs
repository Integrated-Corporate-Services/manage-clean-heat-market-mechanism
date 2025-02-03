using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.BoilerSales.Api.Validators.Queries.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Quarterly;

public class GetQuarterlyBoilerSalesQueryValidatorTests
{
    private readonly GetQuarterlyBoilerSalesQueryValidator _validator;

    public GetQuarterlyBoilerSalesQueryValidatorTests()
    {
        _validator = new GetQuarterlyBoilerSalesQueryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Not_Have_Error_When_ManufacturerId_IsEmpty(string inString)
    {
        Guid guidFromString;
        var guid = Guid.TryParse(inString, out guidFromString);
        var query = new GetQuarterlyBoilerSalesQuery(guidFromString, Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ManufacturerId_IsNotEmpty()
    {
        var query = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_SchemeYearId_IsEmpty(string inString)
    {
        Guid guidFromString;
        var guid = Guid.TryParse(inString, out guidFromString);

        var query = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), guidFromString);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SchemeYearId_IsNotEmpty()
    {
        var query = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearId);
    }
}