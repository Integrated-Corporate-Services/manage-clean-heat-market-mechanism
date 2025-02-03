using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Annual;

public class GetAnnualBoilerSalesQueryValidatorTests
{
    private readonly GetAnnualBoilerSalesQueryValidator _validator;

    public GetAnnualBoilerSalesQueryValidatorTests()
    {
        _validator = new GetAnnualBoilerSalesQueryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Not_Have_Error_When_ManufacturerId_IsEmpty(string inString)
    {
        Guid guidFromString;
        var guid = Guid.TryParse(inString, out guidFromString);

        var query = new GetAnnualBoilerSalesQuery(guidFromString, Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ManufacturerId_IsNotEmpty()
    {
        var query = new GetAnnualBoilerSalesQuery(Guid.NewGuid(), Guid.NewGuid());
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

        var query = new GetAnnualBoilerSalesQuery(Guid.NewGuid(), guidFromString);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SchemeYearId_IsNotEmpty()
    {
        var query = new GetAnnualBoilerSalesQuery(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(c => c.SchemeYearId);
    }
}