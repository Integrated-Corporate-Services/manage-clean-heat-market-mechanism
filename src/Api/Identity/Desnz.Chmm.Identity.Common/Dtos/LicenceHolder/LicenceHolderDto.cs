namespace Desnz.Chmm.Identity.Common.Dtos.LicenceHolder
{
    /// <summary>
    /// DTO for a single licence holder
    /// </summary>
    public class LicenceHolderDto
    {
        /// <summary>
        /// The CHMM Id of the licence holder
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Manufacturer Id from the MID
        /// </summary>
        public int McsManufacturerId { get; set; }

        /// <summary>
        /// The name of the licence holder (Manufacturer Name) from MID
        /// </summary>
        public string Name { get; set; }
    }
}
