using Desnz.Chmm.Common;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Desnz.Chmm.CreditLedger.Constants.CreditLedgerConstants;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for transactions
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ILogger<TransactionRepository> _logger;
        private readonly CreditLedgerContext _context;
        /// <summary>
        /// Unit of Work
        /// </summary>
        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The Credit Ledger Context</param>
        /// <param name="logger">Logger</param>
        public TransactionRepository(CreditLedgerContext context, ILogger<TransactionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all transactions that reference the given orgaisation and scheme year
        /// </summary>
        /// <param name="organisationId">The organisation to query</param>
        /// <param name="schemeYearId">The scheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A list of transactions</returns>
        public async Task<IList<Transaction>?> GetTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false)
        {
            var query = _context.Transactions.Where(t => t.SchemeYearId == schemeYearId && t.Entries.Any(i => i.OrganisationId == organisationId));

            // Currently there is no scenario where we wouldn't want to include the entries 
            // included in a transaction
            query = query.Include(u => u.Entries);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        /// <summary>
        /// Get all transactions that reference the scheme year
        /// </summary>
        /// <param name="schemeYearId">The scheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A list of transactions</returns>
        public async Task<IList<Transaction>?> GetTransactions(Guid schemeYearId, bool withTracking = false)
        {
            var query = _context.Transactions.Where(t => t.SchemeYearId == schemeYearId);

            // Currently there is no scenario where we wouldn't want to include the entries 
            // included in a transaction
            query = query.Include(u => u.Entries);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        /// <summary>
        /// Returns all tranfer type transactions for the given organisation in the given scheme year
        /// </summary>
        /// <param name="organisationId">Organisation to query</param>
        /// <param name="schemeYearId">Sheme year to query</param>
        /// <param name="withTracking">Should track?</param>
        public async Task<IList<Transaction>> GetTransferTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false)
        {
            var query = _context.Transactions.Where(t => 
                                                    t.SchemeYearId == schemeYearId &&
                                                    t.TransactionType == TransactionType.Transfer &&
                                                    t.Entries.Any(i => i.OrganisationId == organisationId));

            // Currently there is no scenario where we wouldn't want to include the entries 
            // included in a transaction
            query = query.Include(u => u.Entries);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        /// <summary>
        /// Add a tranaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task AddTransaction(Transaction transaction, bool saveChanges = true)
        {
            await _context.Transactions.AddAsync(transaction);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Add a collection of transactions
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        public async Task AddTransactions(List<Transaction> transactions, bool saveChanges = true)
        {
            await _context.Transactions.AddRangeAsync(transactions);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task RollbackRedemptionTransactions(Guid schemeYearId, bool saveChanges = true)
        {
            var transactions = _context.Transactions.Where(i => i.SchemeYearId == schemeYearId && i.TransactionType == CreditLedgerConstants.TransactionType.Redeemed);

            _context.Transactions.RemoveRange(transactions);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task RollbackCarryForwardCredit(Guid schemeYearId, Guid nextSchemeYearId, bool saveChanges = true)
        {
            var carryForwardTransactions = _context.Transactions.Where(i => i.SchemeYearId == schemeYearId && i.TransactionType == CreditLedgerConstants.TransactionType.CarriedOverToNextYear);
            var broughtForwardTransactions = _context.Transactions.Where(i => i.SchemeYearId == nextSchemeYearId && i.TransactionType == CreditLedgerConstants.TransactionType.CarriedOverFromPreviousYear);

            _context.Transactions.RemoveRange(carryForwardTransactions);
            _context.Transactions.RemoveRange(broughtForwardTransactions);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<Transaction>> GetAllTransactions(Expression<Func<Transaction, bool>> query)
            => await _context.Transactions.Where(query).ToListAsync();
    }
}
