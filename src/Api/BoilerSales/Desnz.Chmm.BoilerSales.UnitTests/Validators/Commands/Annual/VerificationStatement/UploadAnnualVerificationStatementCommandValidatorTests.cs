using Desnz.Chmm.BoilerSales.Common.Commands;
using Xunit;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using System;
using FluentAssertions;
using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Annual.VerificationStatement;

public class UploadAnnualVerificationStatementCommandValidatorTests
{
    private readonly UploadAnnualVerificationStatementCommandValidator _validator;

    public UploadAnnualVerificationStatementCommandValidatorTests()
    {
        _validator = new UploadAnnualVerificationStatementCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var _command = new UploadAnnualVerificationStatementCommand(Guid.Empty, Guid.NewGuid(), null);

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.ManufacturerId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var _command = new UploadAnnualVerificationStatementCommand(Guid.NewGuid(), Guid.Empty, null);

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Theory, MemberData(nameof(SupportingEvidenceList))]
    public void ShouldHaveError_When_SupportingEvidenceListEmpty(List<IFormFile>? supportingEvidence)
    {
        var _command = new UploadAnnualVerificationStatementCommand(Guid.NewGuid(), Guid.NewGuid(), supportingEvidence);

        var result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(c => c.VerificationStatement);
    }

    [Fact]
    public void ShouldHaveError_When_SupportingEvidenceEmpty()
    {
        var _command = new UploadAnnualVerificationStatementCommand(Guid.NewGuid(), Guid.NewGuid(), new List<IFormFile?>() { default });

        var result = _validator.TestValidate(_command);
        var hasErrors = result.Errors.Any(e => e.PropertyName.StartsWith("VerificationStatement"));
        hasErrors.Should().BeTrue();
    }

    public static IEnumerable<object?[]> SupportingEvidenceList
    {
        get
        {
            yield return new object?[] { default(List<IFormFile>) };
            yield return new object?[] { new List<IFormFile>() };
        }
    }
}
