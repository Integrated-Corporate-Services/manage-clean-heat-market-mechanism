using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

public class HeatPumpInstallationsRepository : IHeatPumpInstallationsRepository
{
    private readonly McsSynchronisationContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public HeatPumpInstallationsRepository(McsSynchronisationContext context)
    {
        _context = context;
    }

    public async Task<HeatPumpInstallation?> Get(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).SingleOrDefaultAsync();


    public async Task<List<HeatPumpInstallation>> GetAll(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool withTracking = false)
        => await Query(condition, withTracking).ToListAsync();

    private IQueryable<HeatPumpInstallation> Query(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool withTracking = false)
    {
        var query = condition == null ?
            _context.HeatPumpInstallations :
            _context.HeatPumpInstallations.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query.Include(x => x.HeatPumpProducts);
    }

    public async Task<Guid> Create(HeatPumpInstallation installation, bool saveChanges = true)
    {
        await _context.HeatPumpInstallations.AddAsync(installation);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return installation.Id;
    }

    public async Task Create(IEnumerable<HeatPumpInstallation> installations, bool saveChanges = true)
    {
        await _context.HeatPumpInstallations.AddRangeAsync(installations);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }

}
