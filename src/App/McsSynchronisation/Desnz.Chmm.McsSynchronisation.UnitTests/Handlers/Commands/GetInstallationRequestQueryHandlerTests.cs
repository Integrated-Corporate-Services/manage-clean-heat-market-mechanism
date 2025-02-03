using Desnz.Chmm.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Desnz.Chmm.ApiClients.Http;
using System.Net;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using AutoMapper;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using System.Linq.Expressions;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Common.Extensions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Handlers.Commands
{
    public class GetInstallationRequestQueryHandlerTests
    {
        private readonly Mock<IHeatPumpInstallationsRepository> _mockHeatPumpInstallationsRepository;
        private readonly Mock<ILogger<GetInstallationRequestQueryHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetInstallationRequestQueryHandler _handler;

        public GetInstallationRequestQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetInstallationRequestQueryHandler>>();
            _mockHeatPumpInstallationsRepository = new Mock<IHeatPumpInstallationsRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _mockHeatPumpInstallationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _handler = new GetInstallationRequestQueryHandler(_mockLogger.Object, _mockMapper.Object, _mockHeatPumpInstallationsRepository.Object);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_Ok_On_Success()
        {
            // Arrange
            var manufacturerId = 1;
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var installationRequestId = Guid.NewGuid();

            var installation = new HeatPumpInstallation
            {
                InstallationRequestId = installationRequestId,
                CommissioningDate = startDate,
                HeatPumpProducts = new List<HeatPumpProduct> { new HeatPumpProduct { Id = 1, ManufacturerId = manufacturerId } }
            };

            var installationDto = new CreditCalculationDto
            {
                CommissioningDate = startDate,
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1, ManufacturerId = manufacturerId } }
            };


            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() { McsManufacturerId = manufacturerId} }, null);

            var command = new GetInstallationRequestQuery(installationRequestId, 1);

            var expectedResult = new List<CreditCalculationDto> { installationDto };

            var installations = new List<HeatPumpInstallation> { installation };
            _mockHeatPumpInstallationsRepository.Setup(x => x.GetAll(It.Is<Expression<Func<HeatPumpInstallation, bool>>>(y => y.Compile()(installation)), false)).Returns(Task.FromResult(installations));

            _mockMapper.Setup(x => x.Map<List<CreditCalculationDto>>(installations)).Returns(new List<CreditCalculationDto> { installationDto});

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Value.Result.Should().BeEquivalentTo(expectedResult);
            result.Result.Should().BeNull();
        }


        [Fact]
        internal async Task InstallationRequest_On_InternalServerError_Failure_Test()
        {
            //Arange
            var requestId = Guid.NewGuid();
            var command = new GetInstallationRequestQuery(requestId, 1);
            var handler = new GetInstallationRequestQueryHandler(_mockLogger.Object, _mockMapper.Object, null);

            //Act
            var result = await handler.Handle(command, new CancellationToken());

            var expectedResult = new ObjectResult($"Error getting Installations for Request Id: {requestId}, exception: Object reference not set to an instance of an object.")
            {
                StatusCode = 500
            };

            //Assert
            result.Result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
