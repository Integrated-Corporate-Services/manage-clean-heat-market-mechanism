using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class EditUserCommandHandlerTests
{
    private Mock<IUsersRepository> _mockUsersRepository;
    private Mock<IRolesRepository> _mockRolesRepository;
    private Mock<IIdentityNotificationService> _mockIdentityNotificationService;
    private Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private Mock<ILogger<EditAdminUserCommandHandler>> _mockLogger;
    private Mock<IUnitOfWork> _unitOfWork;

    private EditAdminUserCommandHandler _handler;
    private GovukNotifyConfig _govukNotifyConfig;

    public EditUserCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EditAdminUserCommandHandler>>();
        _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
        _mockRolesRepository = new Mock<IRolesRepository>(MockBehavior.Strict);
        _mockIdentityNotificationService = new Mock<IIdentityNotificationService>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _govukNotifyConfig = new GovukNotifyConfig();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(_govukNotifyConfig);
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());
        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _handler = new EditAdminUserCommandHandler(_mockLogger.Object, _mockUsersRepository.Object, _mockRolesRepository.Object, _mockIdentityNotificationService.Object);

    }

    [Fact]
    internal async Task TestPostUser_UserId_NotFoundAsync()
    {
        //Arrange
        var command = new EditAdminUserCommand() { Id = Guid.NewGuid()};

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(null));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    internal async Task TestPostUser_RoleId_NotFoundAsync()
    {
        //Arrange
        var role = new ChmmRole("A Role");
        var user = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { role });
        user.Activate();
        var command = new EditAdminUserCommand() { Id = user.Id, RoleIds = new List<Guid> { role.Id } };

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockRolesRepository.Setup(x => x.GetAll(r => command.RoleIds.Contains(r.Id), true)).Returns(Task.FromResult(new List<ChmmRole>()));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }


    [Fact]
    internal async Task TestPostUser_OkAsync()
    {
        //Arrange
        var role = new ChmmRole("A Role");
        var user = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { role });
        user.Activate();
        var command = new EditAdminUserCommand() { Id = user.Id, RoleIds = new List<Guid> { role.Id } };

        //_mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockRolesRepository.Setup(x => x.GetAll(r => command.RoleIds.Contains(r.Id), true)).Returns(Task.FromResult(new List<ChmmRole> { role }));
        _mockIdentityNotificationService.Setup(x => x.NotifyAdminUserEdited(user));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        _mockIdentityNotificationService.VerifyAll();
        Assert.NotNull(actionResult);
        Assert.IsType<OkResult>(actionResult);
    }
}
