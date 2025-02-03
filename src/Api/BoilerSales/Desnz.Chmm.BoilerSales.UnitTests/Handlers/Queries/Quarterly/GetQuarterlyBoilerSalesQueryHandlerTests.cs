using Xunit;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Moq;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using AutoMapper;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Desnz.Chmm.ApiClients.Services;
using System.Linq.Expressions;
using Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Dtos;
using System.Net;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.CommonValidation;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using JWT;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Queries.Quarterly;

public class GetQuarterlyBoilerSalesQueryHandlerTests : TestClaimsBase
{
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly GetQuarterlyBoilerSalesQueryHandler _handler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<ISchemeYearService> _schemeYearService;
    private Mock<IOrganisationService> _mockOrganisationService;

    public GetQuarterlyBoilerSalesQueryHandlerTests()
    {
        _dateTimeProvider = new DateTimeOverrideProvider();
        _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mockMapper = new Mock<IMapper>();
        _mockUserService = new Mock<ICurrentUserService>();
        _schemeYearService = new Mock<ISchemeYearService>();
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Quarters = new List<SchemeYearQuarterDto>
            {
                new SchemeYearQuarterDto{ Id = SchemeYearConstants.QuarterOneId, EndDate = SchemeYearConstants.QuarterOneEndDate },
                new SchemeYearQuarterDto{ Id = SchemeYearConstants.QuarterTwoId, EndDate = SchemeYearConstants.QuarterTwoEndDate },
                new SchemeYearQuarterDto{ Id = SchemeYearConstants.QuarterThreeId, EndDate = SchemeYearConstants.QuarterThreeEndDate },
                new SchemeYearQuarterDto{ Id = SchemeYearConstants.QuarterFourId, EndDate = SchemeYearConstants.QuarterFourEndDate }
            },
            BoilerSalesSubmissionEndDate = SchemeYearConstants.BoilerSalesSubmissionEndDate
        }, null);
        _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(new GovukNotifyConfig());
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig());
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            new Mock<ISchemeYearService>(MockBehavior.Strict).Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new GetQuarterlyBoilerSalesQueryHandler(
            new Mock<ILogger<BaseRequestHandler<GetQuarterlyBoilerSalesQuery, ActionResult<List<QuarterlyBoilerSalesDto>>>>>().Object,
            _mockMapper.Object, 
            _mockQuarterlyBoilerSalesRepository.Object,
            _schemeYearService.Object, 
            _dateTimeProvider,
            validator);
    }

    [Fact]
    internal async Task GetQuarterlyBoilerSales_OkAsync()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        var command = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        var entity = new List<QuarterlyBoilerSales> { new QuarterlyBoilerSales(Guid.NewGuid(), SchemeYearConstants.Id, SchemeYearConstants.QuarterOneId, 1, 1, new List<QuarterlyBoilerSalesFile>(), "Me") };
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), true, true, false)).Returns(Task.FromResult(entity));
        _mockMapper.Setup(x => x.Map<List<QuarterlyBoilerSalesDto>>(entity)).Returns(new List<QuarterlyBoilerSalesDto>());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<List<QuarterlyBoilerSalesDto>>>(actionResult);
        Assert.NotNull(actionResult.Value);
    }

    [Fact]
    internal async Task GetQuarterlyBoilerSales_Quarter1NotDueOnEndDate()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate);

        var command = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        var entity = new List<QuarterlyBoilerSales> { new QuarterlyBoilerSales(Guid.NewGuid(), SchemeYearConstants.Id, SchemeYearConstants.QuarterOneId, 1, 1, new List<QuarterlyBoilerSalesFile>(), "Me") };
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), true, true, false)).Returns(Task.FromResult(entity));
        _mockMapper.Setup(x => x.Map<List<QuarterlyBoilerSalesDto>>(entity)).Returns(new List<QuarterlyBoilerSalesDto>());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotEqual(BoilerSalesStatus.Due, actionResult.Value.First(i => i.SchemeYearQuarterId == SchemeYearConstants.QuarterOneId).Status);
    }

    [Fact]
    internal async Task GetQuarterlyBoilerSales_Quarter1DueOneDayAfterEndDate()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate.AddDays(1));

        var command = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        var entity = new List<QuarterlyBoilerSales> { new QuarterlyBoilerSales(Guid.NewGuid(), SchemeYearConstants.Id, SchemeYearConstants.QuarterOneId, 1, 1, new List<QuarterlyBoilerSalesFile>(), "Me") };
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), true, true, false)).Returns(Task.FromResult(entity));
        _mockMapper.Setup(x => x.Map<List<QuarterlyBoilerSalesDto>>(entity)).Returns(new List<QuarterlyBoilerSalesDto>());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(BoilerSalesStatus.Due, actionResult.Value.First(i => i.SchemeYearQuarterId == SchemeYearConstants.QuarterOneId).Status);
    }

    [Fact]
    internal async Task GetQuarterlyBoilerSales_Quarter1DueBoilerSalesSubmissionEndDate()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.BoilerSalesSubmissionEndDate.AddDays(-1));

        var command = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        var entity = new List<QuarterlyBoilerSales> { new QuarterlyBoilerSales(Guid.NewGuid(), SchemeYearConstants.Id, SchemeYearConstants.QuarterOneId, 1, 1, new List<QuarterlyBoilerSalesFile>(), "Me") };
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), true, true, false)).Returns(Task.FromResult(entity));
        _mockMapper.Setup(x => x.Map<List<QuarterlyBoilerSalesDto>>(entity)).Returns(new List<QuarterlyBoilerSalesDto>());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(BoilerSalesStatus.Due, actionResult.Value.First(i => i.SchemeYearQuarterId == SchemeYearConstants.QuarterOneId).Status);
    }

    [Fact]
    internal async Task GetQuarterlyBoilerSales_Quarter1NotDueOnBoilerSalesSubmissionEndDate()
    {
        //Arrange
        var identity = new GenericIdentity("some name", "test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Principal Technical Officer"));
        var contextUser = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext() { User = contextUser };
        _mockUserService.Setup(x => x.CurrentUser).Returns(context.User);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.BoilerSalesSubmissionEndDate);

        var command = new GetQuarterlyBoilerSalesQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        var entity = new List<QuarterlyBoilerSales> { new QuarterlyBoilerSales(Guid.NewGuid(), SchemeYearConstants.Id, SchemeYearConstants.QuarterOneId, 1, 1, new List<QuarterlyBoilerSalesFile>(), "Me") };
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), true, true, false)).Returns(Task.FromResult(entity));
        _mockMapper.Setup(x => x.Map<List<QuarterlyBoilerSalesDto>>(entity)).Returns(new List<QuarterlyBoilerSalesDto>());

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotEqual(BoilerSalesStatus.Due, actionResult.Value.First(i => i.SchemeYearQuarterId == SchemeYearConstants.QuarterOneId).Status);
    }
}
