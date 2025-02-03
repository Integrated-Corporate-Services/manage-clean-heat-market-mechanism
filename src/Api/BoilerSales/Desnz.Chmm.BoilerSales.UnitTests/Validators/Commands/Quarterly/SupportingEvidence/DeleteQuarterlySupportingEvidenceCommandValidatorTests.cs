using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Quarterly.SupportingEvidence;

public class DeleteQuarterlySupportingEvidenceCommandValidatorTests
{
    private readonly DeleteQuarterlySupportingEvidenceCommandValidator _validator;

    public DeleteQuarterlySupportingEvidenceCommandValidatorTests()
    {
        _validator = new DeleteQuarterlySupportingEvidenceCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var _command = new DeleteQuarterlySupportingEvidenceCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "value");

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var _command = new DeleteQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "value");

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var _command = new DeleteQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "value");

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldHaveError_When_FileNameIsEmpty(string? fileName)
    {
        var _command = new DeleteQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), fileName);

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.FileName);
    }
}
