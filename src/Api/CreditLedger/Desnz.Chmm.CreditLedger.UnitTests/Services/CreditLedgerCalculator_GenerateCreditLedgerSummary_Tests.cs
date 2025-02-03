using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.CreditLedger.Constants;
using Desnz.Chmm.Testing.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Services
{
    public class CreditLedgerCalculator_GenerateCreditLedgerSummary_Tests
    {
        private static readonly Guid _existingOrganisationId = Guid.NewGuid();
        private static readonly Guid _licenceHolderId = Guid.NewGuid();
        private CreditLedgerCalculator _installationCreditCalculator;

        public CreditLedgerCalculator_GenerateCreditLedgerSummary_Tests()
        {
            var mockLogger = new Mock<ILogger<CreditLedgerCalculator>>();
            _installationCreditCalculator = new CreditLedgerCalculator(
                mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnCorrectResults_When_Ok()
        {
            var (date, _) = DateTime.Now;
            // Arrange
            var installationCredits = 
                new List<InstallationCredit>
                {
                new InstallationCredit(_licenceHolderId, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(_licenceHolderId, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(_licenceHolderId, 1, SchemeYearConstants.Id, date, 100, true),
                new InstallationCredit(_licenceHolderId, 1, SchemeYearConstants.Id, date, 100, true)
                };

            var transactions = 
                new List<Transaction>
                {
                // Transfer In
                new Transaction(SchemeYearConstants.Id, Guid.NewGuid(), _existingOrganisationId, 30, Guid.NewGuid(), DateTime.Now),
                // Transfer Out
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, Guid.NewGuid(), 20, Guid.NewGuid(), DateTime.Now),
                // Carry Over
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 10, Guid.NewGuid(), CreditLedgerConstants.TransactionType.CarriedOverFromPreviousYear, DateTime.Now),
                // Admin adjustments
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 500, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 380, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                // Redemption
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -480, Guid.NewGuid(), CreditLedgerConstants.TransactionType.Redeemed, DateTime.Now),
                // Carry over
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -120, Guid.NewGuid(), CreditLedgerConstants.TransactionType.CarriedOverToNextYear, DateTime.Now)
                };

            var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var summary = _installationCreditCalculator.GenerateCreditLedgerSummary(_existingOrganisationId, installationCredits, transactions);

            // Assert
            Assert.Equal(100, summary.CreditsGeneratedByHeatPumps);
            Assert.Equal(200, summary.CreditsGeneratedByHybridHeatPumps);
            Assert.Equal(30-20, summary.CreditsTransferred);
            Assert.Equal(10, summary.CreditsBoughtForward);
            Assert.Equal(2, summary.CreditAmendments.Count);
            Assert.Equal(500, summary.CreditAmendments.First().Credits);
            Assert.Equal(380, summary.CreditAmendments.Last().Credits);

            Assert.Equal(1200, summary.CreditBalance);

            Assert.Equal(-480, summary.CreditsRedeemed);

            Assert.Equal(-120, summary.CreditsCarriedForward);
            Assert.Equal(-600, summary.CreditsExpired);
        }

        [Fact]
        public async Task ShouldReturnCorrectResults_2_When_Ok()
        {

            // Arrange
            var installationCredits =
                new List<InstallationCredit>
                {
                };

            var transactions =
                new List<Transaction>
                {
                // Transfer Out
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, Guid.NewGuid(), 50, Guid.NewGuid(), DateTime.Now),
                // Admin adjustments
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 100, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 50, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                // Redemption
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -50, Guid.NewGuid(), CreditLedgerConstants.TransactionType.Redeemed, DateTime.Now),
                // Carry over
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -10, Guid.NewGuid(), CreditLedgerConstants.TransactionType.CarriedOverToNextYear, DateTime.Now)
                };

            var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

            // Act
            var summary = _installationCreditCalculator.GenerateCreditLedgerSummary(_existingOrganisationId, installationCredits, transactions);

            // Assert
            Assert.Equal(0, summary.CreditsGeneratedByHeatPumps);
            Assert.Equal(0, summary.CreditsGeneratedByHybridHeatPumps);
            Assert.Equal(0-50, summary.CreditsTransferred);
            Assert.Equal(0, summary.CreditsBoughtForward);
            Assert.Equal(2, summary.CreditAmendments.Count);
            Assert.Equal(100, summary.CreditAmendments.First().Credits);
            Assert.Equal(50, summary.CreditAmendments.Last().Credits);

            Assert.Equal(100, summary.CreditBalance);

            Assert.Equal(-50, summary.CreditsRedeemed);

            Assert.Equal(-10, summary.CreditsCarriedForward);
            Assert.Equal(-40, summary.CreditsExpired);
        }
    }
}
