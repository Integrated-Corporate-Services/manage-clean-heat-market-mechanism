using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;

namespace Desnz.Chmm.Identity.Api.Entities
{
    /// <summary>
    /// Organisation address
    /// </summary>
    public class OrganisationAddress : Entity
    {
        #region Properties
        /// <summary>
        /// Address type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Address line one
        /// </summary>
        public string LineOne { get; private set; }

        /// <summary>
        /// Address line two (optional)
        /// </summary>
        public string? LineTwo { get; private set; }

        /// <summary>
        /// Town or city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// County (optional)
        /// </summary>
        public string? County { get; private set; }

        /// <summary>
        /// Post code
        /// </summary>
        public string PostCode { get; private set; }

        /// <summary>
        /// Id of organisation address belongs to
        /// </summary>
        public Guid OrganisationId { get; set; }

        public Organisation Organisation { get; private set; }
        #endregion

        #region Constructors
        protected OrganisationAddress() : base()
        {
        }

        public OrganisationAddress(CreateOrganisationAddressDto address, string? createdBy = null) : base(createdBy)
        {
            SetOrganisationAddressDetails(address);
        }
        #endregion

        public void SetOrganisationAddressDetails(CreateOrganisationAddressDto address)
        {
            Type = address.IsUsedAsLegalCorrespondence ? OrganisationAddressConstants.Type.LegalCorrespondenceAddress : OrganisationAddressConstants.Type.OfficeAddress;
            LineOne = address.LineOne;
            LineTwo = address.LineTwo;
            City = address.City;
            County = address.County;
            PostCode = address.Postcode;
        }

        internal void UpdateDetails(string lineOne, string? lineTwo, string city, string? county, string postcode)
        {
            LineOne = lineOne;
            LineTwo = lineTwo;
            City = city;
            County = county;
            PostCode = postcode;
        }
    }
}
