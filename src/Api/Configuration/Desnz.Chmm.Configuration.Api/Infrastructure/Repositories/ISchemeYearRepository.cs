using Desnz.Chmm.Common;
using Desnz.Chmm.Configuration.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Definition for for repository for getting Manufacturer Configuration
    /// </summary>
    public interface ISchemeYearRepository : IRepository
    {
        Task<List<CreditWeighting>> GetAllCreditWeightings();
        Task<ObligationCalculations?> GeObligationCalculations(Guid schemeYearId, bool withTracking = false);
        CreditWeighting? GetCreditWeighting(Expression<Func<CreditWeighting, bool>> condition, bool withTracking = false);
        Task<SchemeYear?> GetSchemeYear(Expression<Func<SchemeYear, bool>> condition, bool includeWeightings = false, bool includeObligationCalculations = false,bool withTracking = false);
        Task<SchemeYear> GetFirstSchemeYearAsync();
        Task<SchemeYear?> GetSchemeYearById(Guid schemeYearId, bool includeWeightings = false, bool includeObligationCalculations = false, bool withTracking = false);
        SchemeYearQuarter? GetSchemeYearQuarter(Expression<Func<SchemeYearQuarter, bool>> condition, bool withTracking = false);
        Task<List<SchemeYear>> GetAllSchemeYears();
        IQueryable<SchemeYear> Query(Expression<Func<SchemeYear, bool>>? condition = null, bool withTracking = false);
        Task<Guid> Create(SchemeYear schemeYear, bool saveChanges = true);
        ObligationCalculations? GetObligationCalculations(Expression<Func<ObligationCalculations, bool>> condition, bool withTracking = false);
        Task RemoveCascade(SchemeYear schemeYear, bool saveChanges = true);
        Task<List<AlternativeSystemFuelTypeWeightingValue>> CreateFuelTypeWeightingValues(List<AlternativeSystemFuelTypeWeightingValue> values, bool saveChanges = true);
        Task<AlternativeSystemFuelTypeWeightingValue> GetAlternativeSystemFuelTypeWeightingValueById(Guid id, bool withTracking = false);
    }
}
