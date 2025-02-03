namespace Desnz.Chmm.Identity.Common.Dtos
{
    /// <summary>
    /// Base information required for a licence holder creation event
    /// </summary>
    public class LicenceHolderInformationDto
    {
        /// <summary>
        /// The Id from the MID for the licence holder (Manufacturer)
        /// </summary>
        public int McsManufacturerId { get; set; }

        /// <summary>
        /// The Name for the licence holder from the MID (Manufacturer Name)
        /// </summary>
        public string McsManufacturerName { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(McsManufacturerId, McsManufacturerName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as LicenceHolderInformationDto);
        }

        public bool Equals(LicenceHolderInformationDto other)
        {
            return other is not null &&
                   McsManufacturerId == other.McsManufacturerId &&
                   McsManufacturerName == other.McsManufacturerName;
        }

    }
}
