using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Handlers.Commands.ApprovalFiles;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.ApprovalFiles;
using Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Commands;
using Desnz.Chmm.Identity.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Desnz.Chmm.Common.Services.FileService;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands.ApprovalFiles;

public class DeleteApprovalFilesCommandHandlerTests
{
    private readonly DeleteApprovalFilesCommandHandler _handler;

    private readonly Mock<ILogger<BaseRequestHandler<DeleteApprovalFilesCommand, ActionResult>>> _logger;
    private readonly Mock<IFileService> _fileService;
    private readonly Mock<IOrganisationsRepository> _organisationsRepository;

    public DeleteApprovalFilesCommandHandlerTests()
    {
        _logger = new Mock<ILogger<BaseRequestHandler<DeleteApprovalFilesCommand, ActionResult>>>();
        _fileService = new Mock<IFileService>();
        _organisationsRepository = new Mock<IOrganisationsRepository>();

        _handler = new DeleteApprovalFilesCommandHandler(
            _logger.Object,
            _fileService.Object,
            _organisationsRepository.Object
        );
    }

    [Fact]
    public async Task Not_Found_When_Organisation_Not_Found()
    {
        _organisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, false, false))
            .Returns(Task.FromResult<Api.Entities.Organisation?>(null));

        var response = await _handler.Handle(new DeleteApprovalFilesCommand(Guid.NewGuid(), "test.pdf"), CancellationToken.None);

        Assert.NotNull(response);
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task Bad_Request_When_File_Service_Fails()
    {
        var organisation = ApproveManufacturerApplicationCommandHandlerTestsFixture.GetMockOrganisation();
        _fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null, "Error"));
        _organisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, false, false)).Returns(Task.FromResult(organisation));

        var response = await _handler.Handle(new DeleteApprovalFilesCommand(organisation.Id, "test.pdf"), CancellationToken.None);

        Assert.NotNull(response);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Ok()
    {
        var organisation = ApproveManufacturerApplicationCommandHandlerTestsFixture.GetMockOrganisation();
        var files = new List<IFormFile> { FileHelper.CreateFormFile("Approval File 1.txt") };
        _fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, ""));
        _organisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, false, false)).Returns(Task.FromResult(organisation));

        var response = await _handler.Handle(new DeleteApprovalFilesCommand(organisation.Id, "test.pdf"), CancellationToken.None);

        Assert.NotNull(response);
        Assert.IsType<OkResult>(response);
    }
}
