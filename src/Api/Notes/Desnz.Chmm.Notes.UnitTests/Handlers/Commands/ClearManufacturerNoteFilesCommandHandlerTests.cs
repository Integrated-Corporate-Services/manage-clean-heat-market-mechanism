using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Handlers.Commands;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Notes.Common.Commands;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;

namespace Desnz.Chmm.Notes.UnitTests.Handlers.Commands;

public class ClearManufacturerNoteFilesCommandHandlerTests
{
    private readonly ClearManufacturerNoteFilesCommandHandler handler;
    private readonly Mock<ILogger<BaseRequestHandler<ClearManufacturerNoteFilesCommand, ActionResult>>> logger;
    private readonly Mock<IManufacturerNotesRepository> manufacturerNotesRepository;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> fileService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;

    private readonly Guid userId = Guid.NewGuid();

    private readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private readonly Guid noteId = Guid.NewGuid();
    private readonly string fileName = "TEST.pdf";

    private readonly CancellationToken cancellationToken = new CancellationToken();
    private Mock<ICurrentUserService> _mockUserService;

    public ClearManufacturerNoteFilesCommandHandlerTests()
    {
        logger = new Mock<ILogger<BaseRequestHandler<ClearManufacturerNoteFilesCommand, ActionResult>>>();
        manufacturerNotesRepository = new Mock<IManufacturerNotesRepository>();
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        fileService = new Mock<IFileService>();
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

        handler = new ClearManufacturerNoteFilesCommandHandler( 
            logger.Object,
            fileService.Object,
            _mockUserService.Object,
            validator);
    }

    private void TestAs(Guid? userId = null, Guid? organisationId = null, string role = null)
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
        TestAs(userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                HttpObjectResponseFactory.Problem<OrganisationStatusDto>(
                    new Chmm.Common.ValueObjects.ProblemDetails(StatusCodes.Status404NotFound, "Organisation not found"))));

        var command = new ClearManufacturerNoteFilesCommand(_organisationId, _schemeYearId);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Handle_Invalid_SchemeYearId()
    {
        TestAs(userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                HttpObjectResponseFactory.Problem<OrganisationStatusDto>(
                    new Chmm.Common.ValueObjects.ProblemDetails(StatusCodes.Status404NotFound, "Cannot retrieve the current scheme year: Null scheme year returned"))));

        var command = new ClearManufacturerNoteFilesCommand(_organisationId, Guid.NewGuid());
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Handle_Success_Without_Files()
    {
        TestAs(userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { }));

        var command = new ClearManufacturerNoteFilesCommand(_organisationId, _schemeYearId);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task Handle_Success_With_Files()
    {
        TestAs(userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { fileName }));
        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName)));

        var command = new ClearManufacturerNoteFilesCommand(_organisationId, _schemeYearId);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task Handle_Error_With_Files()
    {
        TestAs(userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { fileName }));
        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName, "error")));

        var command = new ClearManufacturerNoteFilesCommand(_organisationId, _schemeYearId);

        var response = await handler.Handle(command, cancellationToken);

        //Assert
        Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, ((ObjectResult)response).StatusCode);
        Assert.Contains("Error deleting S3 files", ((ObjectResult)response).Value.ToString());
    }
}
