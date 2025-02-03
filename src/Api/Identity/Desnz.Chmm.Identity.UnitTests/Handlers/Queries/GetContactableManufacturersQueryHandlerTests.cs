using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
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
using System.Linq.Expressions;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Constants;
using System.Net;
using Desnz.Chmm.Testing.Common;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetContactableManufacturersQueryHandlerTests
{
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;

    private readonly GetContactableManufacturersQueryHandler _handler;

    public GetContactableManufacturersQueryHandlerTests()
    {
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);

        _handler = new GetContactableManufacturersQueryHandler(
            new Mock<ILogger<BaseRequestHandler<GetContactableManufacturersQuery, ActionResult<List<OrganisationContactDetailsDto>>>>>().Object,
            _mockOrganisationsRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnOrganisations()
    {
        // Arrange
        var organisation = GetMockOrganisation();
        var myOrganisation = GetMockOrganisation();
        myOrganisation.Activate();

        var organisations = new List<Organisation>() { organisation };

        var query = new GetContactableManufacturersQuery(Guid.NewGuid());

        _mockOrganisationsRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, false)).ReturnsAsync(myOrganisation);

        _mockOrganisationsRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<Organisation, bool>>>(), false, false)).ReturnsAsync(organisations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.First().ContactName.Should().Be(organisation.ContactName);
        result.Value.First().ContactEmail.Should().Be(organisation.ContactEmail);
        result.Value.First().ContactTelephone.Should().Be(organisation.ContactTelephoneNumber);
        result.Value.First().OrganisationName.Should().Be(organisation.Name);
    }
}
