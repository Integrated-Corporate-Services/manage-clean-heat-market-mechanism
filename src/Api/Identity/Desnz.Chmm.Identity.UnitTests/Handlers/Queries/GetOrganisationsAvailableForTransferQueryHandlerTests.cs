using AutoMapper;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Moq;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Queries.GetOrganisationsQueryHandlerTestsFixture;
using Xunit;
using Desnz.Chmm.Identity.Api.Entities;
using FluentAssertions;
using System.Linq.Expressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Desnz.Chmm.Common.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries
{
    public class GetOrganisationsAvailableForTransferQueryHandlerTests
    {

        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        private readonly GetOrganisationsAvailableForTransferQueryHandler _handler;

        public GetOrganisationsAvailableForTransferQueryHandlerTests()
        {
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);

            _handler = new GetOrganisationsAvailableForTransferQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetOrganisationsAvailableForTransferQuery, ActionResult<List<ViewOrganisationDto>>>>>().Object,
                _mockMapper.Object, 
                _mockOrganisationsRepository.Object, 
                _mockCurrentUserService.Object);
        }

        [Fact]
        public async Task ShouldReturnOrganisations()
        {
            var orgId = Guid.NewGuid();

            // Arrange
            var organisation = GetMockOrganisation();
            var organisations = new List<Organisation>() { organisation };

            var query = new GetOrganisationsAvailableForTransferQuery(orgId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);
            _mockOrganisationsRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<Organisation, bool>>?>(), false, false)).ReturnsAsync(organisations);
            _mockMapper.Setup(x => x.Map<List<ViewOrganisationDto>>(It.IsAny<List<Organisation>>())).Returns(new List<ViewOrganisationDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Value.Should().NotBeNull();
        }
    }
}
