using Desnz.Chmm.BoilerSales.Api.Validators.Queries.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Quarterly;

public class GetQuarterlySupportingEvidenceFileNamesQueryValidatorTests
{
    private readonly GetQuarterlySupportingEvidenceFileNamesQueryValidator _validator;

    public GetQuarterlySupportingEvidenceFileNamesQueryValidatorTests()
    {
        _validator = new GetQuarterlySupportingEvidenceFileNamesQueryValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var query = new GetQuarterlySupportingEvidenceFileNamesQuery(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var query = new GetQuarterlySupportingEvidenceFileNamesQuery(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var query = new GetQuarterlySupportingEvidenceFileNamesQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }
}
