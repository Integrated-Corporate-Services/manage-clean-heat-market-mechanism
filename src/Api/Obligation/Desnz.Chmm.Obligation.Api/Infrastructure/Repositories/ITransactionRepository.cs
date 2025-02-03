using Desnz.Chmm.Common;
using Desnz.Chmm.Obligation.Api.Entities;
using System.Linq.Expressions;
using static Desnz.Chmm.Common.Constants.RepositoryConstants;

namespace Desnz.Chmm.Obligation.Api.Infrastructure.Repositories
{
    public interface ITransactionRepository : IRepository
    {
        Task AddTransactions(List<Transaction> transactions, bool saveChanges = true);
        Task<Guid> Create(Transaction transaction, bool saveChanges = true);
        Task<Transaction?> Get(Expression<Func<Transaction, bool>>? condition = null, bool withTracking = false);
        Task<List<Transaction>> GetAll(Expression<Func<Transaction, bool>>? condition = null, SortOrder sortOrder = SortOrder.Descending, bool withTracking = false);
        Task Remove(List<Transaction> transactions, bool saveChanges = true);
        Task RollbackCarryForwardObligation(Guid schemeYearId, Guid nextSchemeYearId, bool saveChanges = true);
        Task RollbackRedemptionTransactions(Guid schemeYearId, bool saveChanges = true);
        Task<Transaction?> GetQuarterTransaction(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, bool withTracking = false);
        Task<List<Transaction>> GetQuarterTransactions(Guid organisationId, Guid schemeYearId, bool withTracking = false);
        Task<Transaction?> GetAnnualTransaction(Guid organisationId, Guid schemeYearId, bool withTracking = false);
    }
}