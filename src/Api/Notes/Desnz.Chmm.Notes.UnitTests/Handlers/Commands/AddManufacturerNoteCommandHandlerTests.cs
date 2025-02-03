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

public class AddManufacturerNoteCommandHandlerTests
{
    private readonly AddManufacturerNoteCommandHandler handler;
    private readonly Mock<ILogger<BaseRequestHandler<AddManufacturerNoteCommand, ActionResult<Guid>>>> logger;
    private readonly Mock<IHttpContextAccessor> httpContextAccessor;
    private readonly Mock<IManufacturerNotesRepository> manufacturerNotesRepository;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> fileService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private readonly Guid _noteId = Guid.NewGuid();
    private readonly string fileName = "TEST.pdf";

    private readonly CancellationToken cancellationToken = new CancellationToken();
    private Mock<ICurrentUserService> _mockUserService;

    public AddManufacturerNoteCommandHandlerTests()
    {
        logger = new Mock<ILogger<BaseRequestHandler<AddManufacturerNoteCommand, ActionResult<Guid>>>>();
        httpContextAccessor = new Mock<IHttpContextAccessor>();
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

        handler = new AddManufacturerNoteCommandHandler(
            logger.Object,
            _mockUserService.Object,
            manufacturerNotesRepository.Object,
            _mockOrganisationService.Object,
            fileService.Object,
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
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                HttpObjectResponseFactory.Problem<OrganisationStatusDto>(
                    new Chmm.Common.ValueObjects.ProblemDetails(StatusCodes.Status404NotFound, "Organisation not found"))));

        var response = await handler.Handle(new AddManufacturerNoteCommand
        {
            Details = "details",
            OrganisationId = _organisationId,
            SchemeYearId = _schemeYearId
        }, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Success_Without_Files()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        manufacturerNotesRepository.Setup(x => x.Create(It.IsAny<ManufacturerNote>(), true))
            .Returns(Task.FromResult(_noteId));
        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { }));

        var response = await handler.Handle(new AddManufacturerNoteCommand
        {
            Details = "details",
            OrganisationId = _organisationId,
            SchemeYearId = _schemeYearId
        }, cancellationToken);
        Assert.IsType<CreatedResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Success_With_Files()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        manufacturerNotesRepository.Setup(x => x.Create(It.IsAny<ManufacturerNote>(), true))
            .Returns(Task.FromResult(_noteId));
        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { fileName }));
        fileService.Setup(x => x.CopyFileAsync(Buckets.Note, It.IsAny<string>(), Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileCopyResponse(null, fileName)));
        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName)));

        var response = await handler.Handle(new AddManufacturerNoteCommand
        {
            Details = "details",
            OrganisationId = _organisationId,
            SchemeYearId = _schemeYearId
        }, cancellationToken);
        Assert.IsType<CreatedResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Returns_500_When_Copying_S3_files_fails()
    {
        //Arrange
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        manufacturerNotesRepository.Setup(x => x.Create(It.IsAny<ManufacturerNote>(), true))
            .Returns(Task.FromResult(_noteId));
        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { fileName }));
        fileService.Setup(x => x.CopyFileAsync(Buckets.Note, It.IsAny<string>(), Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileCopyResponse(null, fileName, "error")));
        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName)));

        //Act
        var response = await handler.Handle(new AddManufacturerNoteCommand
        {
            Details = "details",
            OrganisationId = _organisationId,
            SchemeYearId = _schemeYearId
        }, cancellationToken);

        //Assert
        Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(500, ((ObjectResult)response.Result).StatusCode);
        Assert.Contains("Error copying S3 files", ((ObjectResult)response.Result).Value.ToString());
    }

    [Fact]
    public async Task Handle_Returns_500_When_Deleting_S3_files_fails()
    {
        //Arrange
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        manufacturerNotesRepository.Setup(x => x.Create(It.IsAny<ManufacturerNote>(), true))
            .Returns(Task.FromResult(_noteId));
        fileService.Setup(x => x.GetFileFullPathsAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new List<string> { fileName }));
        fileService.Setup(x => x.CopyFileAsync(Buckets.Note, It.IsAny<string>(), Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileCopyResponse(null, fileName)));
        fileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.Note, It.IsAny<string>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, fileName, "error")));

        //Act
        var response = await handler.Handle(new AddManufacturerNoteCommand
        {
            Details = "details",
            OrganisationId = _organisationId,
            SchemeYearId = _schemeYearId
        }, cancellationToken);

        //Assert
        Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(500, ((ObjectResult)response.Result).StatusCode);
        Assert.Contains("Error deleting S3 files", ((ObjectResult)response.Result).Value.ToString());
    }
}
