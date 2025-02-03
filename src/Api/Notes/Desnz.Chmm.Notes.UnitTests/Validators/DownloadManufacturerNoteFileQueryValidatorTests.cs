using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.Notes.Api.Validators.Queries;
using Desnz.Chmm.Notes.Common.Queries;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.Notes.UnitTests.Validators;

public class DownloadManufacturerNoteFileQueryValidatorTests
{
    private readonly DownloadManufacturerNoteFileQueryValidator _validator;

    public DownloadManufacturerNoteFileQueryValidatorTests()
    {
        _validator = new DownloadManufacturerNoteFileQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_OrganisationId_IsEmpty()
    {
        var query = new DownloadManufacturerNoteFileQuery(null, Guid.NewGuid(), Guid.NewGuid(), "string");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_SchemeYearId_IsEmpty()
    {
        var query = new DownloadManufacturerNoteFileQuery(Guid.NewGuid(), null, Guid.NewGuid(), "string");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_NoteId_IsEmpty()
    {
        var query = new DownloadManufacturerNoteFileQuery(Guid.NewGuid(), Guid.NewGuid(), null, "string");
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.NoteId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Not_Have_Error_When_FileName_IsEmpty(string inString)
    {
        var query = new DownloadManufacturerNoteFileQuery(Guid.NewGuid(), Guid.Empty, Guid.Empty, inString);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(c => c.FileName);
    }
}
