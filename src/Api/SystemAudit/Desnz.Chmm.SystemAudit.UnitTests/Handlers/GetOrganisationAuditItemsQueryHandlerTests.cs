using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.SystemAudit.Api.Handlers.Queries;
using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.SystemAudit.Common.Queries;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using System.Net;
using Desnz.Chmm.Common.Mediator;
using FluentAssertions;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Common.ValueObjects;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Testing.Common;

namespace Desnz.Chmm.SystemAudit.UnitTests.Handlers
{
    public class GetOrganisationAuditItemsQueryHandlerTests : TestClaimsBase
    {
        private Mock<ILogger<GetOrganisationAuditItemsQueryHandler>> _mockLogger;
        private Mock<IAuditItemRepository> _mockAuditItemRepository;
        private Mock<IOrganisationService> _mockOrganisationService;
        private Mock<IUserService> _mockUserService;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetOrganisationAuditItemsQueryHandler _handler;

        public GetOrganisationAuditItemsQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetOrganisationAuditItemsQueryHandler>>();
            _mockAuditItemRepository = new Mock<IAuditItemRepository>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockCurrentUserService.Setup(x => x.CurrentUser).Returns(GetMockAdminUser(Guid.NewGuid()));

            var validator = new RequestValidator(
            _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                new Mock<ISchemeYearService>(MockBehavior.Strict).Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new GetOrganisationAuditItemsQueryHandler(
                _mockLogger.Object,
                _mockAuditItemRepository.Object,
                _mockUserService.Object,
                validator);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_GetOrganisationFails()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetOrganisationAuditItemsQuery(organisationId);
            
            var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.InternalServerError), null);
            _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

            var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_GetManufacturerUsersFails()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetOrganisationAuditItemsQuery(organisationId);

            var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto());
            _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

            var getManufacturerUsersResponse = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.InternalServerError), null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsersResponse);

            var expectedResult = Responses.BadRequest($"Failed to get Organiastion Users for Organisation with Id: {organisationId}, problem: null");

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        internal async Task ShouldReturnBadRequest_When_GetAdminUsers()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetOrganisationAuditItemsQuery(organisationId);

            var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto());
            _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

            var getManufacturerUsersResponse = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>());
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsersResponse);

            var getAdminUsersResponse = new HttpObjectResponse<List<ChmmUserDto>>(new HttpResponseMessage(HttpStatusCode.InternalServerError), null);
            _mockUserService.Setup(x => x.GetAdminUsers()).ReturnsAsync(getAdminUsersResponse);

            var expectedResult = Responses.BadRequest("Failed to get Admin Users, problem: null");

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_Successful()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetOrganisationAuditItemsQuery(organisationId);

            var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto());
            _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

            var userId = Guid.NewGuid();
            var mockUsers = new List<ViewManufacturerUserDto> { new ViewManufacturerUserDto { Id = userId, Name = "Test user name" } };
            var getManufacturerUsersResponse = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK),
                mockUsers);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsersResponse);

            var getAdminUsersResponse = new HttpObjectResponse<List<ChmmUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ChmmUserDto>());
            _mockUserService.Setup(x => x.GetAdminUsers()).ReturnsAsync(getAdminUsersResponse);

            var mockAuditItem = new AuditItem("", true, "", userId, Guid.NewGuid(), "Test friendly name", "", new { TestField = "Test field value" }, 1);
            _mockAuditItemRepository.Setup(x => x.GetAuditItemsForOrganisation(It.IsAny<Guid>())).ReturnsAsync(new List<AuditItem> { mockAuditItem });

            var users = new List<ChmmUserDto>();
            users.AddRange(mockUsers);
            var expectedResult = new List<AuditItemDto>
            {
                new AuditItemDto(users, mockAuditItem)
            };
            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}
