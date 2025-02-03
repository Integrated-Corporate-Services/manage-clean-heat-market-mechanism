using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class InviteManufacturerUserCommandHandlerTests
{
    private readonly Mock<ILogger<BaseRequestHandler<InviteManufacturerUserCommand, ActionResult<Guid>>>> _mockLogger;
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<IRolesRepository> _mockRolesRepository;
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IIdentityNotificationService> _mockNotificationService;

    private readonly InviteManufacturerUserCommandHandler handler;

    private readonly string _existingUserEmailAddress = "existing@example.com";
    private readonly string _newUserEmailAddress = "new@example.com";
    private readonly Guid _existingOrganisationId = Guid.NewGuid();

    public InviteManufacturerUserCommandHandlerTests()
    {
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);

        _mockLogger = new Mock<ILogger<BaseRequestHandler<InviteManufacturerUserCommand, ActionResult<Guid>>>>();

        _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
        var existingUser = new ChmmUser(new CreateManufacturerUserDto
        {
            Email = _existingUserEmailAddress,
            IsResponsibleOfficer = false,
            JobTitle = "Job",
            Name = "Existing",
            ResponsibleOfficerOrganisationName = "Org",
            TelephoneNumber = "0790382734"
        }, new List<ChmmRole> { manufacturerRole }, _existingOrganisationId);
        _mockUsersRepository.Setup(u => u.GetByEmail(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(() => null);
        _mockUsersRepository.Setup(u => u.GetByEmail(_existingUserEmailAddress, It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(existingUser);
        _mockUsersRepository.Setup(u => u.Create(It.IsAny<ChmmUser>(), true)).ReturnsAsync(Guid.NewGuid());

        _mockRolesRepository = new Mock<IRolesRepository>(MockBehavior.Strict);
        _mockRolesRepository.Setup(r => r.GetByName(IdentityConstants.Roles.Manufacturer, It.IsAny<bool>())).ReturnsAsync(manufacturerRole);

        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
        _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(() => null);
        var organisaton = new Organisation(new Common.Dtos.Organisation.CreateOrganisationDto
        {
            Addresses = new List<Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto>(),
            CreditContactDetails = new Common.Dtos.CreditContactDetailsDto { Name = "Name", Email = "test@example.com", TelephoneNumber = "000" },
            HeatPumpBrands = new string[] { },
            IsFossilFuelBoilerSeller = true,
            IsOnBehalfOfGroup = true,
            ResponsibleUndertaking = new Common.Dtos.ResponsibleUndertakingDto { Name = "Org Name" },
            Users = new List<CreateManufacturerUserDto> {
                new CreateManufacturerUserDto
                {
                    Email = "someone@example.com",
                    JobTitle = "Job",
                    IsResponsibleOfficer = true,
                    Name = "Name",
                    ResponsibleOfficerOrganisationName = "Name",
                    TelephoneNumber = "000"
                }
            }
        }, new List<ChmmRole> { manufacturerRole });
        _mockOrganisationsRepository.Setup(o => o.GetById(_existingOrganisationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organisaton);

        _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
        _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

        _mockNotificationService = new Mock<IIdentityNotificationService>();

        handler = new InviteManufacturerUserCommandHandler(
            _mockLogger.Object,
            _mockUsersRepository.Object,
            _mockRolesRepository.Object,
            _mockOrganisationsRepository.Object,
            _mockCurrentUserService.Object,
            _mockNotificationService.Object);
    }

    [Fact]
    public async void When_UserNotLoggedIn_ReturnUnauthorized()
    {
        _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns((ClaimsPrincipal?)null);

        var expectedResult = Responses.Unauthorized($"You are not authenticated");

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _existingUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_UserIdNotPresent_ReturnUnauthorized()
    {
        _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(new ClaimsPrincipal());

        var expectedResult = Responses.Unauthorized($"You are not authenticated");

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _existingUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_EmailAlreadyExists_UserIsAdmin_ReturnBadRequest()
    {
        var expectedResult = Responses.BadRequest($"User already exists with email \"{_existingUserEmailAddress}\"");

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _existingUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_EmailAlreadyExists_UserIsSameOrganisation_ReturnBadRequest()
    {
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(Claims.OrganisationId, _existingOrganisationId.ToString()),
                new Claim(ClaimTypes.Role, "Manufacturer")
            }));
        _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

        var expectedResult = Responses.BadRequest($"User already exists with email \"{_existingUserEmailAddress}\"");

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _existingUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_EmailAlreadyExists_UserIsDifferentOrganisation_ReturnBadRequest()
    {
        var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(Claims.OrganisationId, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Manufacturer")
            }));
        _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

        var expectedResult = Responses.BadRequest("Unable to save details, please contact the Administrator");

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _existingUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_ManufacturerRolesCannotBeFound_ReturnBadRequest()
    {
        var expectedResult = Responses.NotFound($"Failed to get Role with Id: {IdentityConstants.Roles.Manufacturer}");

        _mockRolesRepository.Setup(r => r.GetByName(IdentityConstants.Roles.Manufacturer, It.IsAny<bool>())).ReturnsAsync(() => null);

        var result = await handler.Handle(new InviteManufacturerUserCommand(Guid.NewGuid(), "name", _newUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_OrganisationDoesNotExist_ReturnBadRequest()
    {
        var organisationId = Guid.NewGuid();
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}");

        var result = await handler.Handle(new InviteManufacturerUserCommand(organisationId, "name", _newUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void When_InvitingUserDoesNotExist_ReturnBadRequest()
    {
        var organisationId = Guid.NewGuid();
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}");
        _mockUsersRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false))
            .ReturnsAsync((ChmmUser?)null);

        var result = await handler.Handle(new InviteManufacturerUserCommand(organisationId, "name", _newUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockUsersRepository.Verify(x => x.Create(It.IsAny<ChmmUser>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async void When_DetailsOK_ReturnCreated()
    {
        const string invitingUserName = "Jane Doe";
        var expectedResult = Responses.Created(Guid.NewGuid());
        _mockUsersRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false))
            .ReturnsAsync(new ChmmUser(invitingUserName, "jane@manufacturer.co.uk", new List<ChmmRole>()));

        var result = await handler.Handle(new InviteManufacturerUserCommand(_existingOrganisationId, "name", _newUserEmailAddress, "Job", "0192308712"), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeOfType(expectedResult.GetType());
        _mockUsersRepository.Verify(x => x.Create(It.IsAny<ChmmUser>(), It.IsAny<bool>()), Times.Once());
        _mockNotificationService.Verify(x => x.NotifyManufacturerUserInvited(It.IsAny<ChmmUser>(), invitingUserName), Times.Once);
    }
}
