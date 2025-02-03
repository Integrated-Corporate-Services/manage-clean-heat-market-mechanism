using Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using FluentValidation.TestHelper;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Annual;

public class GetAnnualVerificationStatementFileNamesQueryValidatorTests
{
    private readonly GetAnnualVerificationStatementFileNamesQueryValidator _validator;

    public GetAnnualVerificationStatementFileNamesQueryValidatorTests()
    {
        _validator = new GetAnnualVerificationStatementFileNamesQueryValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var query = new GetAnnualVerificationStatementFileNamesQuery(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var query = new GetAnnualVerificationStatementFileNamesQuery(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }
}
