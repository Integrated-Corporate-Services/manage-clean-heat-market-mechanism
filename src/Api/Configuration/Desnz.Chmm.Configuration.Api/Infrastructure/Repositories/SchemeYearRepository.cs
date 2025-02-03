using Desnz.Chmm.Common;
using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.Configuration.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for scheme year Configuration
    /// </summary>
    public class SchemeYearRepository : ISchemeYearRepository
    {
        private readonly ILogger<SchemeYearRepository> _logger;
        private readonly IConfigurationContext _context;

        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// Constructor taking all args
        /// </summary>
        /// <param name="context">The DBContext for the repository to use</param>
        /// <param name="logger">Logger</param>
        public SchemeYearRepository(IConfigurationContext context, ILogger<SchemeYearRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ObligationCalculations? GetObligationCalculations(Expression<Func<ObligationCalculations, bool>> condition, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.ObligationCalculations:
                _context.ObligationCalculations.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.SingleOrDefault();
        }

        /// <summary>
        /// Return all scheme year data
        /// </summary>
        /// <returns></returns>
        public async Task<List<SchemeYear>> GetAllSchemeYears()
            => await Query().OrderBy(u => u.Year).ToListAsync();

        /// <summary>
        /// Returns obligation calculations parameters for the requested scheme year 
        /// </summary>
        /// <returns></returns>
        public async Task<ObligationCalculations?> GeObligationCalculations(Guid schemeYearId, bool withTracking = false)
        {
            return withTracking?
                await _context.ObligationCalculations.FirstOrDefaultAsync(x => x.SchemeYear.Id == schemeYearId):
                await _context.ObligationCalculations.AsNoTracking().FirstOrDefaultAsync(x => x.SchemeYear.Id == schemeYearId);
        }

        /// <summary>
        /// Return all credit weightings
        /// </summary>
        /// <returns></returns>
        public Task<List<CreditWeighting>> GetAllCreditWeightings()
        {
            var query = _context.CreditWeightings
                .Include(u => u.AlternativeSystemFuelTypeWeightings)
                .ThenInclude(u => u.AlternativeSystemFuelTypeWeightingValue)
                .Include(u => u.HeatPumpTechnologyTypeWeightings);

            return query.ToListAsync();
        }

        /// <summary>
        /// Return a single credit weighting based on the query
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="withTracking"></param>
        /// <returns></returns>
        public CreditWeighting? GetCreditWeighting(Expression<Func<CreditWeighting, bool>> condition, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.CreditWeightings
                    .Include(u => u.AlternativeSystemFuelTypeWeightings)
                    .ThenInclude(u => u.AlternativeSystemFuelTypeWeightingValue)
                    .Include(u => u.HeatPumpTechnologyTypeWeightings) :
                _context.CreditWeightings
                    .Include(u => u.AlternativeSystemFuelTypeWeightings)
                    .ThenInclude(u => u.AlternativeSystemFuelTypeWeightingValue)
                    .Include(u => u.HeatPumpTechnologyTypeWeightings).Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.SingleOrDefault();
        }

        /// <summary>
        /// Return a single scheme year quarter
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="withTracking"></param>
        /// <returns></returns>
        public SchemeYearQuarter? GetSchemeYearQuarter(Expression<Func<SchemeYearQuarter, bool>> condition, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.SchemeYearQuarters :
                _context.SchemeYearQuarters.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query.SingleOrDefault();
        }

        /// <summary>
        /// Get a single scheme year by its Id.
        /// </summary>
        /// <param name="schemeYearId"></param>
        /// <param name="includeWeightings"></param>
        /// <param name="withTracking"></param>
        /// <returns></returns>
        public Task<SchemeYear?> GetSchemeYearById(Guid schemeYearId, bool includeWeightings = false, bool includeObligationCalculations = false, bool withTracking = false)
            => GetSchemeYear(i => i.Id == schemeYearId, includeWeightings, includeObligationCalculations, withTracking);

        /// <summary>
        /// Get a single scheme year
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="withTracking"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SchemeYear?> GetSchemeYear(Expression<Func<SchemeYear, bool>> condition, bool includeWeightings = false, bool includeObligationCalculations = false, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.SchemeYears.Include(u => u.Quarters) :
                _context.SchemeYears.Include(u => u.Quarters).Where(condition);

            if (includeWeightings)
            {
                query = query
                    .Include(u => u.CreditWeightings)
                        .ThenInclude(u => u.HeatPumpTechnologyTypeWeightings)
                    .Include(u => u.CreditWeightings)
                        .ThenInclude(u => u.AlternativeSystemFuelTypeWeightings)
                        .ThenInclude(u => u.AlternativeSystemFuelTypeWeightingValue);
            }

            if (includeObligationCalculations)
            {
                query = query
                    .Include(u => u.ObligationCalculations);
            }

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get the first scheme year
        /// </summary>
        public async Task<SchemeYear> GetFirstSchemeYearAsync()
        {
            return await _context.SchemeYears.OrderBy(s => s.StartDate).FirstAsync();
        }

        /// <summary>
        /// Queries the scheme year Configuration
        /// </summary>
        /// <param name="condition">A filter condiction to apply to the query</param>
        /// <param name="withTracking">Whether to track changes (execution is faster if no traking is required)</param>
        /// <returns></returns>
        public IQueryable<SchemeYear> Query(Expression<Func<SchemeYear, bool>>? condition = null, bool withTracking = false)
        {
            var query = (condition == null) ?
            _context.SchemeYears.Include(u => u.Quarters) :
            _context.SchemeYears.Include(u => u.Quarters).Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public async Task<Guid> Create(SchemeYear schemeYear, bool saveChanges = true)
        {
            await _context.SchemeYears.AddAsync(schemeYear);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return schemeYear.Id;
        }

        public async Task RemoveCascade(SchemeYear schemeYear, bool saveChanges = true)
        {
            var valueIds = schemeYear.CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(i => i.AlternativeSystemFuelTypeWeightingValue.Id).Distinct().ToList();
            
            _context.SchemeYears.Remove(schemeYear);
            _context.AlternativeSystemFuelTypeWeightingValues.RemoveRange(
                _context.AlternativeSystemFuelTypeWeightingValues.Where(i => valueIds.Contains(i.Id)));

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AlternativeSystemFuelTypeWeightingValue>> CreateFuelTypeWeightingValues(List<AlternativeSystemFuelTypeWeightingValue> values, bool saveChanges = true)
        {
            await _context.AlternativeSystemFuelTypeWeightingValues.AddRangeAsync(values);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

            return _context.AlternativeSystemFuelTypeWeightingValues.Where(v => values.Select(i => i.Id).Contains(v.Id)).ToList();
        }

        public async Task<AlternativeSystemFuelTypeWeightingValue> GetAlternativeSystemFuelTypeWeightingValueById(Guid id, bool withTracking = false)
        {
            var query = _context.AlternativeSystemFuelTypeWeightingValues.Where(i =>  i.Id == id);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync();
        }
    }
}
