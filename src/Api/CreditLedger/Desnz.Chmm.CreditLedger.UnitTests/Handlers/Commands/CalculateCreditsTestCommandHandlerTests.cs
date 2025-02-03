using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.CreditLedger.Api.Handlers.Commands;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Newtonsoft.Json;
using AutoMapper;
using Desnz.Chmm.CreditLedger.Api.AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Dtos;
using System.Net;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Api.Entities;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Commands
{
    public class CalculateCreditsTestCommandHandlerTests : TestClaimsBase
    {
        private readonly Mock<IInstallationCreditRepository> _mockLicenceHolderRepository;
        private readonly Mock<ILogger<CalculateCreditsTestCommandHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CreditLedgerCalculator _mockInstallationCreditCalculator;
        private readonly CalculateCreditsTestCommandHandler _handler;
        private readonly IMapper _mapper;

        private List<CreateLicenceHolderCommand> _licenceHolders;
        private readonly Mock<ISchemeYearService> _schemeYearService;

        public CalculateCreditsTestCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CalculateCreditsTestCommandHandler>>();
            _mockLicenceHolderRepository = new Mock<IInstallationCreditRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockInstallationCreditCalculator = new CreditLedgerCalculator(new Mock<ILogger<CreditLedgerCalculator>>().Object);

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreditLedgerAutoMapperProfile>();
            });
            _mapper = configuration.CreateMapper();

            _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _schemeYearService = new Mock<ISchemeYearService>();
            var zeroValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0M };
            var halfValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0.5M };
            var wholeValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 1M };

            var creditWeightings = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), new CreditWeightingsDto()
            {
                Id = Guid.NewGuid(),
                TotalCapacity = 70,
                SchemeYearId = SchemeYearConstants.Id,
                AlternativeSystemFuelTypeWeightings = new List<AlternativeSystemFuelTypeWeightingDto>
                {
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Anthracite",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BiogasLandfillCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BulkWoodPellets",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Coal",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "DualFuelMineralAndWood",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Electricity",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "MainsGas",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Oil",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WasteCombustionCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodChips",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodLogs",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "NotApplicableNoOtherHeatingSource",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BioliquidHvoBioLpg",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Solar",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Other",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "LPG",
                        AlternativeSystemFuelTypeWeightingValue =  halfValue
                    }
                },
                HeatPumpTechnologyTypeWeightings = new List<HeatPumpTechnologyTypeWeightingDto>
                {
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "AirSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "ExhaustAirHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GasAbsorbtionHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GroundWaterSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "HotWaterHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "SolarAssistedHeatPump",
                        Value =  1
                    }
                }
            }, null);
            _schemeYearService.Setup(x => x.GetCreditWeightings(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(creditWeightings);

            _handler = new CalculateCreditsTestCommandHandler(_mockLogger.Object, _mapper, _mockInstallationCreditCalculator, _schemeYearService.Object);

            _licenceHolders = new List<CreateLicenceHolderCommand> { new CreateLicenceHolderCommand { McsManufacturerId = 1, McsManufacturerName = "Test" } };
        }

        [Fact]
        internal async Task CalculateCredits_With_Invalid_SchemeYearId_Returns_Bad_Request()
        {
            //Arrange
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            _schemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var installations = GetTestInstallations();
            var command = new CalculateCreditsTestCommand(installations);

            var expectedResult = Responses.BadRequest("Failed to get current Scheme Year, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Result.Should().BeEquivalentTo(expectedResult);

        }

        [Fact]
        internal async Task Can_Calculate_Credits()
        {
            //Arrange
            var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _schemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
            _schemeYearService.Setup(x => x.GetCreditWeightings(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponse);
            
            var zeroValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0M };
            var halfValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0.5M };
            var wholeValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 1M };

            var creditWeightings = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), new CreditWeightingsDto()
            {
                Id = Guid.NewGuid(),
                TotalCapacity = 70,
                SchemeYearId = SchemeYearConstants.Id,
                AlternativeSystemFuelTypeWeightings = new List<AlternativeSystemFuelTypeWeightingDto>
                {
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Anthracite",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BiogasLandfillCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BulkWoodPellets",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue 
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Coal",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "DualFuelMineralAndWood",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Electricity",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "MainsGas",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Oil",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WasteCombustionCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodChips",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodLogs",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "NotApplicableNoOtherHeatingSource",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BioliquidHvoBioLpg",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Solar",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Other",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "LPG",
                        AlternativeSystemFuelTypeWeightingValue =  halfValue
                    }
                },
                HeatPumpTechnologyTypeWeightings = new List<HeatPumpTechnologyTypeWeightingDto>
                {
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "AirSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "ExhaustAirHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GasAbsorbtionHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GroundWaterSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "HotWaterHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "SolarAssistedHeatPump",
                        Value =  1
                    }
                }
            }, null);
            _schemeYearService.Setup(x => x.GetCreditWeightings(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(creditWeightings);

            var installations = GetTestInstallations();
            var command = new CalculateCreditsTestCommand(installations);

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            var list = actionResult.Value;
            foreach (var item in list)
            {
                Assert.NotNull(item.Credit);
            }

        }


        private IEnumerable<HeatPumpInstallationDto> GetTestInstallations()
        {
            return new List<HeatPumpInstallationDto>() {
                new HeatPumpInstallationDto(){
                    AirTypeTechnologyId = 1,
                    AlternativeHeatingAgeId = 1,
                    AlternativeHeatingFuelId = 1,
                    AlternativeHeatingSystemId = 1,
                    CertificatesCount = 1,
                    CommissioningDate = DateTime.UtcNow,
                    HeatPumpProducts = new List<McsProductDto>()
                    {
                        new McsProductDto()
                        {
                            Code = "Code",
                            Id = 1,
                            ManufacturerId = 1,
                            ManufacturerName = "Manufacturer Name",
                            Name = "Name",
                        }
                    },
                    MidId = 2,
                    IsAlternativeHeatingSystemPresent = true,
                    IsHybrid = true,
                    IsNewBuildId = 1,
                    Mpan = "Mpan",
                    RenewableSystemDesignId = 1,
                    TechnologyTypeId = 1,
                    TotalCapacity = 1
                },
                new HeatPumpInstallationDto(){
                    AirTypeTechnologyId = 2,
                    AlternativeHeatingAgeId = 2,
                    AlternativeHeatingFuelId = 2,
                    AlternativeHeatingSystemId = 2,
                    CertificatesCount = 2,
                    CommissioningDate = DateTime.UtcNow,
                    HeatPumpProducts = new List<McsProductDto>()
                    {
                        new McsProductDto()
                        {
                            Code = "Code",
                            Id = 2,
                            ManufacturerId = 2,
                            ManufacturerName = "Manufacturer Name",
                            Name = "Name",
                        }
                    },
                    MidId = 2,
                    IsAlternativeHeatingSystemPresent = true,
                    IsHybrid = true,
                    IsNewBuildId = 2,
                    Mpan = "Mpan",
                    RenewableSystemDesignId = 2,
                    TechnologyTypeId = 6,
                    TotalCapacity = 2
                }
            };
        }

        [Fact(Skip = "Used to prepare json string")]
        internal async Task GenerateJsonString()
        {
            //Arrange
            var installations = GetTestInstallations();

            var serialised = JsonConvert.SerializeObject(installations);


        }
    }
}
