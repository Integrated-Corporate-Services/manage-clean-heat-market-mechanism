using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Handlers.Queries;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Notes.Common.Dtos;
using Desnz.Chmm.Notes.Common.Queries;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Desnz.Chmm.Notes.UnitTests.Handlers.Queries;

public class GetManufacturerNotesQueryHandlerTests
{
    private readonly GetManufacturerNotesQueryHandler handler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<GetManufacturerNotesQueryHandler>> _mockLogger;
    private readonly Mock<IManufacturerNotesRepository> _mockManufacturerNotesRepository;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<ICurrentUserService> _mockUserService;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _schemeYearId = SchemeYearConstants.Id;

    private readonly CancellationToken cancellationToken = new CancellationToken();

    public GetManufacturerNotesQueryHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<GetManufacturerNotesQueryHandler>>();
        _mockManufacturerNotesRepository = new Mock<IManufacturerNotesRepository>();
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                (HttpObjectResponse<OrganisationStatusDto>)
                new CustomHttpObjectResponse<OrganisationStatusDto>(new OrganisationStatusDto { Status = OrganisationConstants.Status.Active })));

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        handler = new GetManufacturerNotesQueryHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockManufacturerNotesRepository.Object,
            validator);
    }

    private void TestAs(Guid? userId = null, Guid? organisationId = null, string role = Roles.Manufacturer)
    {
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, role)
        };
        if (userId.HasValue)
            claims.Add(new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()));
        if (organisationId.HasValue)
            claims.Add(new Claim(Claims.OrganisationId, organisationId.ToString()));
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(httpContext.User);
    }

    [Fact]
    public async Task Handle_Invalid_OrganisationId()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                HttpObjectResponseFactory.Problem<OrganisationStatusDto>(
                    new Chmm.Common.ValueObjects.ProblemDetails(StatusCodes.Status404NotFound, "Organisation not found"))));

        var query = new GetManufacturerNotesQuery(_organisationId, _schemeYearId);
        var response = await handler.Handle(query, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Success()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        _mockManufacturerNotesRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<ManufacturerNote, bool>>>(), false))
            .Returns(Task.FromResult(new List<ManufacturerNote> { }));
        _mockMapper.Setup(x => x.Map<List<ManufacturerNoteDto>>(It.IsAny<List<ManufacturerNote>>()))
            .Returns(new List<ManufacturerNoteDto> { });

        var query = new GetManufacturerNotesQuery(_organisationId, _schemeYearId);
        var response = await handler.Handle(query, cancellationToken);
        Assert.IsType<ActionResult<List<ManufacturerNoteDto>>>(response);
    }
}
