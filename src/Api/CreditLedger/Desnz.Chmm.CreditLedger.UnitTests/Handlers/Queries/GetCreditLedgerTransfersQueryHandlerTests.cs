using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Handlers.Queries;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries
{
    public class GetCreditLedgerTransfersQueryHandlerTests : TestClaimsBase
    {
        private Mock<ISchemeYearService> _mockSchemeYearService;
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private Mock<IOrganisationService> _mockOrganisationService;
        private Mock<IUserService> _mockUserService;

        private static readonly Guid _existingOrganisationId = Guid.NewGuid();
        private static readonly Guid _pendingOrganisationId = Guid.NewGuid();
        private static readonly Guid _existingUserId = Guid.NewGuid();

        private GetCreditLedgerTransfersQueryHandler _handler;
        public GetCreditLedgerTransfersQueryHandlerTests()
        {
            var mockLogger = new Mock<ILogger<BaseRequestHandler<GetCreditLedgerTransfersQuery, ActionResult<CreditLedgerTransfersDto>>>>();

            // Transaction repository
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);

            // User Service
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            var mockCurrentUser = GetMockManufacturerUser(_existingUserId, _existingOrganisationId);
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

            _mockSchemeYearService = new Mock<ISchemeYearService>();
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            // Organisation Service
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            var activeOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Active
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_existingOrganisationId)).ReturnsAsync(activeOrganisation);

            // User Service
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);

            var validator = new RequestValidator(
                _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                _mockSchemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new GetCreditLedgerTransfersQueryHandler(
                mockLogger.Object,
                _mockTransactionRepository.Object,
                _mockOrganisationService.Object,
                _mockUserService.Object,
                validator);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
        {
            // Arrange
            var missingOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(missingOrganisation);

            var query = new GetCreditLedgerTransfersQuery(Guid.NewGuid(), SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
        {
            // Arrange
            var pendingOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Pending
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_pendingOrganisationId)).ReturnsAsync(pendingOrganisation);

            var query = new GetCreditLedgerTransfersQuery(_pendingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_ManufacturerUsersNotFound()
        {
            // Arrange
            var getOrganisationsResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
            {
                new ViewOrganisationDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsNonSchemeParticipant = false,
                    Status = OrganisationConstants.Status.Active
                }
            }, null);
            _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(It.IsAny<Guid>(), null)).ReturnsAsync(getOrganisationsResponse);

            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_GetManufacturerNamesNotFound()
        {
            var manufacturerUserId = Guid.NewGuid();

            // Arrange
            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>
            {
               new ViewManufacturerUserDto
               {
                   Id = manufacturerUserId,
                   Name = "Test Name"
               }
            }, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var transactions = new List<Transaction>();
            _mockTransactionRepository.Setup(x => x.GetTransferTransactions(_existingOrganisationId, SchemeYearConstants.Id, false)).ReturnsAsync(transactions);

            var getOrganisationsResponse = new HttpObjectResponse<List<OrganisationNameDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(x => x.GetManufacturerNames(It.IsAny<OrganisationNameLookupQuery>(), null)).ReturnsAsync(getOrganisationsResponse);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnOK_When_NoTransactions()
        {
            var organisationId = Guid.NewGuid();
            var manufacturerUserId = Guid.NewGuid();

            // Arrange
            var getOrganisationsResponse = new HttpObjectResponse<List<OrganisationNameDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationNameDto>
            {
                new OrganisationNameDto
                {
                    Id = organisationId,
                    Name = "Test"
                }
            }, null);
            _mockOrganisationService.Setup(x => x.GetManufacturerNames(It.IsAny<OrganisationNameLookupQuery>(), null)).ReturnsAsync(getOrganisationsResponse);

            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>
            {
               new ViewManufacturerUserDto
               {
                   Id = manufacturerUserId,
                   Name = "Test Name"
               }
            }, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var transactions = new List<Transaction>();
            _mockTransactionRepository.Setup(x => x.GetTransferTransactions(_existingOrganisationId, SchemeYearConstants.Id, false)).ReturnsAsync(transactions);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.Value.TransfersOut.Count());
            Assert.Equal(0, result.Value.TransfersIn.Count());
        }

        [Fact]
        public async Task ShouldReturnOK_When_TransferOut()
        {
            var organisationId = Guid.NewGuid();
            var manufacturerUserId = Guid.NewGuid();

            // Arrange
            var getOrganisationsResponse = new HttpObjectResponse<List<OrganisationNameDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationNameDto>
            {
                new OrganisationNameDto
                {
                    Id = organisationId,
                    Name = "Test"
                }
            }, null);
            _mockOrganisationService.Setup(x => x.GetManufacturerNames(It.IsAny<OrganisationNameLookupQuery>(), null)).ReturnsAsync(getOrganisationsResponse);

            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>
            {
               new ViewManufacturerUserDto
               {
                   Id = manufacturerUserId,
                   Name = "Test Name"
               }
            }, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var transactions = new List<Transaction>
            {
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, organisationId, 10, manufacturerUserId, DateTime.Now)
            };
            _mockTransactionRepository.Setup(x => x.GetTransferTransactions(_existingOrganisationId, SchemeYearConstants.Id, false)).ReturnsAsync(transactions);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.Value.TransfersOut.Count());
            var transfer = result.Value.TransfersOut.First();
            Assert.Equal("Test", transfer.OrganisationName);
            Assert.Equal(-10, transfer.Value);
            Assert.Equal("Test Name", transfer.TransferredBy);

            Assert.Equal(0, result.Value.TransfersIn.Count());
        }

        [Fact]
        public async Task ShouldReturnOK_When_TransferIn()
        {
            var organisationId = Guid.NewGuid();
            var manufacturerUserId = Guid.NewGuid();

            // Arrange
            var getOrganisationsResponse = new HttpObjectResponse<List<OrganisationNameDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationNameDto>
            {
                new OrganisationNameDto
                {
                    Id = organisationId,
                    Name = "Test"
                }
            }, null);
            _mockOrganisationService.Setup(x => x.GetManufacturerNames(It.IsAny<OrganisationNameLookupQuery>(), null)).ReturnsAsync(getOrganisationsResponse);

            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>
            {
               new ViewManufacturerUserDto
               {
                   Id = manufacturerUserId,
                   Name = "Test Name"
               }
            }, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var transactions = new List<Transaction>
            {
                new Transaction(SchemeYearConstants.Id, organisationId, _existingOrganisationId, 10, Guid.NewGuid(), DateTime.Now)
            };
            _mockTransactionRepository.Setup(x => x.GetTransferTransactions(_existingOrganisationId, SchemeYearConstants.Id, false)).ReturnsAsync(transactions);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.Value.TransfersOut.Count());

            Assert.Equal(1, result.Value.TransfersIn.Count());
            var transfer = result.Value.TransfersIn.First();
            Assert.Equal("Test", transfer.OrganisationName);
            Assert.Equal(10, transfer.Value);
        }

        [Fact]
        public async Task ShouldReturnOK_When_TransferInAndOut()
        {
            var organisationId = Guid.NewGuid();
            var manufacturerUserId = Guid.NewGuid();

            // Arrange
            var getOrganisationsResponse = new HttpObjectResponse<List<OrganisationNameDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationNameDto>
            {
                new OrganisationNameDto
                {
                    Id = organisationId,
                    Name = "Test"
                }
            }, null);
            _mockOrganisationService.Setup(x => x.GetManufacturerNames(It.IsAny<OrganisationNameLookupQuery>(), null)).ReturnsAsync(getOrganisationsResponse);

            var getManufacturerUsers = new HttpObjectResponse<List<ViewManufacturerUserDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewManufacturerUserDto>
            {
               new ViewManufacturerUserDto
               {
                   Id = manufacturerUserId,
                   Name = "Test Name"
               }
            }, null);
            _mockUserService.Setup(x => x.GetManufacturerUsers(It.IsAny<Guid>())).ReturnsAsync(getManufacturerUsers);

            var transactions = new List<Transaction>
            {
                new Transaction(SchemeYearConstants.Id, organisationId, _existingOrganisationId, 10, Guid.NewGuid(), DateTime.Now),
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, organisationId, 20, manufacturerUserId, DateTime.Now)
            };
            _mockTransactionRepository.Setup(x => x.GetTransferTransactions(_existingOrganisationId, SchemeYearConstants.Id, false)).ReturnsAsync(transactions);

            var query = new GetCreditLedgerTransfersQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.Value.TransfersOut.Count());
            var transferOut = result.Value.TransfersOut.First();
            Assert.Equal("Test", transferOut.OrganisationName);
            Assert.Equal(-20, transferOut.Value);
            Assert.Equal("Test Name", transferOut.TransferredBy);

            Assert.Equal(1, result.Value.TransfersIn.Count());
            var transferIn = result.Value.TransfersIn.First();
            Assert.Equal("Test", transferIn.OrganisationName);
            Assert.Equal(10, transferIn.Value);
        }
    }
}
