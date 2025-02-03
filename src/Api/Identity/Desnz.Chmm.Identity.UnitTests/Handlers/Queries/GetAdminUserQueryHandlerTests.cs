using Xunit;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using AutoMapper;
using Desnz.Chmm.Identity.Common.Queries;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Common.Dtos;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetAdminUserQueryHandlerTests
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly Mock<ILogger<GetAdminUserQueryHandler>> _mockLogger;
    private readonly GetAdminUserQueryHandler _handler;
    private readonly Mock<IMapper> _mockMapper;

    public GetAdminUserQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetAdminUserQueryHandler>>();
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mockMapper = new Mock<IMapper>();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());

        _handler = new GetAdminUserQueryHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockMapper.Object);
    }

    [Fact]
    internal async Task TestGetUser_UserId_NotFoundAsync()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var command = new GetAdminUserQuery() { UserId = Guid.NewGuid() };

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.UserId, true, false)).Returns(Task.FromResult<ChmmUser?>(null));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult.Result);
     }


    [Fact]
    internal async Task TestGettUser_OkAsync()
    {
        //Arrange
        var role = new ChmmRole("A Role");
        var user = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { role });
        user.Activate();

        var command = new GetAdminUserQuery() { UserId = Guid.NewGuid() };

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.UserId, true, false)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockMapper.Setup(x => x.Map<ChmmUserDto>(user)).Returns(new ChmmUserDto());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<ChmmUserDto>>(actionResult);
        Assert.NotNull(actionResult.Value);
    }

}
