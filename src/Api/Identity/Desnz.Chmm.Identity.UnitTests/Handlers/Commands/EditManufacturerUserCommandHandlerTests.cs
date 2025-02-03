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
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Common.Extensions;
using System.IdentityModel.Tokens.Jwt;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class EditManufacturerUserCommandHandlerTests
{
    private Mock<IUsersRepository> _mockUsersRepository;
    private Mock<IRolesRepository> _mockRolesRepository;
    private Mock<IIdentityNotificationService> _mockIdentityNotificationService;
    private Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private Mock<ILogger<EditManufacturerUserCommandHandler>> _mockLogger;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<ICurrentUserService> _mockCurrentUserService;
    private Mock<IOrganisationsRepository> _mockOrganisationsRepository;

    private EditManufacturerUserCommandHandler _handler;
    private GovukNotifyConfig _govukNotifyConfig;

    public EditManufacturerUserCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EditManufacturerUserCommandHandler>>();
        _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
        _mockRolesRepository = new Mock<IRolesRepository>(MockBehavior.Strict);
        _mockIdentityNotificationService = new Mock<IIdentityNotificationService>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);

        _govukNotifyConfig = new GovukNotifyConfig();

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(_govukNotifyConfig);
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());
        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _handler = new EditManufacturerUserCommandHandler(_mockLogger.Object,
                                                          _mockUsersRepository.Object,
                                                          _mockRolesRepository.Object,
                                                          _mockIdentityNotificationService.Object,
                                                          _mockCurrentUserService.Object,
                                                          _mockOrganisationsRepository.Object);

    }

    [Fact]
    internal async Task TestPostUser_UserId_NotFoundAsync()
    {
        //Arrange
        var command = new EditManufacturerUserCommand() { Id = Guid.NewGuid()};

        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(null));

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
        var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
        var organisationId = organisation.Id;
        var role = new ChmmRole("Manufacturer");

        var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
        var user = new ChmmUser(dto, new List<ChmmRole> { role }, organisationId);

        user.Activate();
        var command = new EditManufacturerUserCommand() { Id = user.Id, OrganisationId = organisationId };

        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockIdentityNotificationService.Setup(x => x.NotifyManufacturerUserEdited(user));


        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organisation);


        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        _mockIdentityNotificationService.VerifyAll();
        Assert.NotNull(actionResult);
        Assert.IsType<OkResult>(actionResult);
    }


    [Fact]
    internal async Task TestPostAdminUser_OkAsync()
    {
        //Arrange
        var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
        var organisationId = organisation.Id;
        var role = new ChmmRole("Manufacturer");

        var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
        var user = new ChmmUser(dto, new List<ChmmRole> { role }, organisationId);

        user.Activate();

        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Manufacturer"));
        identity.AddClaim(new Claim(Claims.OrganisationId , organisationId.ToString()));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(context.User);

        var command = new EditManufacturerUserCommand() { Id = context.User.GetUserId().Value, OrganisationId = organisationId };

        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockIdentityNotificationService.Setup(x => x.NotifyManufacturerUserEdited(user));



        _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organisation);


        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        _mockIdentityNotificationService.VerifyAll();
        Assert.NotNull(actionResult);
        Assert.IsType<OkResult>(actionResult);
    }


    [Fact]
    internal async Task TestPostManufacturerUser_NonManufacturerUserEditingAtempted()
    {
        //Arrange
        var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
        var organisationId = organisation.Id;
        var role = new ChmmRole("Manufacturer");

        var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
        var user = new ChmmUser(dto, new List<ChmmRole> { role }, null);

        user.Activate();

        var command = new EditManufacturerUserCommand() { Id = user.Id, OrganisationId = organisationId };

        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockIdentityNotificationService.Setup(x => x.NotifyManufacturerUserEdited(user));

        _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organisation);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    internal async Task TestPostAdminUser_CannotEditUserAcrossOrganisation()
    {
        //Arrange
        var organisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(Roles.Manufacturer) });
        var organisationId = organisation.Id;
        var role = new ChmmRole("Manufacturer");

        var dto = new CreateManufacturerUserDto() { Email = "fgfgf" };
        var user = new ChmmUser(dto, new List<ChmmRole> { role }, organisationId);

        user.Activate();

        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Manufacturer"));
        identity.AddClaim(new Claim(Claims.OrganisationId, Guid.NewGuid().ToString()));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(context.User);

        var command = new EditManufacturerUserCommand() { Id = context.User.GetUserId().Value, OrganisationId = organisationId };

        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
        _mockUsersRepository.Setup(x => x.Get(u => u.Id == command.Id, true, true)).Returns(Task.FromResult<ChmmUser?>(user));
        _mockIdentityNotificationService.Setup(x => x.NotifyManufacturerUserEdited(user));

        _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organisation);


        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    private static CreateOrganisationDto GetCreateOrganisationDto()
    {
        var editOrganisationDto = new CreateOrganisationDto()
        {
            Addresses = new List<CreateOrganisationAddressDto>()
                {
                    new()
                    {
                        LineOne = "Test line one",
                        City = "Test city",
                        Postcode = "Test postcode",
                        IsUsedAsLegalCorrespondence = false
                    }
                },
            Users = new List<CreateManufacturerUserDto>()
                {
                    new()
                    {
                        TelephoneNumber = "012345678901",
                        IsResponsibleOfficer = true,
                        Name = "Test name",
                        Email = "test@test",
                        JobTitle = "Test job title"
                    }
                },
            IsOnBehalfOfGroup = false,
            ResponsibleUndertaking = new ResponsibleUndertakingDto()
            {
                Name = "Test name",
            },
            IsFossilFuelBoilerSeller = false,
            CreditContactDetails = new CreditContactDetailsDto(),
        };
        return editOrganisationDto;
    }
}
