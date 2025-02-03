using Amazon.S3.Model;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;
using static Desnz.Chmm.Common.Services.FileService;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationFileConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries
{
    public class DownloadApprovalCommentsFileQueryHandlerTests
    {
        private readonly Mock<ILogger<BaseRequestHandler<DownloadApprovalCommentsFileQuery, ActionResult<Stream>>>> _mockLogger;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<ICurrentUserService> _mockUserService;
        private readonly Mock<IFileService> _mockFileService;

        private readonly DownloadApprovalCommentsFileQueryHandler _handler;

        private Guid _existingOrganisationId = Guid.NewGuid();
        private string _fileName = "file.txt";

        public DownloadApprovalCommentsFileQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<BaseRequestHandler<DownloadApprovalCommentsFileQuery, ActionResult<Stream>>>>();

            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisationsRepository
                .Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);
            _mockOrganisationsRepository
                .Setup(o => o.GetById(_existingOrganisationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new Mock<Organisation>().Object);

            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _mockFileService = new Mock<IFileService>(MockBehavior.Strict);

            _handler = new DownloadApprovalCommentsFileQueryHandler(
                _mockLogger.Object, 
                _mockOrganisationsRepository.Object,
                _mockUserService.Object, 
                _mockFileService.Object);
        }


        [Fact]
        public async void When_UserNotLoggedIn_ReturnBadRequest()
        {
            _mockUserService.SetupGet(x => x.CurrentUser).Returns((ClaimsPrincipal?)null);
            var expectedResult = Responses.Unauthorized($"You are not authenticated");

            var result = await _handler.Handle(new DownloadApprovalCommentsFileQuery(_existingOrganisationId, _fileName), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        public async void When_OrganisationDoesNotExist_ReturnBadRequest()
        {
            var orgId = Guid.NewGuid();
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {orgId}");

            var result = await _handler.Handle(new DownloadApprovalCommentsFileQuery(orgId, _fileName), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DetailsOK_ReturnComment()
        {

            var fileDownloadResponse = new FileDownloadResponse(It.IsAny<GetObjectResponse>(), Encoding.ASCII.GetBytes("File"), "text/plain", "fileKey", It.IsAny<string>());
            _mockFileService
                .Setup(x => x.DownloadFileAsync(Buckets.IdentityOrganisationApprovals, $"{_existingOrganisationId}/{_fileName}"))
                .ReturnsAsync(() => fileDownloadResponse);

            var result = await _handler.Handle(new DownloadApprovalCommentsFileQuery(_existingOrganisationId, _fileName), CancellationToken.None);

            result.Result.Should().NotBeNull();
        }

        [Fact]
        public async void When_File_DoesNotExist_ReturnBadRequest()
        {
            var fileDownloadResponse = new FileDownloadResponse(It.IsAny<GetObjectResponse>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), "NotFound");
            _mockFileService
                .Setup(x => x.DownloadFileAsync(Buckets.IdentityOrganisationApprovals, $"{_existingOrganisationId}/{_fileName}"))
                .ReturnsAsync(() => fileDownloadResponse);

            var expectedResult = Responses.BadRequest(string.Format("Could not download an Approval Comments File with name: {0} for organisation with Id {1}", _fileName, _existingOrganisationId));

            var result = await _handler.Handle(new DownloadApprovalCommentsFileQuery(_existingOrganisationId, _fileName), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        public async void When_File_CannotBeFetched_ReturnBadRequest()
        {
            var getObjectResponse = new GetObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.Unauthorized};
            var errorMessage = string.Format("Failed to get document '{0}' from bucket '{1}'", "p1", "p2");
            var fileDownloadResponse = new FileDownloadResponse(getObjectResponse, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), errorMessage);
            _mockFileService
                .Setup(x => x.DownloadFileAsync(Buckets.IdentityOrganisationApprovals, $"{_existingOrganisationId}/{_fileName}"))
                .ReturnsAsync(() => fileDownloadResponse);

            var expectedResult = Responses.BadRequest("Failed to get document 'p1' from bucket 'p2'");

            var result = await _handler.Handle(new DownloadApprovalCommentsFileQuery(_existingOrganisationId, _fileName), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
