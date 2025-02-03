using Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Queries.Annual;

public class GetAnnualSupportingEvidenceFileNamesQueryValidatorTests
{
    private readonly GetAnnualSupportingEvidenceFileNamesQueryValidator _validator;

    public GetAnnualSupportingEvidenceFileNamesQueryValidatorTests()
    {
        _validator = new GetAnnualSupportingEvidenceFileNamesQueryValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var query = new GetAnnualSupportingEvidenceFileNamesQuery(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var query = new GetAnnualSupportingEvidenceFileNamesQuery(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }
}
