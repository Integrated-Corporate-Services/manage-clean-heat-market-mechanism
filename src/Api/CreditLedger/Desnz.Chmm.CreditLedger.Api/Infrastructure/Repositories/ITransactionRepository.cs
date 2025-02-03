using Desnz.Chmm.CreditLedger.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Provides access to all transactions
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Get all transactions matching a query
        /// </summary>
        /// <param name="query">Query to filter transactions</param>
        /// <returns></returns>
        Task<IList<Transaction>> GetAllTransactions(Expression<Func<Transaction, bool>> query);

        /// <summary>
        /// Returns all transactions for the given organisation in the given scheme year
        /// </summary>
        /// <param name="organisationId">Organisation to query</param>
        /// <param name="schemeYearId">Sheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        Task<IList<Transaction>> GetTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false);

        /// <summary>
        /// Returns all tranfer type transactions for the given organisation in the given scheme year
        /// </summary>
        /// <param name="organisationId">Organisation to query</param>
        /// <param name="schemeYearId">Sheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        Task<IList<Transaction>> GetTransferTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false);

        /// <summary>
        /// Returns all transactions for the given scheme year
        /// </summary>
        /// <param name="schemeYearId">Sheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        Task<IList<Transaction>> GetTransactions(Guid schemeYearId, bool withTracking = false);

        /// <summary>
        /// Add a transaction to the ledger
        /// </summary>
        /// <param name="transaction">The transaction to add</param>
        /// <returns>OK</returns>
        Task AddTransaction(Transaction transaction, bool saveChanges = true);

        /// <summary>
        /// Add a collection of transactions to the ledger
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        Task AddTransactions(List<Transaction> transactions, bool saveChanges = true);

        /// <summary>
        /// Remove redemption transactions for the given scheme year
        /// </summary>
        /// <param name="schemeYearId"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        Task RollbackRedemptionTransactions(Guid schemeYearId, bool saveChanges = true);
        Task RollbackCarryForwardCredit(Guid schemeYearId, Guid nextSchemeYearId, bool saveChanges = true);
    }
}