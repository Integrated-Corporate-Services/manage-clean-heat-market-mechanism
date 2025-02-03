using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
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

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetRejectionFilesQueryHandlerTests
{
    private readonly GetRejectionFilesQueryHandler _handler;

    private readonly Mock<ILogger<BaseRequestHandler<GetRejectionFilesQuery, ActionResult<List<string>>>>> _logger;
    private readonly Mock<IFileService> _fileService;
    private readonly Mock<IOrganisationsRepository> _organisationsRepository;

    public GetRejectionFilesQueryHandlerTests()
    {
        _logger = new Mock<ILogger<BaseRequestHandler<GetRejectionFilesQuery, ActionResult<List<string>>>>>();
        _fileService = new Mock<IFileService>();
        _organisationsRepository = new Mock<IOrganisationsRepository>();

        _handler = new GetRejectionFilesQueryHandler(
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

        var response = await _handler.Handle(new GetRejectionFilesQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.NotNull(response);
        Assert.NotNull(response.Result);
        Assert.Null(response.Value);
        Assert.IsType<NotFoundObjectResult>(response.Result);
    }

    [Fact]
    public async Task Ok()
    {
        var organisation = ApproveManufacturerApplicationCommandHandlerTestsFixture.GetMockOrganisation();
        _fileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<string> { "test.pdf" });
        _organisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, false, false)).Returns(Task.FromResult(organisation));

        var response = await _handler.Handle(new GetRejectionFilesQuery(organisation.Id), CancellationToken.None);

        Assert.NotNull(response);
        Assert.Null(response.Result);
        Assert.NotNull(response.Value);
        Assert.IsType<List<string>>(response.Value);
    }
}
