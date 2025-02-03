namespace Desnz.Chmm.Identity.Common.Dtos
{
    /// <summary>
    /// Organisation address Dto
    /// </summary>
    public class OrganisationAddressDto
    {
        /// <summary>
        /// Address line one
        /// </summary>
        public string LineOne { get; set; }

        /// <summary>
        /// Address line two (optional)
        /// </summary>
        public string? LineTwo { get; set; }

        /// <summary>
        /// Town or city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// County (optional)
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// Post code
        /// </summary>
        public string Postcode { get; set; }

        public bool? IsUsedAsLegalCorrespondence { get; set; }
    }
}
