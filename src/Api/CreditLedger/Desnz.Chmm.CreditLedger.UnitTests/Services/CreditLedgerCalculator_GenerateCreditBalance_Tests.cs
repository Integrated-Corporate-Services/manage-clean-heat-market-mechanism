using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Constants;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Testing.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Services;

public class CreditLedgerCalculator_GenerateCreditBalance_Tests
{
    private readonly Mock<ILogger<CreditLedgerCalculator>> _mockLogger;
    private readonly ICreditLedgerCalculator _calculator;

    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _schemeYearId = SchemeYearConstants.Id;

    public CreditLedgerCalculator_GenerateCreditBalance_Tests()
    {        
        _mockLogger = new Mock<ILogger<CreditLedgerCalculator>>();
        _calculator = new CreditLedgerCalculator(
            _mockLogger.Object);
    }

    [Fact]
    public async void GenerateCreditBalance_Generates_Correct_Balance()
    {
        var licenceHolderId = Guid.NewGuid();

        var creditBalance = 1;

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 2, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now)
        };

        var balance = _calculator.GenerateCreditBalance(_organisationId, creditBalance, transactions);

        Assert.Equal(3, balance);
    }

    [Fact]
    public async void GenerateCreditBalances_Generates_Correct_Balances()
    {
        var licenceHolderId = Guid.NewGuid();
        var organisation2Id = Guid.NewGuid();

        var allCredits = new List<OrganisationLicenceHolderCreditsDto>
        {
            new OrganisationLicenceHolderCreditsDto(licenceHolderId, _organisationId, 1)
        };

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 2, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now)
        };

        var organisatons = new List<ViewOrganisationDto>
        {
            new ViewOrganisationDto{ Id = _organisationId },
            new ViewOrganisationDto{ Id = organisation2Id }
        };
        var balance = _calculator.GenerateCreditBalances(organisatons.Select(i => i.Id).ToList(), allCredits, transactions);

        Assert.Equal(3, balance.Single(i => i.OrganisationId == _organisationId).CreditBalance);
        Assert.Equal(0, balance.Single(i => i.OrganisationId == organisation2Id).CreditBalance);
    }
}