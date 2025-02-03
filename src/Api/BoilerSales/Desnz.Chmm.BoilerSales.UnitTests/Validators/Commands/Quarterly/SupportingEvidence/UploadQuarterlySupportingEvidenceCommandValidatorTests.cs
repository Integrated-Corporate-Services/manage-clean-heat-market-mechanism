using Desnz.Chmm.BoilerSales.Common.Commands;
using Xunit;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using System;
using FluentAssertions;
using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Quarterly.SupportingEvidence;

public class UploadQuarterlySupportingEvidenceCommandValidatorTests
{
    private readonly UploadQuarterlySupportingEvidenceCommandValidator _validator;

    public UploadQuarterlySupportingEvidenceCommandValidatorTests()
    {
        _validator = new UploadQuarterlySupportingEvidenceCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), new List<IFormFile> { });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), new List<IFormFile> { });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearQuarterIdIsEmpty()
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, new List<IFormFile> { });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearQuarterId);
    }

    [Theory, MemberData(nameof(SupportingEvidenceList))]
    public void ShouldHaveError_When_SupportingEvidenceListEmpty(List<IFormFile>? supportingEvidence)
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), supportingEvidence);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SupportingEvidence);
    }

    [Fact]
    public void ShouldHaveError_When_SupportingEvidenceEmpty()
    {
        var command = new UploadQuarterlySupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<IFormFile?>() { default });

        var result = _validator.TestValidate(command);
        var hasErrors = result.Errors.Any(e => e.PropertyName.StartsWith("SupportingEvidence"));
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
