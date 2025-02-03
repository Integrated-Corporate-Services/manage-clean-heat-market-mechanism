using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using System.Text;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Handlers.Query
{
    public class DownloadRequestDataFileQueryHandlerTests
    {
        private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;
        private Mock<ILogger<DownloadRequestDataFileQueryHandler>> _logger;

        public DownloadRequestDataFileQueryHandlerTests()
        {
            _logger = new Mock<ILogger<DownloadRequestDataFileQueryHandler>>();
            _mockCreditLedgerService = new Mock<ICreditLedgerService>();
        }

        [Fact]
        internal async Task Request_Returns_Correct_Data()
        {
            // Setup reference data
            var requestId = Guid.NewGuid();

            var referenceRepository = new Mock<IMcsReferenceDataRepository>();
            var techTypes = new List<TechnologyType> { new TechnologyType { Id = 1, Description = "Desc 1" }, new TechnologyType { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllTechnologyTypes(null, false)).ReturnsAsync(techTypes);
            var newBuilds = new List<NewBuildOption> { new NewBuildOption { Id = 1, Description = "Desc 1" }, new NewBuildOption { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllNewBuildOptions(null, false)).ReturnsAsync(newBuilds);

            // Setup installation data
            var repository = new Mock<IInstallationRequestRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var installRequest = new InstallationRequest()
            {
                RequestDate = now,
                EndDate = now,
                StartDate = now.AddDays(1),
                IsNewBuildIds = new[] { 1, 2 },
                TechnologyTypeIds = new[] { 1, 2 }
            };
            var requests = new List<InstallationRequest>() { installRequest };
            repository.Setup(r => r.GetAll(null, false)).ReturnsAsync(requests);
            repository.Setup(r => r.Get(It.IsAny<Guid?>(), It.IsAny<bool>())).ReturnsAsync(installRequest);

            // setup mcs data
            var dataRepository = new Mock<IMcsInstallationDataRepository>();
            var installData = new List<HeatPumpInstallation>() {
                new HeatPumpInstallation(){
                    AirTypeTechnologyId = 1,
                    AlternativeHeatingAgeId = 1,
                    AlternativeHeatingFuelId = 1,
                    AlternativeHeatingSystemId = 1,
                    CertificatesCount = 1,
                    CommissioningDate = now,
                    HeatPumpProducts = new List<HeatPumpProduct>()
                    {
                        new HeatPumpProduct()
                        {
                            Code = "Code",
                            Id = 1,
                            ManufacturerId = 1,
                            ManufacturerName = "Manufacturer Name",
                            Name = "Name",
                        }
                    },
                    Id = requestId,
                    InstallationRequestId = Guid.NewGuid(),
                    IsAlternativeHeatingSystemPresent = false,
                    IsHybrid = false,
                    IsNewBuildId = 1,
                    Mpan = "Mpan",
                    RenewableSystemDesignId = 1,
                    TechnologyTypeId = 1,
                    TotalCapacity = 1, 
                    MidId = 321,
                    IsSystemSelectedAsMCSTechnology = true,
                },
                new HeatPumpInstallation(){MidId = 322}
            };
            dataRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<HeatPumpInstallation, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(installData);

            var creditsResponse = new HttpObjectResponse<List<HeatPumpInstallationCreditsDto>>(
                new HttpResponseMessage(HttpStatusCode.OK),
                new List<HeatPumpInstallationCreditsDto> { new(321, 456) },
                null);

            _mockCreditLedgerService.Setup(x => x.GetInstallationCredits(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<string>()))
                .Returns(Task.FromResult(creditsResponse));

            var handler = new DownloadRequestDataFileQueryHandler(_logger.Object, repository.Object, dataRepository.Object, _mockCreditLedgerService.Object);

            var actionResult = await handler.Handle(new DownloadRequestDataFileQuery(installRequest.Id), CancellationToken.None);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<Stream>>(actionResult);

            var stream = actionResult.Result as FileContentResult;
            Assert.NotNull(stream);

            var data = stream.FileContents;
            string resultString = Encoding.UTF8.GetString(data);

            Assert.NotNull(resultString);

            var lines = resultString.Split(Environment.NewLine);
            Assert.Equal(
                $"321,1,1,False,True,1,1,1,{now.ToString("dd/MM/yyyy")},Mpan,1,1,False,1,1,\"[\"\"ID: 1 | MCS Product Number: Code | Product Name: Name | Licence Holder: Manufacturer Name\"\"]\",456",
                lines[1].Replace("\r", "")); // Remove any stray newlines.
            Assert.Equal("322,,,,,,,,,,,,,,,,0",lines[2].Replace("\r", ""));

        }

        private void SetupGetAllCreditBalancesBadRequest()
        {
            var badRequestResponse = new HttpObjectResponse<List<HeatPumpInstallationCreditsDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockCreditLedgerService.Setup(x => x.GetInstallationCredits(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<string>()))
                .Returns(Task.FromResult(badRequestResponse));
        }

        [Fact]
        internal async Task Request_Returns_BadRequest_When_NoCreditsFetched()
        {
            // Setup reference data
            var requestId = Guid.NewGuid();

            var referenceRepository = new Mock<IMcsReferenceDataRepository>();
            var techTypes = new List<TechnologyType> { new TechnologyType { Id = 1, Description = "Desc 1" }, new TechnologyType { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllTechnologyTypes(null, false)).ReturnsAsync(techTypes);
            var newBuilds = new List<NewBuildOption> { new NewBuildOption { Id = 1, Description = "Desc 1" }, new NewBuildOption { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllNewBuildOptions(null, false)).ReturnsAsync(newBuilds);

            // Setup installation data
            var repository = new Mock<IInstallationRequestRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var installRequest = new InstallationRequest()
            {
                RequestDate = now,
                EndDate = now,
                StartDate = now.AddDays(1),
                IsNewBuildIds = new[] { 1, 2 },
                TechnologyTypeIds = new[] { 1, 2 }
            };
            var requests = new List<InstallationRequest>() { installRequest };
            repository.Setup(r => r.GetAll(null, false)).ReturnsAsync(requests);
            repository.Setup(r => r.Get(It.IsAny<Guid?>(), It.IsAny<bool>())).ReturnsAsync(installRequest);

            // setup mcs data
            var dataRepository = new Mock<IMcsInstallationDataRepository>();
            var installData = new List<HeatPumpInstallation>() {
                new()
            };

            dataRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<HeatPumpInstallation, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(installData);

            SetupGetAllCreditBalancesBadRequest();

            var handler = new DownloadRequestDataFileQueryHandler(_logger.Object, repository.Object, dataRepository.Object, _mockCreditLedgerService.Object);

            var expectedResult = Responses.BadRequest($"Failed to get Installation Credits for the period: [{DateOnly.FromDateTime(installRequest.StartDate)}..{DateOnly.FromDateTime(installRequest.EndDate)}]");

            //Act
            var result = await handler.Handle(new DownloadRequestDataFileQuery(installRequest.Id), CancellationToken.None);

            //Assert
            result.Result.Should().BeEquivalentTo(expectedResult);

        }
    }
}
