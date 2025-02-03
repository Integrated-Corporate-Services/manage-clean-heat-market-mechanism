using Desnz.Chmm.BoilerSales.Common.Commands;
using Xunit;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using System;
using FluentAssertions;
using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Annual.SupportingEvidence;

public class UploadAnnualSupportingEvidenceCommandValidatorTests
{
    private readonly UploadAnnualSupportingEvidenceCommandValidator _validator;

    public UploadAnnualSupportingEvidenceCommandValidatorTests()
    {
        _validator = new UploadAnnualSupportingEvidenceCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new UploadAnnualSupportingEvidenceCommand(Guid.Empty, Guid.NewGuid(), new List<IFormFile> { });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new UploadAnnualSupportingEvidenceCommand(Guid.NewGuid(), Guid.Empty, new List<IFormFile> { });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Theory, MemberData(nameof(SupportingEvidenceList))]
    public void ShouldHaveError_When_SupportingEvidenceListEmpty(List<IFormFile>? supportingEvidence)
    {
        var command = new UploadAnnualSupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), supportingEvidence);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SupportingEvidence);
    }

    [Fact]
    public void ShouldHaveError_When_SupportingEvidenceEmpty()
    {
        var command = new UploadAnnualSupportingEvidenceCommand(Guid.NewGuid(), Guid.NewGuid(), new List<IFormFile?>() { default });

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
