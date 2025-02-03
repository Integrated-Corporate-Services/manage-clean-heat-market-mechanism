using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.CreditLedger.Api.Handlers.Commands;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.ApiClients.Http;
using System.Net;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Newtonsoft.Json;
using AutoMapper;
using Desnz.Chmm.CreditLedger.Api.AutoMapper;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Api.Entities;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Commands
{
    public class GenerateCreditsCommandHandlerDerived : GenerateCreditsCommandHandler
    {
        public GenerateCreditsCommandHandlerDerived(ILogger<BaseRequestHandler<GenerateCreditsCommand, ActionResult>> logger, IMapper mapper, IInstallationCreditRepository installationCreditRepository, ILicenceHolderService licenceHolderService, ICreditLedgerCalculator installationCreditCalculator, ISchemeYearService schemeYearService) : base(logger, mapper, installationCreditRepository, licenceHolderService, installationCreditCalculator, schemeYearService)
        {
        }

        public IList<InstallationCredit> GetCreditsByManufacturer(CalculatedInstallationCreditDto installation, Dictionary<int, Guid> licenceHolders)
        { 
            return base.GetCreditsByManufacturer(installation, licenceHolders);
        }
    }

    public class GenerateCreditsCommandHandlerTests : TestClaimsBase
    {
        private readonly Mock<IInstallationCreditRepository> _mockLicenceHolderRepository;
        private readonly Mock<ILogger<GenerateCreditsCommandHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
        private readonly Mock<ICreditLedgerCalculator> _mockInstallationCreditCalculator;
        private readonly GenerateCreditsCommandHandler _handler;
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly HttpObjectResponse<List<SchemeYearDto>> _schemYearData;
        private readonly GenerateCreditsCommand _command;

        public GenerateCreditsCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GenerateCreditsCommandHandler>>();
            _mockLicenceHolderRepository = new Mock<IInstallationCreditRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockLicenceHolderService = new Mock<ILicenceHolderService>();
            _mockInstallationCreditCalculator = new Mock<ICreditLedgerCalculator>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreditLedgerAutoMapperProfile>();
            });
            _mapper = configuration.CreateMapper();

            _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _schemeYearService = new Mock<ISchemeYearService>();
            _schemYearData = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<SchemeYearDto>()
            {
                new SchemeYearDto()
                {
                    Id = SchemeYearConstants.Id,
                    Year = SchemeYearConstants.Year,
                    CreditGenerationWindowStartDate = SchemeYearConstants.CreditGenerationWindowStartDate,
                    CreditGenerationWindowEndDate = SchemeYearConstants.CreditGenerationWindowEndDate,
                },
                    new SchemeYearDto()
                {
                    Id = SchemeYearConstants.Year2025Id,
                    Year = SchemeYearConstants.Year + 1,
                    CreditGenerationWindowStartDate = SchemeYearConstants.CreditGenerationWindowStartDate.AddYears(1),
                    CreditGenerationWindowEndDate = SchemeYearConstants.CreditGenerationWindowEndDate.AddYears(1),
                }
            }, null);
            _schemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(_schemYearData);
            var creditWeightings = new HttpObjectResponse<List<CreditWeightingsDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<CreditWeightingsDto>(){
                GenerateCreditWeightings(SchemeYearConstants.Id),
                GenerateCreditWeightings(SchemeYearConstants.Year2025Id)
                
            }, null);
            _schemeYearService.Setup(x => x.GetAllCreditWeightings(It.IsAny<CancellationToken>())).ReturnsAsync(creditWeightings);

            _handler = new GenerateCreditsCommandHandler(
                _mockLogger.Object,
                _mapper,
                _mockLicenceHolderRepository.Object,
                _mockLicenceHolderService.Object,
                _mockInstallationCreditCalculator.Object,
                _schemeYearService.Object);

            _command = new GenerateCreditsCommand(new List<McsInstallationDto>());
        }

        private CreditWeightingsDto GenerateCreditWeightings(Guid id)
        {
            var zeroValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0M };
            var halfValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0.5M };
            var wholeValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 1M };

            return new CreditWeightingsDto()
            {
                Id = Guid.NewGuid(),
                TotalCapacity = 70,
                SchemeYearId = id,
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
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Electricity",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "MainsGas",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Oil",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
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
                        AlternativeSystemFuelTypeWeightingValue = halfValue
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
                        AlternativeSystemFuelTypeWeightingValue = halfValue
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
            };
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_Ok_When_NoInstallations_Exist()
        {
            // Arrange
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);
            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var expectedResult = Responses.Ok();

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_Ok_On_Success()
        {
            // Arrange
            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } },
                CommissioningDate = commissioningDate
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);

            var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

            

            var expectedResult = Responses.Ok();

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var credit = 0.5m;
            _mockInstallationCreditCalculator.Setup(x => x.Calculate(installation, weightings.ToWeightingDictionary())).Returns(credit);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_LicenceHolders()
        {
            // Arrange
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } }
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), new List<LicenceHolderDto>(), new Chmm.Common.ValueObjects.ProblemDetails(400, "Failed to get Product Licence Holders, problem: 1"));

            var expectedResult = Responses.BadRequest($"Failed to get all Licence Holders, problem: {JsonConvert.SerializeObject(httpResponseLicenceHolders.Problem)}");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_LicenceHolders_2()
        {
            // Arrange
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } }
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

            

            var expectedResult = Responses.BadRequest($"Failed to get all Licence Holders, problem: null");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_GetAllSchemeYears()
        {
            // Arrange
            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } },
                CommissioningDate = commissioningDate
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);

            var httpResponseSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _schemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseSchemeYears);

            var expectedResult = Responses.BadRequest($"Failed to get all Scheme Years, problem: null");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var creditWeightings = new HttpObjectResponse<List<CreditWeightingsDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<CreditWeightingsDto>(){
                GenerateCreditWeightings(SchemeYearConstants.Id),
                GenerateCreditWeightings(SchemeYearConstants.Year2025Id)

            }, null);
            _schemeYearService.Setup(x => x.GetAllCreditWeightings(It.IsAny<CancellationToken>())).ReturnsAsync(creditWeightings);



            var handler = new GenerateCreditsCommandHandler(
                                                                      _mockLogger.Object,
                                                                      _mapper,
                                                                      _mockLicenceHolderRepository.Object,
                                                                      _mockLicenceHolderService.Object,
                                                                      _mockInstallationCreditCalculator.Object,
                                                                      _schemeYearService.Object);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_GetAllSchemeYears_2()
        {
            // Arrange
            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } },
                CommissioningDate = commissioningDate
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);
            var schemeYearService = new Mock<ISchemeYearService>();

            var httpResponseSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), new List<SchemeYearDto>(), new Chmm.Common.ValueObjects.ProblemDetails(400, "Failed to get scheme years, problem: 1"));
            schemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseSchemeYears);


            var creditWeightings = new HttpObjectResponse<List<CreditWeightingsDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<CreditWeightingsDto>(){
                GenerateCreditWeightings(SchemeYearConstants.Id),
                GenerateCreditWeightings(SchemeYearConstants.Year2025Id)

            }, null);
            schemeYearService.Setup(x => x.GetAllCreditWeightings(It.IsAny<CancellationToken>())).ReturnsAsync(creditWeightings);

            var expectedResult = Responses.BadRequest($"Failed to get Scheme Years, problem: {JsonConvert.SerializeObject(httpResponseSchemeYears.Problem)}");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var handler = new GenerateCreditsCommandHandler(
                                                                      _mockLogger.Object,
                                                                      _mapper,
                                                                      _mockLicenceHolderRepository.Object,
                                                                      _mockLicenceHolderService.Object,
                                                                      _mockInstallationCreditCalculator.Object,
                                                                      schemeYearService.Object);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            string message = ((ObjectResult)result).Value.ToString();
            Assert.StartsWith("Failed to get all Scheme Years, problem: ", message);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_GetAllCreditWeightings()
        {
            // Arrange
            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } },
                CommissioningDate = commissioningDate
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);
            var schemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

            var httpResponse = new HttpObjectResponse<List<CreditWeightingsDto>>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            schemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(_schemYearData);
            schemeYearService.Setup(x => x.GetAllCreditWeightings(It.IsAny<CancellationToken>())).ReturnsAsync(httpResponse);

            

            var expectedResult = Responses.BadRequest($"Failed to get Credit Weightings, problem: null");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var handler = new GenerateCreditsCommandHandler(
                                                                      _mockLogger.Object,
                                                                      _mapper,
                                                                      _mockLicenceHolderRepository.Object,
                                                                      _mockLicenceHolderService.Object,
                                                                      _mockInstallationCreditCalculator.Object,
                                                                      schemeYearService.Object);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GenerateCredits_ShouldReturn_BadRequest_When_Failing_HttpRequest_GetAllCreditWeightings_2()
        {
            // Arrange
            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CreditCalculationDto
            {
                HeatPumpProducts = new List<McsProductDto> { new McsProductDto { Id = 1 } },
                CommissioningDate = commissioningDate
            };

            var httpResponseInstallations = new HttpObjectResponse<DataPage<CreditCalculationDto>>(new HttpResponseMessage(HttpStatusCode.OK), (new List<CreditCalculationDto>() { installation }).ToPage(1, 1), null);
            var httpResponseLicenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>() { new LicenceHolderDto() }, null);
            var schemeYearService = new Mock<ISchemeYearService>();

            var httpResponse = new HttpObjectResponse<List<CreditWeightingsDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(400, "Failed to get Credit Weightings, problem: 1"));
            schemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(_schemYearData);
            schemeYearService.Setup(x => x.GetAllCreditWeightings(It.IsAny<CancellationToken>())).ReturnsAsync(httpResponse);

            var expectedResult = Responses.BadRequest($"Failed to get Credit Weightings, problem: {JsonConvert.SerializeObject(httpResponse.Problem)}");

            _mockLicenceHolderService.Setup(x => x.GetAll(null)).ReturnsAsync(httpResponseLicenceHolders);

            var handler = new GenerateCreditsCommandHandler(
                                                                      _mockLogger.Object,
                                                                      _mapper,
                                                                      _mockLicenceHolderRepository.Object,
                                                                      _mockLicenceHolderService.Object,
                                                                      _mockInstallationCreditCalculator.Object,
                                                                      schemeYearService.Object);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCreditsByManufacturer_OneManufacturer_OneProduct()
        {
            var product1 = new McsProductDto { Id = 20021, ManufacturerId = 700, Code = "PRD1" };

            var handler = new GenerateCreditsCommandHandlerDerived(_mockLogger.Object,
                _mapper,
                _mockLicenceHolderRepository.Object,
                _mockLicenceHolderService.Object,
                _mockInstallationCreditCalculator.Object,
                _schemeYearService.Object);

            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CalculatedInstallationCreditDto
            {
                MidId = 1,
                SchemeYearId = Guid.NewGuid(),
                CommissioningDate = commissioningDate,
                Credit = 1.0m,
                IsHybrid = false,
                HeatPumpProducts = new List<McsProductDto> { product1 },
            };

            var result = handler.GetCreditsByManufacturer(installation, new Dictionary<int, Guid> { { 700, Guid.NewGuid() } });

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result[0].Value);
        }


        [Fact]
        public void GetCreditsByManufacturer_OneManufacturer_TwoProducts()
        {
            var product1 = new McsProductDto { Id = 20021, ManufacturerId = 700, Code = "PRD1" };
            var product2 = new McsProductDto { Id = 20022, ManufacturerId = 700, Code = "PRD2" };

            var handler = new GenerateCreditsCommandHandlerDerived(_mockLogger.Object,
                _mapper,
                _mockLicenceHolderRepository.Object,
                _mockLicenceHolderService.Object,
                _mockInstallationCreditCalculator.Object,
                _schemeYearService.Object);

            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CalculatedInstallationCreditDto
            {
                MidId = 1,
                SchemeYearId = Guid.NewGuid(),
                CommissioningDate = commissioningDate,
                Credit = 1.0m,
                IsHybrid = false,
                HeatPumpProducts = new List<McsProductDto> { product1, product2 },
            };

            var result = handler.GetCreditsByManufacturer(installation, new Dictionary<int, Guid> { { 700, Guid.NewGuid() } });

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result[0].Value);
        }


        [Fact]
        public void GetCreditsByManufacturer_TwoManufacturer_OneProduct_Each()
        {
            var product1 = new McsProductDto { Id = 20021, ManufacturerId = 700, Code = "PRD1" };
            var product2 = new McsProductDto { Id = 20022, ManufacturerId = 701, Code = "PRD2" };

            var handler = new GenerateCreditsCommandHandlerDerived(_mockLogger.Object,
                _mapper,
                _mockLicenceHolderRepository.Object,
                _mockLicenceHolderService.Object,
                _mockInstallationCreditCalculator.Object,
                _schemeYearService.Object);

            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CalculatedInstallationCreditDto
            {
                MidId = 1,
                SchemeYearId = Guid.NewGuid(),
                CommissioningDate = commissioningDate,
                Credit = 1.0m,
                IsHybrid = false,
                HeatPumpProducts = new List<McsProductDto> { product1, product2 },
            };

            var result = handler.GetCreditsByManufacturer(installation, new Dictionary<int, Guid> { { 700, Guid.NewGuid() }, { 701, Guid.NewGuid() } });

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Value);
            Assert.Equal(1, result[1].Value);
        }

        [Fact]
        public void GetCreditsByManufacturer_TwoManufacturer_VariableProducts()
        {
            var product1 = new McsProductDto { Id = 20021, ManufacturerId = 700, Code = "PRD1" };
            var product2 = new McsProductDto { Id = 20022, ManufacturerId = 701, Code = "PRD2" };
            var product3 = new McsProductDto { Id = 20023, ManufacturerId = 701, Code = "PRD3" };

            var handler = new GenerateCreditsCommandHandlerDerived(_mockLogger.Object,
                _mapper,
                _mockLicenceHolderRepository.Object,
                _mockLicenceHolderService.Object,
                _mockInstallationCreditCalculator.Object,
                _schemeYearService.Object);

            var commissioningDate = SchemeYearConstants.CreditGenerationWindowStartDate.ToDateTime(new TimeOnly(1, 1, 1));
            var installation = new CalculatedInstallationCreditDto
            {
                MidId = 1,
                SchemeYearId = Guid.NewGuid(),
                CommissioningDate = commissioningDate,
                Credit = 1.0m,
                IsHybrid = false,
                HeatPumpProducts = new List<McsProductDto> { product1, product2, product3 },
            };

            var result = handler.GetCreditsByManufacturer(installation, new Dictionary<int, Guid> { { 700, Guid.NewGuid() }, { 701, Guid.NewGuid() } });

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Value);
            Assert.Equal(2, result[1].Value);
        }
    }
}
