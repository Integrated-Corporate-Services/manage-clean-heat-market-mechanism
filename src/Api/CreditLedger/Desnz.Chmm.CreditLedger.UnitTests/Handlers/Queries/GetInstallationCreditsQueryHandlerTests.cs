using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Handlers.Queries;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.Common.Extensions;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries;

public class GetInstallationCreditsQueryHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<BaseRequestHandler<GetInstallationCreditsQuery, ActionResult<List<HeatPumpInstallationCreditsDto>>>>> _mockLogger;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;

    private readonly GetInstallationCreditsQueryHandler _handler;

    public GetInstallationCreditsQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<BaseRequestHandler<GetInstallationCreditsQuery, ActionResult<List<HeatPumpInstallationCreditsDto>>>>>();
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);

        _handler = new GetInstallationCreditsQueryHandler(
            _mockLogger.Object,
            _mockCreditLedgerRepository.Object);
    }

    [Fact]
    public async Task ShouldReturnCorrectResults_When_Ok()
    {
        // Arrange
        var (date, _) = DateTime.Now;
        var licenceHolderId = Guid.NewGuid();
        _mockCreditLedgerRepository.Setup(x => x.Get(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync(
            new List<InstallationCredit>
            {
                new InstallationCredit(licenceHolderId, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(licenceHolderId, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(licenceHolderId, 1, SchemeYearConstants.Id, date, 100, true),
                new InstallationCredit(licenceHolderId, 1, SchemeYearConstants.Id, date, 100, true)
            });

        var query = new GetInstallationCreditsQuery(It.IsAny<DateOnly>(), It.IsAny<DateOnly>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.Single(result.Value);
        Assert.Equal(300, result.Value[0].Value);
    }
}
