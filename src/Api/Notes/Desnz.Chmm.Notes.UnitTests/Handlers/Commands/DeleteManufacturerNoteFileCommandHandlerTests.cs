using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Notes.Api.Handlers.Commands;
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

public class DeleteManufacturerNoteFileCommandHandlerTests
{
    private readonly DeleteManufacturerNoteFileCommandHandler handler;
    private readonly Mock<ILogger<DeleteManufacturerNoteFileCommandHandler>> logger;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> fileService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private readonly string fileName = "TEST.pdf";

    private readonly CancellationToken cancellationToken = new CancellationToken();
    private Mock<ICurrentUserService> _mockUserService;

    public DeleteManufacturerNoteFileCommandHandlerTests()
    {
        logger = new Mock<ILogger<DeleteManufacturerNoteFileCommandHandler>>();
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
        TestAs(_userId, _organisationId);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                (HttpObjectResponse<OrganisationStatusDto>)
                new CustomHttpObjectResponse<OrganisationStatusDto>(new OrganisationStatusDto { Status = OrganisationConstants.Status.Active })));

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        handler = new DeleteManufacturerNoteFileCommandHandler(
            logger.Object,
            _mockUserService.Object,
            fileService.Object,
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

        var command = new DeleteManufacturerNoteFileCommand(_organisationId, _schemeYearId, fileName);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Handle_Invalid_Delete_Response()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName, "Error")));

        var command = new DeleteManufacturerNoteFileCommand(_organisationId, _schemeYearId, fileName);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Handle_Success()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName)));

        var command = new DeleteManufacturerNoteFileCommand(_organisationId, _schemeYearId, fileName);
        var response = await handler.Handle(command, cancellationToken);
        Assert.IsType<NoContentResult>(response);
    }
}
