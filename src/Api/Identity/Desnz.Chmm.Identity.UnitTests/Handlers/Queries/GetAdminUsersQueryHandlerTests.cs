using Xunit;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Http;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Entities;
using AutoMapper;
using Desnz.Chmm.Common.Dtos;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetAdminUsersQueryHandlerTests
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly Mock<ILogger<GetAdminUsersQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;

    private readonly GetAdminUsersQueryHandler _handler;
    private readonly ChmmRole _adminRole;
    private readonly List<ChmmUser> _adminUsers;

    public GetAdminUsersQueryHandlerTests()
    {
        _adminRole = new ChmmRole(Roles.RegulatoryOfficer);
        _adminUsers = new List<ChmmUser>{ new ChmmUser("Admin User", "user1@example.com", new List<ChmmRole> { _adminRole }) };

        _mockLogger = new Mock<ILogger<GetAdminUsersQueryHandler>>();
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mockMapper = new Mock<IMapper>();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());

        _handler = new GetAdminUsersQueryHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockMapper.Object);
    }

    [Fact]
    internal async Task Can_GetAdminUsers_OkAsync_Test()
    {
        //Arrange
        var expectedUsers = new List<ChmmUserDto>() { new ChmmUserDto() };
        var command = new GetAdminUsersQuery();
        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(_adminUsers));
        _mockMapper.Setup(x => x.Map<List<ChmmUserDto>>(_adminUsers)).Returns(expectedUsers);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<List<ChmmUserDto>>>(actionResult);
        Assert.Single(actionResult.Value);
    }
}
