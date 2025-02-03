
namespace Desnz.Chmm.Identity.Common.Dtos.LicenceHolder
{
    public class LicenceHolderLinkDto
    {
        /// <summary>
        /// The CHMM Id of the licence holder link
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The CHMM Id of the licence holder
        /// </summary>
        public Guid LicenceHolderId { get; set; }

        /// <summary>
        /// The name of the licence holder (Manufacturer Name) from MID
        /// </summary>
        public string LicenceHolderName { get; set; }

        /// <summary>
        /// The organisation id licence holder is linked to
        /// </summary>
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// The organisation name licence holder is linked to
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        /// Date from which the license holder link becomes active for the given manufacturer
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Date at which the license holder link expires for the given manufacturer
        /// </summary>
        public DateOnly? EndDate { get; set; }
    }
}
