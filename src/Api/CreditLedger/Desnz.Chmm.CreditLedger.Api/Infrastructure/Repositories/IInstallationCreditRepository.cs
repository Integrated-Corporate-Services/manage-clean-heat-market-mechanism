using Desnz.Chmm.Common;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using System.Linq.Expressions;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories
{
    public interface IInstallationCreditRepository : IRepository
    {
        Task AddCreditTransfer(CreditTransfer creditTransfer, bool saveChanges = true);
        Task Append(IEnumerable<InstallationCredit> installationCredits);

        /// <summary>
        /// Perform a SUM calculation on an organisation's credits
        /// </summary>
        /// <param name="licenceHolderIds">The licence holders to query</param>
        /// <param name="schemeYearId">The Scheme year to query</param>
        /// <returns>The sum total of scheme year credits</returns>
        Task<decimal> SumCredits(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId);

        /// <summary>
        /// Perform a SUM calculation on all licence holders
        /// </summary>
        /// <param name="licenceHolderIds">The licence holders to query</param>
        /// <param name="schemeYearId">The scheme year to sum</param>
        /// <returns>A list of licence holder ids and how many credits they have</returns>
        Task<List<OrganisationLicenceHolderCreditsDto>> SumCreditsByLicenceHolderAndOrganisation(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId);

        /// <summary>
        /// Get installation credits
        /// </summary>
        /// <param name="licenceHolderIds">Licence holders to query</param>
        /// <param name="schemeYearId">Scheme year to query</param>
        /// <returns>Installation credits</returns>
        Task<IList<InstallationCredit>> GetInstallationCredits(IEnumerable<LicenceOwnershipDto> licenceHolderIds, Guid schemeYearId);
        
        /// <summary>
        /// Get installation credits, generated within a given period
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<IList<InstallationCredit>> Get(DateOnly startDate, DateOnly endDate);
        Task<List<InstallationCredit>> GetAll(Expression<Func<InstallationCredit, bool>>? condition = null, bool withTracking = false);
    }
}