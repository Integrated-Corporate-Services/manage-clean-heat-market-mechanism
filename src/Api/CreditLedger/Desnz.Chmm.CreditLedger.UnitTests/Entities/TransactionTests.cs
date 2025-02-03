using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Constants;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Entities
{
    public class TransactionTests
    {
        [Fact]
        public void When_Creating_Transaction_Entry_Is_Created()
        {
            var value = 100;
            var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), value, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow);

            Assert.Equal(1, transaction.Entries.Count());
            Assert.Equal(value, transaction.Entries.First().Value);
        }

        [Fact]
        public void When_Transfering_Credits_Transactions_Are_Correct_Value()
        {
            var fromOrg = Guid.NewGuid();
            var toOrg = Guid.NewGuid();
            var value = 100;

            var transaction = new Transaction(Guid.NewGuid(), fromOrg, toOrg, value, Guid.NewGuid(), DateTime.Now);

            Assert.Equal(2, transaction.Entries.Count());
            Assert.Equal(value, transaction.Entries.Single(t => t.OrganisationId == toOrg).Value);
            Assert.Equal(-value, transaction.Entries.Single(t => t.OrganisationId == fromOrg).Value);
            Assert.Equal(CreditLedgerConstants.TransactionType.Transfer, transaction.TransactionType);
        }
    }
}
