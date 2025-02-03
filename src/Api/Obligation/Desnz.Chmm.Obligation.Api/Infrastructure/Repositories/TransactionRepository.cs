using Desnz.Chmm.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Desnz.Chmm.Obligation.Api.Entities;
using static Desnz.Chmm.Common.Constants.RepositoryConstants;
using Desnz.Chmm.Obligation.Api.Constants;

namespace Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;

/// <summary>
/// Repository of Credit Transfers
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ILogger<TransactionRepository> _logger;
    private readonly ObligationContext _context;
    /// <summary>
    /// Unit of Work
    /// </summary>
    public IUnitOfWork UnitOfWork => _context;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="context">The Credit Ledger Context</param>
    /// <param name="logger">Logger</param>
    public TransactionRepository(ObligationContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Return all transactions matching the condition
    /// </summary>
    /// <param name="condition">Filter licence holders</param>
    /// <param name="withTracking">Should track?</param>
    /// <returns>A collection of transactions</returns>
    public async Task<List<Transaction>> GetAll(Expression<Func<Transaction, bool>>? condition = null, SortOrder sortOrder = SortOrder.Descending, bool withTracking = false)
    {
        var query = Query(condition, withTracking);

        if (sortOrder == SortOrder.Descending)
        {
            query = query.OrderByDescending(u => u.DateOfTransaction);
        } else
        {
            query = query.OrderBy(u => u.DateOfTransaction);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get a single transaction
    /// </summary>
    /// <param name="condition">Licence holder filter</param>
    /// <param name="withTracking">Should track?</param>
    /// <returns>A single transaction</returns>
    public async Task<Transaction?> Get(Expression<Func<Transaction, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).SingleOrDefaultAsync();

    /// <summary>
    /// Create a single transaction
    /// </summary>
    /// <param name="transaction">The transaction to create</param>
    /// <param name="saveChanges">Should save changes?</param>
    /// <returns>The Id of the created entity</returns>
    public async Task<Guid> Create(Transaction transaction, bool saveChanges = true)
    {
        await _context.Transactions.AddAsync(transaction);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return transaction.Id;
    }

    /// <summary>
    /// Create multiple transactions
    /// </summary>
    /// <param name="transactions">The transactions to create</param>
    /// <param name="saveChanges">Should save changes?</param>
    /// <returns>The Id of the created entity</returns>
    public async Task AddTransactions(List<Transaction> transactions, bool saveChanges = true)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }

    private IQueryable<Transaction> Query(Expression<Func<Transaction, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.Transactions :
            _context.Transactions.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public async Task RollbackRedemptionTransactions(Guid schemeYearId, bool saveChanges = true)
    {
        var transactions = Query(i => i.SchemeYearId == schemeYearId && i.TransactionType == TransactionConstants.TransactionType.Redeemed, true);

        _context.Transactions.RemoveRange(transactions);

        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task RollbackCarryForwardObligation(Guid schemeYearId, Guid nextSchemeYearId, bool saveChanges = true)
    {
        var carryForwardTransactions = _context.Transactions.Where(i => i.SchemeYearId == schemeYearId && i.TransactionType == TransactionConstants.TransactionType.CarryForwardToNextYear);
        var broughtForwardTransactions = _context.Transactions.Where(i => i.SchemeYearId == nextSchemeYearId && i.TransactionType == TransactionConstants.TransactionType.BroughtFowardFromPreviousYear);

        _context.Transactions.RemoveRange(carryForwardTransactions);
        _context.Transactions.RemoveRange(broughtForwardTransactions);

        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }

    public Task<Transaction?> GetQuarterTransaction(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool withTracking = false)
        => Get(t => t.OrganisationId == organisationId && t.SchemeYearId == schemeYearId && t.SchemeYearQuarterId == schemeYearQuarterId && t.TransactionType == TransactionConstants.TransactionType.QuarterlySubmission, withTracking);

    public Task<List<Transaction>> GetQuarterTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false)
        => GetAll(t => t.OrganisationId == organisationId && t.SchemeYearId == schemeYearId && t.TransactionType == TransactionConstants.TransactionType.QuarterlySubmission, withTracking: withTracking);

    public Task<Transaction?> GetAnnualTransaction(Guid organisationId, Guid schemeYearId, bool withTracking = false)
        => Get(t => t.OrganisationId == organisationId && t.SchemeYearId == schemeYearId && t.TransactionType == TransactionConstants.TransactionType.AnnualSubmission, withTracking);

    public async Task Remove(List<Transaction> transactions, bool saveChanges = true)
    {
        _context.Transactions.RemoveRange(transactions);

        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }
}
