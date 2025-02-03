using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Repositories;

public class OrganisationsRepository : IOrganisationsRepository
{
    private readonly IdentityContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public OrganisationsRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task<IList<Organisation>> GetAll(Expression<Func<Organisation, bool>>? condition = null, bool includeLicenceHolderLinks = false, bool withTracking = false)
        => await Query(condition, false, false, includeLicenceHolderLinks, false, withTracking).OrderBy(o => o.Name).ToListAsync();

    public async Task<IList<Organisation>> GetAllActive(bool withTracking = false)
        => await Query(i => i.Status == UsersConstants.Status.Active, false, false, false, false, withTracking).OrderBy(o => o.Name).ToListAsync();

    public async Task<int> Count(Expression<Func<Organisation, bool>>? condition = null)
    {
        var query = (condition == null) ?
            _context.Organisations :
            _context.Organisations.Where(condition);

        return query.Count();
    }

    public async Task<Organisation?> GetById(Guid Id, bool includeAddresses = false, bool includeUsers = false, bool includeLicenceHolderLinks = false, bool withTracking = false)
        => await Query(i => i.Id == Id, includeAddresses, includeUsers, includeLicenceHolderLinks, false, withTracking).FirstOrDefaultAsync();

    public async Task<IOrganisation?> GetByIdForUpdate(Guid Id, bool includeAddresses = false, bool includeUsers = false, bool withTracking = false)
        => await Query(i => i.Id == Id, includeAddresses, includeUsers, false, false, withTracking).FirstOrDefaultAsync();

    public async Task<Organisation?> Get(Expression<Func<Organisation, bool>>? condition = null, List<Guid>? userIds = null, bool withTracking = false)
    {
        IQueryable<Organisation>? query = null;
        if (userIds != null)
        {
            query = Query(condition, true, false, true, false, withTracking)
                .Include(o => o.ChmmUsers.Where(u => userIds.Contains(u.Id)));
        }
        else
        {
            query = Query(condition, true, true, true, false, withTracking);
        }
        return await query.SingleOrDefaultAsync();
    }

    public async Task<Guid> Create(Organisation organisation, bool saveChanges = true)
    {
        await _context.Organisations.AddAsync(organisation);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return organisation.Id;
    }


    public async Task<Guid> CreateAddress(Guid organisationId, OrganisationAddress organisationAddress, bool saveChanges = true)
    {
        organisationAddress.OrganisationId = organisationId;
        _context.OrganisationAddresss.Add(organisationAddress);

        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }

        return organisationAddress.Id;
    }

    private IQueryable<Organisation> Query(Expression<Func<Organisation, bool>>? condition = null, bool includeAddresses = false, bool includeUsers = false, bool includeLicenceHolderLinks = false, bool includeFiles = false, bool withTracking = false)
    {
        var query = (condition == null) ?
            _context.Organisations :
            _context.Organisations.Where(condition);

        if (includeAddresses)
        {
            query = query.Include(u => u.Addresses);
        }

        if (includeUsers)
        {
            query = query.Include(u => u.ChmmUsers);
        }

        if (includeFiles)
        {
            query = query.Include(u => u.OrganisationDecisionFiles);
            query = query.Include(u => u.OrganisationStructureFiles);
        }

        if (includeLicenceHolderLinks)
        {
            query = query.Include(u => u.LicenceHolderLinks).ThenInclude(u => u.LicenceHolder);
        }

        if (!withTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}