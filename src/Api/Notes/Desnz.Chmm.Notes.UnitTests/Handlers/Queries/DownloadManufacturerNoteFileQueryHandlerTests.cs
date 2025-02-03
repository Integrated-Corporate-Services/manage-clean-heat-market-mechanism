using Amazon.S3.Model;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Handlers.Queries;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Notes.Common.Queries;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using System.Security.Claims;
using System.Text;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Common.Services.FileService;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Identity.Common.Constants;

namespace Desnz.Chmm.Notes.UnitTests.Handlers.Queries;

public class DownloadManufacturerNoteFileQueryHandlerTests
{
    private readonly DownloadManufacturerNoteFileQueryHandler _handler;
    private readonly Mock<ILogger<DownloadManufacturerNoteFileQueryHandler>> logger;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<IManufacturerNotesRepository> _mockManufacturerNotesRepository;

    private readonly Guid _userId = Guid.NewGuid();

    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private readonly Guid _noteId = Guid.NewGuid();
    private readonly string _fileName = "TEST.pdf";

    private readonly CancellationToken cancellationToken = new CancellationToken();

    public DownloadManufacturerNoteFileQueryHandlerTests()
    {
        logger = new Mock<ILogger<DownloadManufacturerNoteFileQueryHandler>>();
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>();
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        _mockManufacturerNotesRepository = new Mock<IManufacturerNotesRepository>(MockBehavior.Strict);

        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId))
            .Returns(Task.FromResult(
                (HttpObjectResponse<OrganisationStatusDto>)
                new CustomHttpObjectResponse<OrganisationStatusDto>(new OrganisationStatusDto { Status = OrganisationConstants.Status.Active })));


        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        _mockManufacturerNotesRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<ManufacturerNote, bool>>>(), false))
            .Returns(Task.FromResult(new List<ManufacturerNote> { new ManufacturerNote(_organisationId, _schemeYearId, "details") }));

        _mockManufacturerNotesRepository.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(new ManufacturerNote(_organisationId, _schemeYearId, "details")));

        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);

        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var mockCurrentUser = GetMockCurrentUser(_userId.ToString(), _organisationId.ToString());
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new DownloadManufacturerNoteFileQueryHandler(
            logger.Object,
            _mockFileService.Object,
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

        var query = new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName);
        var response = await _handler.Handle(query, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response.Result);
    }


    [Fact]
    public async Task Handle_Invalid_SchemeYearId()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var query = new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName);
        var response = await _handler.Handle(query, cancellationToken);
        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Success_Existing_Note()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        var fileDownloadResponse = new FileDownloadResponse(It.IsAny<GetObjectResponse>(), Encoding.ASCII.GetBytes("File"), "text/plain", "fileKey", It.IsAny<string>());
        _mockFileService.Setup(x => x.DownloadFileAsync(Buckets.Note, $"{_noteId}/{_fileName}")).ReturnsAsync(() => fileDownloadResponse);
        //_mockFileService.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => fileDownloadResponse);

        var query = new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName);
        var response = await _handler.Handle(query, cancellationToken);
        Assert.IsType<FileContentResult>(response.Result);
    }

    [Fact]
    public async Task Handle_Success_NonExisting_Note()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        var fileDownloadResponse = new FileDownloadResponse(It.IsAny<GetObjectResponse>(), Encoding.ASCII.GetBytes("File"), "text/plain", "fileKey", It.IsAny<string>());
        _mockFileService.Setup(x => x.DownloadFileAsync(Buckets.Note, It.IsAny<string>()))
            .ReturnsAsync(() => fileDownloadResponse);

        _mockManufacturerNotesRepository.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .Returns(Task.FromResult((ManufacturerNote)null));

        var query = new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName);
        var response = await _handler.Handle(query, cancellationToken);
        Assert.IsType<NotFoundObjectResult>(response.Result);
    }


    [Fact]
    public async void When_File_DoesNotExist_ReturnBadRequest()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        var fileDownloadResponse = new FileDownloadResponse(It.IsAny<GetObjectResponse>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), "NotFound");
        _mockFileService
            .Setup(x => x.DownloadFileAsync(Buckets.Note, $"{_noteId}/{_fileName}"))
        .ReturnsAsync(() => fileDownloadResponse);
        var expectedResult = Responses.BadRequest(string.Format("Could not download a Manufacturer Note File with name: {0} for organisation with Id {1}", _fileName, _organisationId));

        var result = await _handler.Handle(new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async void When_File_CannotBeFetched_ReturnBadRequest()
    {
        TestAs(_userId, organisationId: _organisationId, role: Roles.PrincipalTechnicalOfficer);

        var getObjectResponse = new GetObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.Unauthorized };
        var errorMessage = string.Format("Failed to get document '{0}' from bucket '{1}'", "p1", "p2");
        var fileDownloadResponse = new FileDownloadResponse(getObjectResponse, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), errorMessage);
        _mockFileService
            .Setup(x => x.DownloadFileAsync(Buckets.Note, $"{_noteId}/{_fileName}"))
            .ReturnsAsync(() => fileDownloadResponse);
        var expectedResult = Responses.BadRequest("Failed to get document 'p1' from bucket 'p2'");

        var result = await _handler.Handle(new DownloadManufacturerNoteFileQuery(_organisationId, _schemeYearId, _noteId, _fileName), CancellationToken.None);
        var actionResult = result.Result;

        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    private ClaimsPrincipal GetMockCurrentUser(string userId, string organisationId)
    {
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "test@test"),
            new Claim(ClaimTypes.Role, Roles.PrincipalTechnicalOfficer),
            new Claim(JwtRegisteredClaimNames.Sid, userId),
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        return httpContext.User;
    }
}
