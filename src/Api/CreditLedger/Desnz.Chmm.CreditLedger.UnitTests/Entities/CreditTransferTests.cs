using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Entities
{
    public class CreditTransferTests
    {
        [Fact]
        public void When_Creating_CreditTranfer_Status_Is_Created()
        {
            var transfer = new CreditTransfer(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);

            Assert.Equal(CreditLedgerConstants.CreditTransferStatus.Created, transfer.Status);
        }

        [Fact]
        public void When_Accepting_CreditTransfer_Transactions_Are_Created()
        {
            // Arrange
            var fromOrg = Guid.NewGuid();
            var toOrg = Guid.NewGuid();
            var year = Guid.NewGuid();
            var user = Guid.NewGuid();
            var transfer = new CreditTransfer(fromOrg, toOrg, year, user, 1);

            // Act
            transfer.AcceptTransfer();

            // Assert
            Assert.NotNull(transfer.Transaction);
            Assert.Equal(2, transfer.Transaction.Entries.Count);
            Assert.Equal(-1, transfer.Transaction.Entries.Single(i => i.OrganisationId == fromOrg).Value);
            Assert.Equal(1, transfer.Transaction.Entries.Single(i => i.OrganisationId == toOrg).Value);
            // Make sure the user who created the transaction is copied into the initiated by field
            Assert.Equal(user, transfer.Transaction.InitiatedBy);
            // Ensure date of transaction is when the transfer was created, not just when the transaction was created
            Assert.Equal(transfer.CreationDate, transfer.Transaction.DateOfTransaction);
        }
    }
}
