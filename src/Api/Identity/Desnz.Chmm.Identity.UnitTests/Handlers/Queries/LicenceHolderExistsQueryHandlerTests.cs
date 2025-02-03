using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Queries;
using Desnz.Chmm.Identity.Api.Entities;
using FluentAssertions;
using Desnz.Chmm.Common.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class LicenceHolderExistsQueryHandlerTests
{
    private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;

    private readonly LicenceHolderExistsQueryHandler _handler;

    public LicenceHolderExistsQueryHandlerTests()
    {
        _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>(MockBehavior.Strict);

        _handler = new LicenceHolderExistsQueryHandler(
            new Mock<ILogger<BaseRequestHandler<LicenceHolderExistsQuery, ActionResult<LicenceHolderExistsDto>>>>().Object,
            _mockLicenceHolderRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnOrganisations()
    {
        // Arrange
        var licenceHolder = new LicenceHolder(1, "Name");

        var licenceHolderId = licenceHolder.Id;   
        var query = new LicenceHolderExistsQuery(licenceHolderId);

        _mockLicenceHolderRepository.Setup(r => r.Get(It.Is<Expression<Func<LicenceHolder, bool>>>(y => y.Compile()(licenceHolder)), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(licenceHolder);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
    }
}
