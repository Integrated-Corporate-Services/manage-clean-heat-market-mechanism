using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;

/// <summary>
/// Repository used to retrieve Boiler Sales information
/// </summary>
public class AnnualBoilerSalesRepository : IAnnualBoilerSalesRepository
{
    private readonly IBoilerSalesContext _context;
    private readonly ILogger<AnnualBoilerSalesRepository> _logger;

    /// <summary>
    /// Used to commit changes to the database
    /// </summary>
    public IUnitOfWork UnitOfWork => _context;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger</param>
    public AnnualBoilerSalesRepository(IBoilerSalesContext context, ILogger<AnnualBoilerSalesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AnnualBoilerSales?> Get(Expression<Func<AnnualBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false)
    {
        IQueryable<AnnualBoilerSales>? query = Query(condition, includeFiles, includeChanges, withTracking);

        return await query.SingleOrDefaultAsync();
    }


    public async Task<List<AnnualBoilerSales>> GetAll(Expression<Func<AnnualBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false)
    {
        IQueryable<AnnualBoilerSales>? query = Query(condition, includeFiles, includeChanges, withTracking);

        return await query.ToListAsync();
    }

    public async Task<Guid> Create(AnnualBoilerSales annualBoilerSales, bool saveChanges = true)
    {
        await _context.AnnualBoilerSales.AddAsync(annualBoilerSales);
        if (saveChanges)
        {
            _logger.LogInformation("Adding new entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added new entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
        }
        return annualBoilerSales.Id;
    }

    public async Task Update(AnnualBoilerSales annualBoilerSales, bool saveChanges = true)
    {
        _context.AnnualBoilerSales.Entry(annualBoilerSales).State = EntityState.Modified;
        foreach (var file in annualBoilerSales.Files)
        {
            _context.AnnualBoilerSalesFiles.Entry(file).State = EntityState.Added;
        }

        if (saveChanges)
        {
            _logger.LogInformation("Updating existing entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated existing entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
        }
    }

    public async Task Delete(AnnualBoilerSales annualBoilerSales, bool saveChanges = true)
    {
        _context.AnnualBoilerSales.Remove(annualBoilerSales);
        if (saveChanges)
        {
            _logger.LogInformation("Removing entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully removed entity: {entity} to table: {table}", annualBoilerSales, nameof(AnnualBoilerSales));
        }
    }

    private IQueryable<AnnualBoilerSales> Query(Expression<Func<AnnualBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.AnnualBoilerSales :
            _context.AnnualBoilerSales.Where(condition);

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
}