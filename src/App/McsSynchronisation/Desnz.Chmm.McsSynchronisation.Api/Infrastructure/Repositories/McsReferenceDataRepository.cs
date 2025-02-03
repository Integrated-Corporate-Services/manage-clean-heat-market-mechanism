using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

/// <summary>
/// A repository for Mcs Reference Data
/// </summary>
public class McsReferenceDataRepository : IMcsReferenceDataRepository
{
    /// <summary>
    /// Database context
    /// </summary>
    protected readonly McsSynchronisationContext _context;

    /// <summary>
    /// Unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork => _context;

    /// <summary>
    /// DI constructor
    /// </summary>
    /// <param name="context">Database context</param>
    public McsReferenceDataRepository(McsSynchronisationContext context)
    {
        _context = context;
    }

    public async Task<IList<AirTypeTechnology>> GetAllAirTypeTechnologies(Expression<Func<AirTypeTechnology, bool>>? condition = null, bool withTracking = false)
        => await QueryAirTypeTechnologies(condition, withTracking).ToListAsync();

    public async Task<IList<AlternativeSystemFuelType>> GetAllAlternativeSystemFuelTypes(Expression<Func<AlternativeSystemFuelType, bool>>? condition = null, bool withTracking = false)
        => await QueryAlternativeSystemFuelTypes(condition, withTracking).ToListAsync();

    public async Task<IList<AlternativeSystemType>> GetAllAlternativeSystemTypes(Expression<Func<AlternativeSystemType, bool>>? condition = null, bool withTracking = false)
        => await QueryAlternativeSystemTypes(condition, withTracking).ToListAsync();

    public async Task<IList<InstallationAge>> GetAllInstallationAges(Expression<Func<InstallationAge, bool>>? condition = null, bool withTracking = false)
        => await QueryInstallationAges(condition, withTracking).ToListAsync();

    public async Task<IList<Manufacturer>> GetAllManufacturers(Expression<Func<Manufacturer, bool>>? condition = null, bool withTracking = false)
        => await QueryManufacturers(condition, withTracking).ToListAsync();

    public async Task<IList<NewBuildOption>> GetAllNewBuildOptions(Expression<Func<NewBuildOption, bool>>? condition = null, bool withTracking = false)
        => await QueryNewBuildOptions(condition, withTracking).ToListAsync();

    public async Task<IList<RenewableSystemDesign>> GetAllRenewableSystemDesigns(Expression<Func<RenewableSystemDesign, bool>>? condition = null, bool withTracking = false)
        => await QueryRenewableSystemDesigns(condition, withTracking).ToListAsync();

    public async Task<IList<TechnologyType>> GetAllTechnologyTypes(Expression<Func<TechnologyType, bool>>? condition = null, bool withTracking = false)
        => await QueryTechnologyType(condition, withTracking).ToListAsync();

    private IQueryable<TechnologyType> QueryTechnologyType(Expression<Func<TechnologyType, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.TechnologyTypes :
            _context.TechnologyTypes.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<RenewableSystemDesign> QueryRenewableSystemDesigns(Expression<Func<RenewableSystemDesign, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.RenewableSystemDesigns :
            _context.RenewableSystemDesigns.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<NewBuildOption> QueryNewBuildOptions(Expression<Func<NewBuildOption, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.NewBuildOptions :
            _context.NewBuildOptions.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<Manufacturer> QueryManufacturers(Expression<Func<Manufacturer, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.Manufacturers :
            _context.Manufacturers.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<InstallationAge> QueryInstallationAges(Expression<Func<InstallationAge, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.InstallationAges :
            _context.InstallationAges.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<AlternativeSystemType> QueryAlternativeSystemTypes(Expression<Func<AlternativeSystemType, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.AlternativeSystemTypes :
            _context.AlternativeSystemTypes.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<AlternativeSystemFuelType> QueryAlternativeSystemFuelTypes(Expression<Func<AlternativeSystemFuelType, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.AlternativeSystemFuelTypes :
            _context.AlternativeSystemFuelTypes.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private IQueryable<AirTypeTechnology> QueryAirTypeTechnologies(Expression<Func<AirTypeTechnology, bool>>? condition = null, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.AirTypeTechnologies :
            _context.AirTypeTechnologies.Where(condition);

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
