using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Annual.SupportingEvidence;

public class DeleteAnnualSupportingEvidenceCommandValidatorTests
{
    private readonly DeleteAnnualSupportingEvidenceCommandValidator _validator;

    public DeleteAnnualSupportingEvidenceCommandValidatorTests()
    {
        _validator = new DeleteAnnualSupportingEvidenceCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var _command = new DeleteAnnualSupportingEvidenceCommand(Guid.Empty, Guid.NewGuid(), "value");

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var _command = new DeleteAnnualSupportingEvidenceCommand(Guid.NewGuid(), Guid.Empty, "value");

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldHaveError_When_FileNameIsEmpty(string? fileName)
    {
        var _command = new DeleteAnnualSupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), fileName);

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.FileName);
    }
}
