using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Api.AutoMapper;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Commands;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Api.Services;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Services;

public class InstallationRequestCommandHandlerUnitTests
{
    private readonly IServiceCollection _services;
    private readonly IMapper _mapper;
    private readonly InstallationRequestCommandHandler _installationRequestCommandHandler;
    private readonly Mock<ILogger<InstallationRequestCommandHandler>> _logger;
    private readonly Mock<IMcsInstallationDataRepository> _mcsDataRepository;
    private readonly Mock<IMcsMidService> _mcsService;
    private readonly Mock<ILicenceHolderService> _licenceHolderService;
    private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IOptions<ApiKeyPolicyConfig>> _mockOptionsMcsSynchronisationPolicyConfig;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly string _headerName;
    private readonly string _apiKey;

    public TestDatabaseFixture Fixture { get; }

    public InstallationRequestCommandHandlerUnitTests()
    {
        _headerName = "api-key";
        _apiKey = "1234";

        _logger = new Mock<ILogger<InstallationRequestCommandHandler>>();
        _mcsDataRepository = new Mock<IMcsInstallationDataRepository>();
        _mcsService = new Mock<IMcsMidService>();
        _mockIdentityService = new Mock<IIdentityService>();
        const string token = "TOKEN";
        var response = new HttpObjectResponse<string>(new HttpResponseMessage(HttpStatusCode.OK), token, null);


        _mockIdentityService.Setup(x => x.GetJwtToken(It.IsAny<GetJwtTokenCommand>())).Returns(Task.FromResult(response));

        _licenceHolderService = new Mock<ILicenceHolderService>();
        _mockCreditLedgerService = new Mock<ICreditLedgerService>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mockOptionsMcsSynchronisationPolicyConfig = new Mock<IOptions<ApiKeyPolicyConfig>>();
        _mockOptionsMcsSynchronisationPolicyConfig.Setup(x => x.Value).Returns(new ApiKeyPolicyConfig() { HeaderName = _headerName, ApiKey = _apiKey });
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(GetMockContext());
        _mcsDataRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _services = new ServiceCollection().AddLogging();
        _services.AddAutoMapper(typeof(McsSynchronisationAutoMapperProfile));
        IServiceProvider serviceProvider = _services.BuildServiceProvider();

        _mapper = serviceProvider.GetService<IMapper>();

        var datetimeProvider = new DateTimeProvider();
        _installationRequestCommandHandler = new InstallationRequestCommandHandler(_logger.Object, _mapper, _mcsDataRepository.Object, _licenceHolderService.Object, _mockCreditLedgerService.Object, _mockIdentityService.Object, _mockOptionsMcsSynchronisationPolicyConfig.Object, _mockHttpContextAccessor.Object, datetimeProvider, _mcsService.Object);
    }
    private HttpContext GetMockContext()
    {
        var mockHttpContext = new DefaultHttpContext();
        mockHttpContext.Request.Headers["HeaderName"] = _headerName;
        mockHttpContext.Request.Headers["api-key"] = _apiKey;
        return mockHttpContext;
    }

