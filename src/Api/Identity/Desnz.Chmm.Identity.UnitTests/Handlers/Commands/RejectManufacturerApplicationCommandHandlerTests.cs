using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Linq.Expressions;
using FluentAssertions;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Identity.UnitTests.Helpers;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Commands.ApproveManufacturerApplicationCommandHandlerTestsFixture;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands
{
    public class RejectManufacturerApplicationCommandHandlerTests
    {
        private Mock<ILogger<RejectManufacturerApplicationCommandHandler>> _mockLogger;
        private Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private Mock<IOrganisationDecisionCommentsRepository> _mockOrganisationCommentsRepository;
        private Mock<IOrganisationDecisionFilesRepository> _mockOrganisationDecisionFilesRepository;
        private Mock<IOrganisationStructureFilesRepository> _mockOrganisationStructureFilesRepository;
        private Mock<IUsersRepository> _mockUsersRepository;
        private Mock<IFileService> _mockFileService;
        private Mock<IIdentityNotificationService> _mockNotificationService;
        private Mock<ICurrentUserService> _mockUserService;
        private Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
        private Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
        private Mock<IUnitOfWork> _unitOfWork;

        private readonly GovukNotifyConfig _govukNotifyConfig;
        private readonly RejectManufacturerApplicationCommandHandler _handler;

        public RejectManufacturerApplicationCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<RejectManufacturerApplicationCommandHandler>>();
            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisationCommentsRepository = new Mock<IOrganisationDecisionCommentsRepository>(MockBehavior.Strict);
            _mockOrganisationDecisionFilesRepository = new Mock<IOrganisationDecisionFilesRepository>(MockBehavior.Strict);
            _mockOrganisationStructureFilesRepository = new Mock<IOrganisationStructureFilesRepository>(MockBehavior.Strict);
            _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
            _mockFileService = new Mock<IFileService>(MockBehavior.Strict);
            _mockNotificationService = new Mock<IIdentityNotificationService>(MockBehavior.Strict);
            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
            _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _govukNotifyConfig = new GovukNotifyConfig();

            _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(_govukNotifyConfig);
            _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());

            _handler = new RejectManufacturerApplicationCommandHandler(
                _mockLogger.Object,
                _mockOrganisationsRepository.Object,
                _mockOrganisationCommentsRepository.Object,
                _mockOrganisationDecisionFilesRepository.Object,
                _mockUsersRepository.Object,
                _mockFileService.Object,
                _mockNotificationService.Object,
                _mockUserService.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var organisationDto = GetEditOrganisationDto();
            var command = new RejectManufacturerApplicationCommand()
            {
                OrganisationId = organisationDto.Id
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
        public async Task ShouldReturnNoContent_When_OrganisationIsRejectedSuccessfully()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);
            var command = new RejectManufacturerApplicationCommand()
            {
                OrganisationId = organisationDto.Id,
                Comment = "Decision comment"
            };
            var expectedResult = Responses.NoContent();

            _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string>()));
            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true))
                .ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmUser, bool>>?>(), false, true)).ReturnsAsync(user);
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
            //_mockNotificationService.Setup(x => x.SendNotification(user.Email, _govukNotifyConfig.ManufacturerApplicationRejectedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null)).ReturnsAsync((EmailNotificationResponse?) null);
            //_mockNotificationService.Setup(x => x.SendNotification(admin.Email, _govukNotifyConfig.ManufacturerApplicationRejectedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null)).ReturnsAsync((EmailNotificationResponse?)null);

            _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string>()));

            _mockNotificationService.Setup(x => x.NotifyManufacturerRejected(It.IsAny<Organisation>())).Returns(Task.FromResult(Task.CompletedTask));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldCallFileUpload_When_DecisionFilesAreIncluded()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);

            var command = new RejectManufacturerApplicationCommand()
            {
                OrganisationId = organisationDto.Id,
                Comment = string.Empty
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true))
                .ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmUser, bool>>?>(), false, true)).ReturnsAsync(user);
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationDecisionFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationStructureFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationStructureFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
            _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(new FileService.FileUploadResponse(null, "")));
            _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "one.pdf" }));

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyManufacturerRejected(It.IsAny<Organisation>())).Returns(Task.FromResult(Task.CompletedTask));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            //_mockFileService.Verify(x => x.UploadFileAsync(
            //    It.Is<string>(s => s == "identity-organisation-rejections"),
            //    It.Is<string>(s => s.EndsWith("Decision File 1.txt")),
            //    It.IsAny<IFormFile>()), Times.Exactly(1));
            _mockOrganisationDecisionFilesRepository.Verify(x => x.Create(It.IsAny<OrganisationDecisionFile>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [Fact(Skip = "Feature not implemented yet")]
        public async Task ShouldCallFileUpload_When_StructureFilesAreIncluded()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);

            var command = new RejectManufacturerApplicationCommand()
            {
                OrganisationId = organisationDto.Id,
                Comment = string.Empty,
                AccountDecisionFiles = new List<IFormFile> { FileHelper.CreateFormFile("Decision File 1.txt") }
                // OrganisationStructureFiles = new List<IFormFile> { FileHelper.CreateFormFile("Structure File 1.txt") }
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true))
                .ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmUser, bool>>?>(), false, true)).ReturnsAsync(user);
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationDecisionFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationStructureFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationStructureFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationStructureFilesRepository.Setup(x => x.GetForOrganisation(It.IsAny<Guid>())).Returns(new List<OrganisationStructureFile>());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
            _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(new FileService.FileUploadResponse(null, "")));

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyManufacturerRejected(It.IsAny<Organisation>())).Returns(Task.FromResult(Task.CompletedTask));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockFileService.Verify(x => x.UploadFileAsync(
                It.Is<string>(s => s == "identity-organisation-approvals"),
                It.Is<string>(s => s.EndsWith("Decision File 1.txt")),
                It.IsAny<IFormFile>()), Times.Exactly(1));
            _mockOrganisationStructureFilesRepository.Verify(x => x.Create(It.IsAny<OrganisationStructureFile>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [Fact(Skip = "Feature not implemented yet")]
        public async Task ShouldCallFileUpload_When_BothFileTypesAreIncluded()
        {
            // Arrange
            var admin = GetMockAdminUser();
            var mockCurrentUser = GetMockUser(admin.Email);

            var organisation = GetMockOrganisation();
            var user = organisation.ChmmUsers.First();
            var addressId = organisation.Addresses.First().Id;

            var organisationDto = GetEditOrganisationDto(addressId, user.Id);

            var command = new RejectManufacturerApplicationCommand()
            {
                OrganisationId = organisationDto.Id,
                Comment = string.Empty,
                AccountDecisionFiles = new List<IFormFile> { FileHelper.CreateFormFile("Decision File 1.txt") }
                // OrganisationStructureFiles = new List<IFormFile> { FileHelper.CreateFormFile("Structure File 1.txt") }
            };
            var expectedResult = Responses.NoContent();

            _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>?>(), null, true))
                .ReturnsAsync(organisation);
            _mockUsersRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmUser, bool>>?>(), false, true)).ReturnsAsync(user);
            _mockOrganisationCommentsRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionComment>(), false)).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationDecisionFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationDecisionFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationStructureFilesRepository.Setup(x => x.Create(It.IsAny<OrganisationStructureFile>(), It.IsAny<bool>())).ReturnsAsync(Guid.NewGuid());
            _mockOrganisationStructureFilesRepository.Setup(x => x.GetForOrganisation(It.IsAny<Guid>())).Returns(new List<OrganisationStructureFile>());
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);
            _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(new FileService.FileUploadResponse(null, "")));

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockNotificationService.Setup(x => x.NotifyManufacturerRejected(It.IsAny<Organisation>())).Returns(Task.FromResult(Task.CompletedTask));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            //_mockFileService.Verify(x => x.UploadFileAsync(
            //    It.Is<string>(s => s == S3Constants.Buckets.OrganisationStructures),
            //    It.Is<string>(s => s == "Structure File 1.txt"),
            //    It.IsAny<IFormFile>()), Times.Exactly(1));
            //_mockOrganisationStructureFilesRepository.Verify(x => x.Create(It.IsAny<OrganisationStructureFile>(), It.IsAny<bool>()), Times.Exactly(1));
            _mockFileService.Verify(x => x.UploadFileAsync(
                It.Is<string>(s => s == "identity-organisation-approvals"),
                It.Is<string>(s => s.EndsWith("Decision File 1.txt")),
                It.IsAny<IFormFile>()), Times.Exactly(1));
            _mockOrganisationDecisionFilesRepository.Verify(x => x.Create(It.IsAny<OrganisationDecisionFile>(), It.IsAny<bool>()), Times.Exactly(1));
        }
    }
}