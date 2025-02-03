using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;

/// <summary>
/// Repository used to retrieve Quarterly Boiler Sales information
/// </summary>
public class QuarterlyBoilerSalesRepository : IQuarterlyBoilerSalesRepository
{
    private readonly IBoilerSalesContext _context;
    private readonly ILogger<QuarterlyBoilerSalesRepository> _logger;

    /// <summary>
    /// Used to commit changes to the database
    /// </summary>
    public IUnitOfWork UnitOfWork => _context;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="context">Database context</param>
    public QuarterlyBoilerSalesRepository(IBoilerSalesContext context, ILogger<QuarterlyBoilerSalesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<QuarterlyBoilerSales>?> GetAllNonAnnual(Guid schemeYearId)
    {
        var query = _context.QuarterlyBoilerSales.Where(x => x.SchemeYearId == schemeYearId &&
                                                            !_context.AnnualBoilerSales.Select(x => x.OrganisationId).Contains(x.OrganisationId));
        return await query.ToListAsync();
    }

    public async Task<List<QuarterlyBoilerSales>?> Get(Expression<Func<QuarterlyBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false)
    {
        return await Query(condition, includeFiles, includeChanges, withTracking).ToListAsync();
    }

    private IQueryable<QuarterlyBoilerSales> Query(Expression<Func<QuarterlyBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.QuarterlyBoilerSales :
            _context.QuarterlyBoilerSales.Where(condition);

        if (includeFiles)
        {
            query = query.Include(u => u.Files);
        }

        if (includeChanges)
        {
            query = query.Include(u => u.Changes);
        }

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public async Task<Guid> Create(QuarterlyBoilerSales quarterlyBoilerSales, bool saveChanges = true)
    {
        await _context.QuarterlyBoilerSales.AddAsync(quarterlyBoilerSales);
        if (saveChanges)
        {
            _logger.LogInformation("Adding new entity: {entity} to table: {table}", quarterlyBoilerSales, nameof(QuarterlyBoilerSales));
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added new entity: {entity} to table: {table}", quarterlyBoilerSales, nameof(QuarterlyBoilerSales));
        }
        return quarterlyBoilerSales.Id;
    }

    public async Task Update(QuarterlyBoilerSales quarterlyBoilerSales, bool saveChanges = true)
    {
        _context.QuarterlyBoilerSales.Entry(quarterlyBoilerSales).State = EntityState.Modified;
        foreach (var file in quarterlyBoilerSales.Files)
        {
            _context.QuarterlyBoilerSalesFiles.Entry(file).State = EntityState.Added;
        }

        if (saveChanges)
        {
            _logger.LogInformation("Updating existing entity: {entity} to table: {table}", quarterlyBoilerSales, nameof(QuarterlyBoilerSales));
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated existing entity: {entity} to table: {table}", quarterlyBoilerSales, nameof(QuarterlyBoilerSales));
        }
    }
}