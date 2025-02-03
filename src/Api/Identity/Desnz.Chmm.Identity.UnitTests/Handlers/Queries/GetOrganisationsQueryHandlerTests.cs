using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using AutoMapper;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Queries.GetOrganisationsQueryHandlerTestsFixture;
using Desnz.Chmm.Identity.Api.Entities;
using FluentAssertions;
using Desnz.Chmm.Common.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetOrganisationsQueryHandlerTests
{
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;

    private readonly GetOrganisationsQueryHandler _handler;

    public GetOrganisationsQueryHandlerTests()
    {
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);

        _handler = new GetOrganisationsQueryHandler(
            new Mock<ILogger<BaseRequestHandler<GetOrganisationsQuery, ActionResult<List<ViewOrganisationDto>>>>>().Object,
            _mockOrganisationsRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnOrganisations()
    {
        // Arrange
        var organisation = GetMockOrganisation();
        var organisations = new List<Organisation>() { organisation };

        var query = new GetOrganisationsQuery();

        _mockOrganisationsRepository.Setup(r => r.GetAll(null, true, false)).ReturnsAsync(organisations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
    }
}
