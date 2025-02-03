using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands
{
    public class DeactivateManufacturerUserCommandHandlerTests
    {
        private readonly Mock<ILogger<BaseRequestHandler<DeactivateManufacturerUserCommand, ActionResult>>> _mockLogger;
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        private readonly DeactivateManufacturerUserCommandHandler _handler;

        private readonly ChmmUser _mockUser;
        private readonly ChmmUser _mockDeactivatedUser;
        private Guid _existingOrganisationId;
        private Guid _existingOtherOrganisationId;
        private Guid _existingUserId;
        private Guid _deactivatedUserId;
        private Guid _myUserId = Guid.NewGuid();

        public DeactivateManufacturerUserCommandHandlerTests() 
        {
            var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);

            _mockLogger = new Mock<ILogger<BaseRequestHandler<DeactivateManufacturerUserCommand, ActionResult>>>();

            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, _myUserId.ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(() => null);
            var mockOrganisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.Manufacturer) });
            _existingOrganisationId = mockOrganisation.Id;
            _mockOrganisationsRepository.Setup(o => o.GetById(_existingOrganisationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mockOrganisation);

            var mockOtherOrganisation = new Organisation(GetCreateOrganisationDto(), new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.Manufacturer) });
            _existingOtherOrganisationId = mockOtherOrganisation.Id;
            _mockOrganisationsRepository.Setup(o => o.GetById(_existingOtherOrganisationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mockOtherOrganisation);

            _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
            _mockUser = new ChmmUser(new CreateManufacturerUserDto() {  Name = "Name", Email = "user@example.com"}, new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.Manufacturer) }, _existingOrganisationId );
            _existingUserId = _mockUser.Id;
            _mockUser.Activate();
            _mockUsersRepository.Setup(u => u.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(() => null);
            _mockUsersRepository.Setup(u => u.GetById(_existingUserId, It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(_mockUser);

            _mockDeactivatedUser = new ChmmUser(new CreateManufacturerUserDto() { Name = "Name", Email = "user@example.com" }, new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.Manufacturer) }, _existingOrganisationId);
            _mockDeactivatedUser.Deactivate();
            _deactivatedUserId = _mockDeactivatedUser.Id;
            _mockUsersRepository.Setup(u => u.GetById(_deactivatedUserId, It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(_mockDeactivatedUser);

            _mockUsersRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _handler = new DeactivateManufacturerUserCommandHandler(
                _mockLogger.Object,
                _mockUsersRepository.Object,
                _mockOrganisationsRepository.Object,
                _mockCurrentUserService.Object);
        }

        [Fact]
        public async void When_OrganisationDoesNotExist_ReturnBadRequest()
        {
            var orgId = Guid.NewGuid();
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {orgId}");

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(_existingUserId, orgId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DeactivatingMyself_ReturnBadRequest()
        {
            var expectedResult = Responses.BadRequest($"You cannot deactivate yourself");

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(_myUserId, _existingOrganisationId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DeactivatingDeactivatedUser_ReturnBadRequest()
        {
            var expectedResult = Responses.BadRequest($"User with Id: {_deactivatedUserId} has an invalid status: Inactive");

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(_deactivatedUserId, _existingOrganisationId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DeactivatingUserInDifferentOrganisation_ReturnBadRequest()
        {
            var expectedResult = Responses.BadRequest($"User with Id: {_existingUserId} is not part of Organisation {_existingOrganisationId} and cannot be deactivated as you do not have permission");

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(_existingUserId, _existingOtherOrganisationId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_UserDoesNotExist_ReturnBadRequest()
        {
            var userId = Guid.NewGuid();
            var expectedResult = Responses.NotFound($"Failed to get User with Id: {userId}");

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(userId, _existingOrganisationId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DetailsOK_UserDeactivated()
        {
            var expectedResult = Responses.NoContent();

            var result = await _handler.Handle(new DeactivateManufacturerUserCommand(_existingUserId, _existingOrganisationId), CancellationToken.None);
            var actionResult = result;

            actionResult.Should().BeOfType(expectedResult.GetType());
            _mockUsersRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
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
}
