using Desnz.Chmm.Common;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for installation credits
    /// </summary>
    public class InstallationCreditRepository : IInstallationCreditRepository
    {
        private readonly ILogger<InstallationCreditRepository> _logger;
        private readonly CreditLedgerContext _context;
        /// <summary>
        /// Unit of Work
        /// </summary>
        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The Credit Ledger Context</param>
        /// <param name="logger">Logger</param>
        public InstallationCreditRepository(CreditLedgerContext context, ILogger<InstallationCreditRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Appends a list of InstallationCredit objects to the DB context, 
        /// leaving the existing ones untouched
        /// </summary>
        /// <param name="installationCredits"></param>
        /// <returns></returns>
        public async Task Append(IEnumerable<InstallationCredit> installationCredits)
        {
            var existingCredits = _context.InstallationCredits.AsNoTracking().AsQueryable();
            var creditsToAdd = installationCredits.Except(existingCredits);
            await _context.InstallationCredits.AddRangeAsync(creditsToAdd);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// SUM all credit figures for the given organisation in the given scheme year Id
        /// </summary>
        /// <param name="licenceHolderIds">The Ids of all licence holders to query</param>
        /// <param name="schemeYearId">The scheme year to query</param>
        /// <returns></returns>
        public async Task<decimal> SumCredits(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId)
        {
            var credits = await GetCredits(licenceHolderIds, schemeYearId);
            return credits.Sum(i => i.Value);
        }

        /// <summary>
        /// Perform a SUM calculation on all licence holders
        /// </summary>
        /// <param name="schemeYearId">The scheme year to sum</param>
        /// <returns>A list of licence holder ids and how many credits they have</returns>
        public async Task<List<OrganisationLicenceHolderCreditsDto>> SumCreditsByLicenceHolderAndOrganisation(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId)
        {
            var credits = _context.InstallationCredits.Where(i => i.SchemeYearId == schemeYearId).Select(i => new
            {
                i.LicenceHolderId,
                i.Value,
                i.DateCreditGenerated
            });

            var licenceHolderCredits = new List<OrganisationLicenceHolderCreditsDto>();
            foreach(var licenceholder in licenceHolderIds)
            {
                var organisationCredits = credits.Where(c =>
                    // If they have a start but no end date
                    (c.DateCreditGenerated >= licenceholder.StartDate && licenceholder.EndDate == null && c.LicenceHolderId == licenceholder.LicenceHolderId) ||
                    // If they have both
                    (c.DateCreditGenerated >= licenceholder.StartDate && c.DateCreditGenerated <= licenceholder.EndDate && c.LicenceHolderId == licenceholder.LicenceHolderId)
                );

                licenceHolderCredits.Add(
                    new OrganisationLicenceHolderCreditsDto(
                        licenceholder.LicenceHolderId, 
                        licenceholder.OrganisationId, 
                        organisationCredits.Sum(i => i.Value)
                    )
                );
            }

            return licenceHolderCredits;
        }

        /// <summary>
        /// Add a credit transfer
        /// </summary>
        /// <param name="creditTransfer">The transfer to create</param>
        /// <returns></returns>
        public async Task AddCreditTransfer(CreditTransfer creditTransfer, bool saveChanges = true)
        {
            await _context.CreditTransfers.AddAsync(creditTransfer);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get installation credits
        /// </summary>
        /// <param name="licenceHolderIds">Licence holders to query</param>
        /// <param name="schemeYearId">Scheme year to query</param>
        /// <returns>Installation credits</returns>
        public async Task<IList<InstallationCredit>> GetInstallationCredits(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId)
        {
            var credits = await GetCredits(licenceHolderIds, schemeYearId);
            return credits.ToList();
        }

        /// <summary>
        /// Get installation credits, generated within a given period
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IList<InstallationCredit>> Get(DateOnly startDate, DateOnly endDate)
        {
            return _context.InstallationCredits.Where(x => x.DateCreditGenerated >= startDate && x.DateCreditGenerated <= endDate).ToList();
        }

        private async Task<IEnumerable<InstallationCredit>> GetCredits(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId)
        {
            return licenceHolderIds.SelectMany(i => _context.InstallationCredits.Where(c =>
                c.SchemeYearId == schemeYearId &&
                c.LicenceHolderId == i.LicenceHolderId &&
                c.DateCreditGenerated >= i.StartDate &&
                (i.EndDate == null || c.DateCreditGenerated <= i.EndDate)
            ));
        }


        public async Task<List<InstallationCredit>> GetAll(Expression<Func<InstallationCredit, bool>>? condition = null, bool withTracking = false)
            => await Query(condition, withTracking).ToListAsync();

        private IQueryable<InstallationCredit> Query(Expression<Func<InstallationCredit, bool>>? condition = null, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.InstallationCredits :
                _context.InstallationCredits.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
