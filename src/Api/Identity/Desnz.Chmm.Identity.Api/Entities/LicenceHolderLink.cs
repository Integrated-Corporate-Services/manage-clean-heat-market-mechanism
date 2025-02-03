using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Identity.Api.Entities
{
    /// <summary>
    /// Defines a licence holder for a product
    /// These will be associated to organisations
    /// </summary>
    public class LicenceHolderLink : Entity
    {
        #region Properties

        /// <summary>
        /// Date from which the license holder link becomes active for the given manufacturer
        /// </summary>
        public DateOnly StartDate { get; private set; }

        /// <summary>
        /// Date at which the license holder link expires for the given manufacturer
        /// </summary>
        public DateOnly? EndDate { get; private set; }

        /// <summary>
        /// Id of organisation that the licence holder link belongs to
        /// </summary>
        public Guid OrganisationId { get; private set; }

        /// <summary>
        /// Id of licence holder that the licence holder link belongs to
        /// </summary>
        public Guid LicenceHolderId { get; private set; }

        /// <summary>
        /// Organisation that the licence holder link belongs to
        /// </summary>
        public Organisation Organisation { get; private set; }

        /// <summary>
        /// Licence holder that the licence holder link belongs to
        /// </summary>
        public LicenceHolder LicenceHolder { get; private set; }


        #endregion

        #region Constructors

        /// <summary>
        /// Default EF constructor
        /// </summary>
        protected LicenceHolderLink() : base() { }

        /// <summary>
        /// Instantiate a new instance of licence holder link
        /// </summary>
        /// <param name="organisationId">The organisation id</param>
        /// <param name="licenceHolderId">The licence holder Id</param>
        /// <param name="schemeYearStartDate">The scheme year start date</param>
        /// <param name="startDate">The date at which the link between the organisation and licence holder starts</param>
        public LicenceHolderLink(Guid organisationId, Guid licenceHolderId, DateOnly schemeYearStartDate, DateOnly? startDate = null)
        {
            OrganisationId = organisationId;
            LicenceHolderId = licenceHolderId;
            StartDate = startDate ?? schemeYearStartDate;
        }

        // TODO: Used temporary for testing until we replace LicenceHolderLinkRepository Create with EF update via Organisation entity 
        public LicenceHolderLink(LicenceHolder licenceHolder, Guid organisationId, DateOnly schemeYearStartDate, DateOnly? startDate = null)
        {
            OrganisationId = organisationId;
            LicenceHolder = licenceHolder;
            LicenceHolderId = licenceHolder.Id;
            OrganisationId = organisationId;

            StartDate = startDate ?? schemeYearStartDate;
        }

        #endregion

        #region Behaviours

        public void EndLink(DateOnly endDate)
        {
            EndDate = endDate;
        }

        #endregion
    }
}
