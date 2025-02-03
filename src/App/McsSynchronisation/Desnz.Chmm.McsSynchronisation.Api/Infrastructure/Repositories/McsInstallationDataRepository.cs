using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

public class McsInstallationDataRepository : IMcsInstallationDataRepository
{
    protected readonly McsSynchronisationContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public McsInstallationDataRepository(McsSynchronisationContext context)
    {
        _context = context;
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public async Task<InstallationRequest?> GetRequest(DateTime startDate, DateTime endDate)
    => await _context.InstallationRequests.FirstOrDefaultAsync(x => x.StartDate == startDate && x.EndDate == endDate);

    /// <summary>
    /// Get all head pump installations
    /// </summary>
    /// <param name="condition">Linq query</param>
    /// <param name="includeHeatPumpProducts">Should we include heat pump products as well</param>
    /// <param name="withTracking">Track this query?</param>
    /// <returns>All heat pump installs filtered</returns>
    public async Task<IList<HeatPumpInstallation>> GetAll(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool includeHeatPumpProducts = true, bool withTracking = false)
    => await Query(condition, includeHeatPumpProducts, withTracking).OrderByDescending(i => i.CommissioningDate).ToListAsync();

    private IQueryable<HeatPumpInstallation> Query(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool includeHeatPumpProducts = true, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.HeatPumpInstallations :
            _context.HeatPumpInstallations.Where(condition);


        if (includeHeatPumpProducts)
        {
            query = query.Include(u => u.HeatPumpProducts);
        }

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public async Task AddInstallationProducts(IEnumerable<HeatPumpInstallationProduct> installationProducts)
    {
        var dbSet = _context.Set<HeatPumpInstallationProduct>("HeatPumpInstallationProducts");
        await dbSet.AddRangeAsync(installationProducts);
    }

    public async Task AddInstallations(IEnumerable<HeatPumpInstallation> installations)
    {
        await _context.HeatPumpInstallations.AddRangeAsync(installations);
    }

    public async Task AddInstallationRequests(InstallationRequest installationRequest)
    {
        await _context.InstallationRequests.AddAsync(installationRequest);
    }

    public async Task AppendProducts(IEnumerable<HeatPumpProduct> products)
    {
        var existingProducts = _context.HeatPumpProducts.AsNoTracking().AsQueryable();
        var productsToAdd = products.Except(existingProducts);
        await _context.HeatPumpProducts.AddRangeAsync(productsToAdd);
    }

    /// <summary>
    /// Removes a list of installations
    /// </summary>
    /// <param name="installationsToRemove"></param>
    /// <returns></returns>
    public async Task RemoveInstallations(IEnumerable<HeatPumpInstallation> installationsToRemove)
    {
        _context.HeatPumpInstallations.RemoveRange(installationsToRemove);
        await _context.SaveChangesAsync();
    }
}
