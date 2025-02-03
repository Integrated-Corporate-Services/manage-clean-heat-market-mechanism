using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using FluentAssertions;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Entities
{
    public class TransactionTests
    {
        public TransactionTests() { }

        [Fact]
        public void ShouldCreateTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transactionType = TransactionConstants.TransactionType.AdminAdjustment;
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();
            var value = 10;
            var schemeYearQuarterId = Guid.NewGuid();
            var transactionDate = DateTime.UtcNow;

            // Act
            var result = new Transaction(userId, transactionType, organisationId, schemeYearId, value, transactionDate, schemeYearQuarterId);

            // Assert
            result.UserId.Should().Be(userId);
            result.TransactionType.Should().Be(transactionType);
            result.OrganisationId.Should().Be(organisationId);
            result.DateOfTransaction.Should().NotBe(default, null);
            result.SchemeYearId.Should().Be(schemeYearId);
            result.SchemeYearQuarterId.Should().Be(schemeYearQuarterId);
            result.IsExcluded.Should().Be(false);
            result.Obligation.Should().Be(value);
            result.IsExcluded.Should().BeFalse();
            result.Id.Should().NotBeEmpty();
            result.CreationDate.Should().NotBe(default, null);
            result.CreatedBy.Should().Be("System");
        }

        [Fact]
        public void ShouldThrowException_When_TransactionTypeNotValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transactionType = "Invalid type";
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();
            var value = 10;
            var schemeYearQuarterId = Guid.NewGuid();
            var transactionDate = DateTime.UtcNow;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Transaction(userId, transactionType, organisationId, schemeYearId, value, transactionDate, schemeYearQuarterId));

        }

        [Fact]
        public void ShouldExcludeTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transactionType = TransactionConstants.TransactionType.AdminAdjustment;
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();
            var value = 10;
            var schemeYearQuarterId = Guid.NewGuid();
            var transactionDate = DateTime.UtcNow;

            var transaction = new Transaction(userId, transactionType, organisationId, schemeYearId, value, transactionDate, schemeYearQuarterId);
            transaction.IsExcluded.Should().Be(false);
            
            // Act
            transaction.Exclude();

            // Assert
            transaction.IsExcluded.Should().Be(true);
        }

    }
}
