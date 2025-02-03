using Xunit;
using Desnz.Chmm.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Identity.Common.Queries;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Entities;
using AutoMapper;
using Desnz.Chmm.Common.Dtos;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetAdminRolesQueryHandlerTests
{
    private readonly Mock<IRolesRepository> _mockRolesRepository;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly Mock<ILogger<GetAdminRolesQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;

    private readonly GetAdminRolesQueryHandler _handler;
    private readonly List<ChmmRole> _adminRoles;

    public GetAdminRolesQueryHandlerTests()
    {
        _adminRoles = new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.RegulatoryOfficer) };

        _mockLogger = new Mock<ILogger<GetAdminRolesQueryHandler>>();
        _mockRolesRepository = new Mock<IRolesRepository>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mockMapper = new Mock<IMapper>();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());

        _handler = new GetAdminRolesQueryHandler(_mockLogger.Object, _mockRolesRepository.Object, _mockMapper.Object);
    }

    [Fact]
    internal async Task Can_GetAdminRoles_OkAsync_Test()
    {
        //Arrange
        var command = new GetAdminRolesQuery();
        var expectedRoles = new List<RoleDto>() { new RoleDto() };

        _mockRolesRepository.Setup(x => x.GetAdminRoles(false)).Returns(Task.FromResult(_adminRoles));
        _mockMapper.Setup(x => x.Map<List<RoleDto>>(_adminRoles)).Returns(expectedRoles);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.Single(actionResult.Value);
        Assert.IsType<ActionResult<List<RoleDto>>>(actionResult);
    }
}
