using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.ApiClients.Http;
using System.Net;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using AutoMapper;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using System.Linq.Expressions;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Newtonsoft.Json;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Handlers.Commands
{
    public class GetManufacturerInstallationsQueryHandlerTests
    {
        private readonly Mock<IHeatPumpInstallationsRepository> _mockHeatPumpInstallationsRepository;
        private readonly Mock<ILogger<GetManufacturerInstallationsQueryHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetManufacturerInstallationsQueryHandler _handler;

        public GetManufacturerInstallationsQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetManufacturerInstallationsQueryHandler>>();
            _mockHeatPumpInstallationsRepository = new Mock<IHeatPumpInstallationsRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockLicenceHolderService = new Mock<ILicenceHolderService>();
            _mockMapper = new Mock<IMapper>();

            _mockHeatPumpInstallationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _handler = new GetManufacturerInstallationsQueryHandler(_mockLogger.Object, _mockMapper.Object, _mockHeatPumpInstallationsRepository.Object, _mockLicenceHolderService.Object);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_Ok_On_Success()
        {
            // Arrange
            var manufacturerId = 1;
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var installation = new HeatPumpInstallation
            {
                CommissioningDate = startDate,
                HeatPumpProducts = new List<HeatPumpProduct> { new HeatPumpProduct { Id = 1, ManufacturerId = manufacturerId } }
            };

            var installationDto = new CreditCalculationDto
            {
                CommissioningDate = startDate,
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1, ManufacturerId = manufacturerId } }
            };


            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() { McsManufacturerId = manufacturerId} }, null);

            var command = new GetManufacturerInstallationsQuery(manufacturerId, startDate, endDate);

            var expectedResult = new List<CreditCalculationDto> { installationDto };

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var installations = new List<HeatPumpInstallation> { installation };
            _mockHeatPumpInstallationsRepository.Setup(x => x.GetAll(It.Is<Expression<Func<HeatPumpInstallation, bool>>>(y => y.Compile()(installation)), false)).Returns(Task.FromResult(installations));

            _mockMapper.Setup(x => x.Map<List<CreditCalculationDto>>(installations)).Returns(new List<CreditCalculationDto> { installationDto});

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
            result.Result.Should().BeNull();
        }

        [Fact(Skip = "TODO")]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_LicenceHolders()
        {
            // Arrange
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>(), null);

            var command = new GetManufacturerInstallationsQuery(1, It.IsAny<DateTime>(), It.IsAny<DateTime>());

            var expectedResult = Responses.BadRequest($"Invalid Manufacturer Id: 1");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_LicenceHolders_3()
        {
            // Arrange
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), new List<LicenceHolderDto>(), new ProblemDetails(400, "Failed to get Licence Holders, problem: 1"));

            var command = new GetManufacturerInstallationsQuery(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());

            var expectedResult = Responses.BadRequest($"Failed to get all Licence Holders, problem: {JsonConvert.SerializeObject(httpResponseLicenceHolders.Problem)}");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_LicenceHolders_2()
        {
            // Arrange
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

            var command = new GetManufacturerInstallationsQuery(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());

            var expectedResult = Responses.BadRequest($"Failed to get all Licence Holders, problem: { JsonConvert.SerializeObject(httpResponseLicenceHolders.Problem)}");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
