using Desnz.Chmm.Notes.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Desnz.Chmm.Notes.UnitTests.Extensions;

public class NotesFileExtensionsTests
{
    private readonly string fileName = "TEST.pdf";
    private readonly Guid organisationId = Guid.NewGuid();
    private readonly Guid schemeYearId = Guid.NewGuid();
    private readonly Guid userId = Guid.NewGuid();
    private readonly Guid noteId = Guid.NewGuid();
    private readonly Mock<IFormFile> file;

    public NotesFileExtensionsTests()
    {
        file = new Mock<IFormFile>();
        file.Setup(x => x.FileName).Returns(fileName);
    }

    [Fact]
    public void BuildObjectKeyForFileForNewNote_IFormFile()
    {
        var key = NotesFileExtensions.BuildObjectKeyForFileForNewNote(file.Object, organisationId, schemeYearId, userId);
        Assert.Equal($"temp/{organisationId}/{schemeYearId}/{userId}/{fileName}", key);
    }

    [Fact]
    public void BuildObjectKeyForFileForNewNote_String()
    {
        var key = NotesFileExtensions.BuildObjectKeyForFileForNewNote(fileName, organisationId, schemeYearId, userId);
        Assert.Equal($"temp/{organisationId}/{schemeYearId}/{userId}/{fileName}", key);
    }

    [Fact]
    public void GetObjectKeyPrefixForNewNote()
    {
        var key = NotesFileExtensions.GetObjectKeyPrefixForNewNote(organisationId, schemeYearId, userId);
        Assert.Equal($"temp/{organisationId}/{schemeYearId}/{userId}", key);
    }

    [Fact]
    public void BuildObjectKeyForFileForExistingNote()
    {
        var key = NotesFileExtensions.BuildObjectKeyForFileForExistingNote(fileName, noteId);
        Assert.Equal($"{noteId}/{fileName}", key);
    }

    [Fact]
    public void GetObjectKeyPrefixForExistingNote()
    {
        var key = NotesFileExtensions.GetObjectKeyPrefixForExistingNote(noteId);
        Assert.Equal($"{noteId}", key);
    }
}