    [Fact]
    internal async Task Can_Handle_InstallationRequest_Test()
    {
        //Arange
        var productDtos = new List<McsProductDto> { new McsProductDto() };

        var singleInstallation = new McsInstallationDto { MidId = 1, HeatPumpProducts = productDtos };
        var installations = new McsInstallationsDto() { Installations = new List<McsInstallationDto> { singleInstallation } };
        var installationsResponse = new HttpObjectResponse<McsInstallationsDto>(new HttpResponseMessage(HttpStatusCode.OK), installations, null);
        _mcsService.Setup(x => x.GetHeatPumpInstallations(It.IsAny<GetMcsInstallationsDto>())).Returns(Task.FromResult(installationsResponse));

        var response = new HttpObjectResponse<List<CreatedResponse>>(new HttpResponseMessage(HttpStatusCode.Created), new List<CreatedResponse> { new CreatedResponse { Id = Guid.NewGuid() } }, null);
        _licenceHolderService.Setup(x => x.Create(It.IsAny<CreateLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        var response2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockCreditLedgerService.Setup(x => x.GenerateCredits(It.IsAny<GenerateCreditsCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response2));

        var command = new InstallationRequestCommand(new DateTime(2023, 3, 31), new DateTime(2023, 1, 1));

        //Act
        var result = await _installationRequestCommandHandler.Handle(command, new CancellationToken());

        //Assert
        Assert.IsType<OkResult>(result);
    }


    [Fact]
    internal async Task InstallationRequest_On_Create_LicenceHolders_Failure_Test()
    {
        //Arange
        var productDtos = new List<McsProductDto> { new McsProductDto() };

        var singleInstallation = new McsInstallationDto { MidId = 1, HeatPumpProducts = productDtos };
        var installations = new McsInstallationsDto() { Installations = new List<McsInstallationDto> { singleInstallation } };
        var installationsResponse = new HttpObjectResponse<McsInstallationsDto>(new HttpResponseMessage(HttpStatusCode.OK), installations, null);
        _mcsService.Setup(x => x.GetHeatPumpInstallations(It.IsAny<GetMcsInstallationsDto>())).Returns(Task.FromResult(installationsResponse));

        var response = new HttpObjectResponse<List<CreatedResponse>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _licenceHolderService.Setup(x => x.Create(It.IsAny<CreateLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        var command = new InstallationRequestCommand(new DateTime(2023, 3, 31), new DateTime(2023, 1, 1));

        //Act
        var result = await _installationRequestCommandHandler.Handle(command, new CancellationToken());

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    internal async Task InstallationRequest_On_Generate_Credits_Failure_Test()
    {
        //Arange
        var productDtos = new List<McsProductDto> { new McsProductDto() };

        var singleInstallation = new McsInstallationDto { MidId = 1, 
            AirTypeTechnologyId = 1,
            AlternativeHeatingAgeId = 1,
            AlternativeHeatingFuelId = 1,
            AlternativeHeatingSystemId = 1,
            CertificatesCount = 1,
            CommissioningDate = DateTime.UtcNow,
            IsAlternativeHeatingSystemPresent = true,
            IsHybrid = true,
            IsNewBuildId = 1,
            Mpan = "sth",
            RenewableSystemDesignId = 1,
            TechnologyTypeId = 1,
            TotalCapacity = 1,
            HeatPumpProducts = productDtos };
        var installations = new McsInstallationsDto() { Installations = new List<McsInstallationDto> { singleInstallation } };
        var installationsResponse = new HttpObjectResponse<McsInstallationsDto>(new HttpResponseMessage(HttpStatusCode.OK), installations, null);
        _mcsService.Setup(x => x.GetHeatPumpInstallations(It.IsAny<GetMcsInstallationsDto>())).Returns(Task.FromResult(installationsResponse));

        var response = new HttpObjectResponse<List<CreatedResponse>>(new HttpResponseMessage(HttpStatusCode.Created), new List<CreatedResponse> { new CreatedResponse { Id = Guid.NewGuid() } }, null);
        _licenceHolderService.Setup(x => x.Create(It.IsAny<CreateLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        var response2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockCreditLedgerService.Setup(x => x.GenerateCredits(It.IsAny<GenerateCreditsCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response2));

        var command = new InstallationRequestCommand(new DateTime(2023, 3, 31), new DateTime(2023, 1, 1));

        //Act
        var result = await _installationRequestCommandHandler.Handle(command, new CancellationToken());

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    internal async Task InstallationRequest_On_BadRequestObjectResult_Failure_Test()
    {
        //Arange
        var command = new InstallationRequestCommand(new DateTime(2023, 3, 31), new DateTime(2023, 1, 1));
        var datetimeProvider = new DateTimeProvider();
        var installationRequestCommandHandler = new InstallationRequestCommandHandler(_logger.Object, _mapper, _mcsDataRepository.Object, _licenceHolderService.Object, _mockCreditLedgerService.Object, _mockIdentityService.Object, _mockOptionsMcsSynchronisationPolicyConfig.Object, _mockHttpContextAccessor.Object, datetimeProvider, null);

        //Act
        var result = await installationRequestCommandHandler.Handle(command, new CancellationToken());

        var expectedResult = new ObjectResult("Error persisting the MSC data, exception: Object reference not set to an instance of an object.")
        {
            StatusCode = 500
        };

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
