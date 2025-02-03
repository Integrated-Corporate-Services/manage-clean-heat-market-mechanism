using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.McsSynchronisation.Api.AutoMapper;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Commands;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Api.Services;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Desnz.Chmm.McsSynchronisation.UnitTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Reflection.Metadata;
using Xunit;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Services;
#region Class under test
public class SynchroniseInstallationsCommandHandlerDerived : SynchroniseInstallationsCommandHandler
{

    public SynchroniseInstallationsCommandHandlerDerived(ILogger<SynchroniseInstallationsCommandHandler> logger,
        IMapper mapper,
        IMcsInstallationDataRepository mcsDataRepository,
        ILicenceHolderService licenceHolderService,
        ICreditLedgerService creditLedgerService,
        IIdentityService identityService,
        IOptions<ApiKeyPolicyConfig> apiKeyPolicyConfig,
        IHttpContextAccessor httpContextAccessor,
        IDateTimeProvider datetimeProvider,
        IMcsMidService mcsMidService) :
        base(logger,
        mapper,
        mcsDataRepository,
        licenceHolderService,
        creditLedgerService,
        identityService,
        apiKeyPolicyConfig,
        httpContextAccessor,
        datetimeProvider,
        mcsMidService)
    {
    }

    public new (DateTime, DateTime) GetPreviousWeekPeriod(DateTime today)
    {
        return base.GetPreviousWeekPeriod(today);
    }
}
#endregion Class under test

public class SynchroniseInstallationsCommandHandlerUnitTests
{
    private readonly IServiceCollection _services;
    private readonly IMapper _mapper;
    private readonly SynchroniseInstallationsCommandHandlerDerived _synchroniseInstallationsCommandHandlerDerived;
    private readonly Mock<ILogger<SynchroniseInstallationsCommandHandler>> _logger;
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

    public SynchroniseInstallationsCommandHandlerUnitTests()
    {
        _headerName = "api-key";
        _apiKey = "1234";

        _logger = new Mock<ILogger<SynchroniseInstallationsCommandHandler>>();
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
        _synchroniseInstallationsCommandHandlerDerived = new SynchroniseInstallationsCommandHandlerDerived(_logger.Object, _mapper, _mcsDataRepository.Object, _licenceHolderService.Object, _mockCreditLedgerService.Object, _mockIdentityService.Object, _mockOptionsMcsSynchronisationPolicyConfig.Object, _mockHttpContextAccessor.Object, datetimeProvider, _mcsService.Object);
    }
    private HttpContext GetMockContext()
    {
        var mockHttpContext = new DefaultHttpContext();
        mockHttpContext.Request.Headers["HeaderName"] = _headerName;
        mockHttpContext.Request.Headers["api-key"] = _apiKey;
        return mockHttpContext;
    }

    [Theory]
    [InlineData("2024-04-07", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-08", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-09", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-10", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-11", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-12", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-13", "2024-03-31 00:00:00.000", "2024-04-06 23:59:59.999")]
    [InlineData("2024-04-14", "2024-04-07 00:00:00.000", "2024-04-13 23:59:59.999")]
    internal async Task Can_GetPreviousWeekPeriod(string todayString, string expectedStartDateString, string expectedEndDateString)
    {
        //Arrange
        var today = DateTime.Parse(todayString);
        var expectedStartDate = DateTime.Parse(expectedStartDateString);
        var expectedEndDate = DateTime.Parse(expectedEndDateString);

        //Act
        var (actualStartDate, actualEndDate) = _synchroniseInstallationsCommandHandlerDerived.GetPreviousWeekPeriod(today);

        //Assert
        Assert.Equal(expectedStartDate, actualStartDate);
        Assert.Equal(expectedEndDate, actualEndDate);
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
        _mcsDataRepository.Setup(x => x.GetRequest(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult<InstallationRequest?>(null));

        var response = new HttpObjectResponse<List<CreatedResponse>>(new HttpResponseMessage(HttpStatusCode.Created), new List<CreatedResponse> { new CreatedResponse { Id = Guid.NewGuid() } }, null);
        _licenceHolderService.Setup(x => x.Create(It.IsAny<CreateLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        var response2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockCreditLedgerService.Setup(x => x.GenerateCredits(It.IsAny<GenerateCreditsCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response2));

        var command = new SynchroniseInstallationsCommand();

        //Act
        var result = await _synchroniseInstallationsCommandHandlerDerived.Handle(command, new CancellationToken());

        //Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    internal async Task Fails_If_Duplicate()
    {
        //Arange

        _mcsDataRepository.Setup(x => x.GetRequest(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(new InstallationRequest { }));

        var command = new SynchroniseInstallationsCommand();

        //Act
        var result = await _synchroniseInstallationsCommandHandlerDerived.Handle(command, new CancellationToken());

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

}
