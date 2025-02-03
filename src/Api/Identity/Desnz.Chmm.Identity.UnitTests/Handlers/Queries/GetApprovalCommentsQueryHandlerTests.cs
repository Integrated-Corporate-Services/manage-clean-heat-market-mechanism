using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries
{
    public class GetApprovalCommentsQueryHandlerTests
    {
        private readonly Mock<ILogger<BaseRequestHandler<GetApprovalCommentsQuery, ActionResult<OrganisationApprovalCommentsDto>>>> _mockLogger;
        private readonly Mock<IOrganisationDecisionCommentsRepository> _mockOrganisationCommentsRepository;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<IOrganisationDecisionFilesRepository> _mockOrganisationApprovalFilesRepository;
        private readonly Mock<ICurrentUserService> _mockUserService;

        private Guid _existingOrganisationId = Guid.NewGuid();
        private readonly string _expectedComment = "My Comment";
        private readonly List<OrganisationDecisionFile> _approvalFiles;

        private readonly GetApprovalCommentsQueryHandler _handler;

        public GetApprovalCommentsQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<BaseRequestHandler<GetApprovalCommentsQuery, ActionResult<OrganisationApprovalCommentsDto>>>>();

            _mockOrganisationCommentsRepository = new Mock<IOrganisationDecisionCommentsRepository>(MockBehavior.Strict);
            var comment = new OrganisationDecisionComment(_expectedComment, Guid.NewGuid(), _existingOrganisationId, OrganisationFileConstants.Decisions.Approve);
            _mockOrganisationCommentsRepository.Setup(x => x.GetByOrganisationIdAndDecision(_existingOrganisationId, OrganisationFileConstants.Decisions.Approve, It.IsAny<bool>())).ReturnsAsync(comment);

            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisationsRepository.Setup(o => o.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(() => null);
            _mockOrganisationsRepository.Setup(o => o.GetById(_existingOrganisationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Mock<Organisation>().Object);

            _mockOrganisationApprovalFilesRepository = new Mock<IOrganisationDecisionFilesRepository>(MockBehavior.Strict);
            _approvalFiles = new List<OrganisationDecisionFile>
            {
                new OrganisationDecisionFile("Name.txt", $"{_existingOrganisationId}/Name.txt", _existingOrganisationId, OrganisationFileConstants.Decisions.Approve)
            };
            _mockOrganisationApprovalFilesRepository.Setup(x => x.GetByOrganisationIdAndDecision(_existingOrganisationId, OrganisationFileConstants.Decisions.Approve, It.IsAny<bool>())).ReturnsAsync(_approvalFiles);

            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _handler = new GetApprovalCommentsQueryHandler(
                _mockLogger.Object,
                _mockOrganisationCommentsRepository.Object,
                _mockOrganisationApprovalFilesRepository.Object,
                _mockOrganisationsRepository.Object,
                _mockUserService.Object);
        }


        [Fact]
        public async void When_UserNotLoggedIn_ReturnBadRequest()
        {
            _mockUserService.SetupGet(x => x.CurrentUser).Returns((ClaimsPrincipal?)null);
            var expectedResult = Responses.Unauthorized($"You are not authenticated");

            var result = await _handler.Handle(new GetApprovalCommentsQuery(_existingOrganisationId), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_OrganisationDoesNotExist_ReturnBadRequest()
        {
            var organisationId = Guid.NewGuid();
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {organisationId}");

            var result = await _handler.Handle(new GetApprovalCommentsQuery(organisationId), CancellationToken.None);
            var actionResult = result.Result;

            actionResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void When_DetailsOK_ReturnComment()
        {
            var result = await _handler.Handle(new GetApprovalCommentsQuery(_existingOrganisationId), CancellationToken.None);

            Assert.Equal(_expectedComment, result.Value.Comments);
            Assert.Equal(_approvalFiles.Count(), result.Value.FileNames.Count());
            Assert.Equal(_approvalFiles.First().FileName, result.Value.FileNames.First());
        }
    }
}
