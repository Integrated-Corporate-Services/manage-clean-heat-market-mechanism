using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class DeactivateAdminUserCommandHandlerTests
{
    private Mock<IUsersRepository> _mockUsersRepository;
    private Mock<ILogger<DeactivateAdminUserCommandHandler>> _mockLogger;
    private Mock<IIdentityNotificationService> _mockIdentityNotificationService;
    private Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private Mock<IUnitOfWork> _unitOfWork;

    private ChmmRole _adminRole;
    private GovukNotifyConfig _govukNotifyConfig;

    public DeactivateAdminUserCommandHandlerTests()
    {
        _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
        _mockLogger = new Mock<ILogger<DeactivateAdminUserCommandHandler>>();
        _mockIdentityNotificationService = new Mock<IIdentityNotificationService>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _adminRole = new ChmmRole(IdentityConstants.Roles.RegulatoryOfficer);
        _govukNotifyConfig = new GovukNotifyConfig();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());
    }

    [Fact]
    internal async Task TestPostUser_NotFoundAsync()
    {
        //Arrange
        var handler = new DeactivateAdminUserCommandHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockIdentityNotificationService.Object);
        var command = new DeactivateAdminUserCommand() { Id = Guid.NewGuid() };

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, false, true)).Returns(Task.FromResult<ChmmUser?>(null));

        //Act
        var actionResult = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    internal async Task TestPostUser_BadRequestAsync()
    {
        //Arrange
        var handler = new DeactivateAdminUserCommandHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockIdentityNotificationService.Object);
        var command = new DeactivateAdminUserCommand() { Id = Guid.NewGuid() };

        var user = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { _adminRole });
        user.Deactivate();

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, false, true)).Returns(Task.FromResult<ChmmUser?>(user));

        //Act
        var actionResult = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    internal async Task TestPostUser_NoContentAsync()
    {
        //Arrange
        var handler = new DeactivateAdminUserCommandHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockIdentityNotificationService.Object);
        var command = new DeactivateAdminUserCommand() { Id = Guid.NewGuid() };

        var user = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { _adminRole });
        user.Activate();

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, false, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockIdentityNotificationService.Setup(x => x.NotifyUserDeactivated(user));

        //Act
        var actionResult = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }
}
