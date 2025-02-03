using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Notes.Api.Controllers;
using Desnz.Chmm.Notes.Common.Commands;
using Desnz.Chmm.Notes.Common.Dtos;
using Desnz.Chmm.Notes.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Desnz.Chmm.Notes.UnitTests.Controllers;

public class NotesControllerTests
{
    private readonly NotesController controller;
    private readonly Mock<IMediator> mediator;

    private readonly Guid organisationId = Guid.NewGuid();
    private readonly Guid schemeYearId = Guid.NewGuid();
    private readonly Guid noteId = Guid.NewGuid();
    private readonly string fileName = "TEST.pdf";
    private readonly CancellationToken cancellationToken = new CancellationToken();

    public NotesControllerTests()
    {
        mediator = new Mock<IMediator>();
        controller = new NotesController(mediator.Object);
    }

    [Fact]
    public async Task GetAllManufacturerNotes()
    {
        mediator.Setup(x => x.Send(It.IsAny<GetManufacturerNotesQuery>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult<List<ManufacturerNoteDto>>>(Responses.Ok()));

        var response = await controller.GetAllManufacturerNotes(organisationId, schemeYearId, cancellationToken);
    }

    [Fact]
    public async Task CreateManufacturerNote()
    {
        mediator.Setup(x => x.Send(It.IsAny<AddManufacturerNoteCommand>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult<Guid>>(Responses.Ok()));

        var response = await controller.CreateManufacturerNote(new(), cancellationToken);
    }

    [Fact]
    public async Task GetExistingNoteFileNames()
    {
        mediator.Setup(x => x.Send(It.IsAny<GetManufacturerNoteFileNamesQuery>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult<List<string>>>(Responses.Ok()));

        var response = await controller.GetExistingNoteFileNames(organisationId, schemeYearId, noteId, cancellationToken);
    }

    [Fact]
    public async Task GetNewNoteFileNames()
    {
        mediator.Setup(x => x.Send(It.IsAny<GetManufacturerNoteFileNamesQuery>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult<List<string>>>(Responses.Ok()));

        var response = await controller.GetNewNoteFileNames(organisationId, schemeYearId, cancellationToken);
    }

    [Fact]
    public async Task ClearNewNoteFiles()
    {
        mediator.Setup(x => x.Send(It.IsAny<ClearManufacturerNoteFilesCommand>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult>(Responses.Ok()));

        var response = await controller.ClearNewNoteFiles(organisationId, schemeYearId, cancellationToken);
    }

    [Fact]
    public async Task UploadNewNoteFile()
    {
        mediator.Setup(x => x.Send(It.IsAny<UploadManufacturerNoteFileCommand>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult>(Responses.Ok()));

        var response = await controller.UploadNewNoteFile(organisationId, schemeYearId, new(), cancellationToken);
    }

    [Fact]
    public async Task DownloadExistingNoteFile()
    {
        mediator.Setup(x => x.Send(It.IsAny<DownloadManufacturerNoteFileQuery>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult<Stream>>(Responses.Ok()));

        var response = await controller.DownloadExistingNoteFile(organisationId, schemeYearId, noteId, fileName, cancellationToken);
    }

    [Fact]
    public async Task DeleteNewNoteFile()
    {
        mediator.Setup(x => x.Send(It.IsAny<DeleteManufacturerNoteFileCommand>(), cancellationToken))
            .Returns(Task.FromResult<ActionResult>(Responses.Ok()));

        var response = await controller.DeleteNewNoteFile(organisationId, schemeYearId, new(), cancellationToken);
    }
}