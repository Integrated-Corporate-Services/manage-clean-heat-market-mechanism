using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using System.Linq.Expressions;
using FluentAssertions;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Commands.ApproveManufacturerApplicationCommandHandlerTestsFixture;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands
{
    public class EditManufacturerCommandHandlerTests
    {
        private readonly Mock<ILogger<EditManufacturerCommandHandler>> _mockLogger;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<IOrganisationDecisionCommentsRepository> _mockOrganisationCommentsRepository;
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly Mock<IIdentityNotificationService> _mockNotificationService;
        private readonly Mock<ICurrentUserService> _mockUserService;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        private readonly GovukNotifyConfig _govukNotifyConfig;
        private readonly EditManufacturerCommandHandler _handler;

        public EditManufacturerCommandHandlerTests() 
        {
            _mockLogger = new Mock<ILogger<EditManufacturerCommandHandler>>();
            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisationCommentsRepository = new Mock<IOrganisationDecisionCommentsRepository>(MockBehavior.Strict);
            _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
            _mockNotificationService = new Mock<IIdentityNotificationService>(MockBehavior.Strict);
            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _unitOfWork = new Mock<IUnitOfWork>();

            _govukNotifyConfig = new GovukNotifyConfig();


            _handler = new EditManufacturerCommandHandler(
                _mockLogger.Object, 
                _mockOrganisationsRepository.Object, 
                _mockOrganisationCommentsRepository.Object,
                _mockUsersRepository.Object,
                _mockNotificationService.Object, 
                _mockUserService.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var organisationDto = GetEditOrganisationDto();
            var command = new EditManufacturerCommand()
            {
                OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto)
            };
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {organisationDto.Id}");

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true))
                .ReturnsAsync((Organisation?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnNoContent_When_Organisation_WIth_NoComment_IsApprovedSuccessfully()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);
            var command = new EditManufacturerCommand()
            {
                OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto)
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true)).ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.GetAdmins(false)).ReturnsAsync(new List<ChmmUser>() { admin });
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyOrganisationEdited(organisation)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnNoContent_When_Update_Creates_New_Address()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);

            organisationDto.Addresses.Add(new()
            {
                LineOne = "Test line one",
                City = "Test city",
                Postcode = "Test postcode",
                IsUsedAsLegalCorrespondence = true
            });

            var command = new EditManufacturerCommand()
            {
                OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto)
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true)).ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.GetAdmins(false)).ReturnsAsync(new List<ChmmUser>() { admin });
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());

            var newAddressId = Guid.NewGuid();
            _mockOrganisationsRepository.Setup(x => x.CreateAddress(It.IsAny<Guid>(), It.IsAny<OrganisationAddress>(), It.IsAny<bool>())).ReturnsAsync(newAddressId);


            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyOrganisationEdited(organisation)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnNoContent_When_Organisation_WIth_Comment_IsApprovedSuccessfully()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);
            var command = new EditManufacturerCommand()
            {
                OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto),
                Comment = "Manufacturer updated" 
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true)).ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.GetAdmins(false)).ReturnsAsync(new List<ChmmUser>() { admin });
            _mockUsersRepository.Setup(x => x.Get(It.Is<Expression<Func<ChmmUser, bool>>>(y => y.Compile()(user)), false, false)).Returns(Task.FromResult(user));
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyOrganisationEdited(organisation)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
