using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using AutoMapper;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Dtos;
using System.Net;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.CommonValidation;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Constants;
using JWT;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Queries.Annual;

public class GetAnnualBoilerSalesQueryHandlerTests
{
    private readonly DateTimeOverrideProvider _datetimeProvider;
    private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly GetAnnualBoilerSalesQueryHandler _handler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<ISchemeYearService> _schemeYearService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;

    public GetAnnualBoilerSalesQueryHandlerTests()
    {
        _datetimeProvider = new DateTimeOverrideProvider();
        _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mockMapper = new Mock<IMapper>();
        _mockUserService = new Mock<ICurrentUserService>();
        _schemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            StartDate = SchemeYearConstants.StartDate,
            EndDate = SchemeYearConstants.EndDate,
            BoilerSalesSubmissionEndDate = SchemeYearConstants.BoilerSalesSubmissionEndDate
        }, null);
        _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _schemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());

        _handler = new GetAnnualBoilerSalesQueryHandler(
            new Mock<ILogger<BaseRequestHandler<GetAnnualBoilerSalesQuery, ActionResult<AnnualBoilerSalesDto>>>>().Object,
            _mockMapper.Object,
            _mockAnnualBoilerSalesRepository.Object, 
            _schemeYearService.Object,
            _datetimeProvider, 
            validator);
    }

    [Fact]
    internal async Task Handle_OkAsync()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        var command = new GetAnnualBoilerSalesQuery(
            Guid.NewGuid(),
            SchemeYearConstants.Id
        );

        var entity = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, new List<AnnualBoilerSalesFile>(), "Me");
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(u => u.Id == command.OrganisationId && u.SchemeYearId == command.SchemeYearId, true, true, false)).Returns(Task.FromResult<AnnualBoilerSales?>(entity));
        _mockMapper.Setup(x => x.Map<AnnualBoilerSalesDto>(entity)).Returns(new AnnualBoilerSalesDto());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<AnnualBoilerSalesDto>>(actionResult);
        Assert.NotNull(actionResult.Value);
    }

    [Fact]
    internal async Task Handle_DueOnBoilerSalesSubmissionEndDate_OkAsync()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _datetimeProvider.OverrideDate(SchemeYearConstants.BoilerSalesSubmissionEndDate);

        var command = new GetAnnualBoilerSalesQuery(
            Guid.NewGuid(),
            SchemeYearConstants.Id
        );

        var entity = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, new List<AnnualBoilerSalesFile>(), "Me");
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(u => u.Id == command.OrganisationId && u.SchemeYearId == command.SchemeYearId, true, true, false)).Returns(Task.FromResult<AnnualBoilerSales?>(entity));
        _mockMapper.Setup(x => x.Map<AnnualBoilerSalesDto>(entity)).Returns(new AnnualBoilerSalesDto());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<AnnualBoilerSalesDto>>(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(BoilerSalesStatus.Due, actionResult.Value.Status);
    }

    [Fact]
    internal async Task Handle_NotDuePostBoilerSalesSubmissionEndDate_OkAsync()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _datetimeProvider.OverrideDate(SchemeYearConstants.BoilerSalesSubmissionEndDate.AddDays(1));

        var command = new GetAnnualBoilerSalesQuery(
            Guid.NewGuid(),
            SchemeYearConstants.Id
        );

        var entity = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, new List<AnnualBoilerSalesFile>(), "Me");
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(u => u.Id == command.OrganisationId && u.SchemeYearId == command.SchemeYearId, true, true, false)).Returns(Task.FromResult<AnnualBoilerSales?>(entity));
        _mockMapper.Setup(x => x.Map<AnnualBoilerSalesDto>(entity)).Returns(new AnnualBoilerSalesDto());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<AnnualBoilerSalesDto>>(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(BoilerSalesStatus.Default, actionResult.Value.Status);
    }

    [Fact]
    internal async Task Handle_NotFound_OkAsync()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        var command = new GetAnnualBoilerSalesQuery(
            Guid.NewGuid(),
            SchemeYearConstants.Id
        );

        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(u => u.Id == command.OrganisationId && u.SchemeYearId == command.SchemeYearId, true, true, false)).Returns(Task.FromResult<AnnualBoilerSales?>(null));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<AnnualBoilerSalesDto>>(actionResult);
        Assert.NotNull(actionResult.Value);
    }
}
