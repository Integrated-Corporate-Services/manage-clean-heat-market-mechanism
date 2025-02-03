using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Identity.Api.Entities
{
    /// <summary>
    /// Defines a licence holder for a product
    /// These will be associated to organisations
    /// </summary>
    public class LicenceHolder : Entity
    {
        #region Properties

        /// <summary>
        /// Stores a link back to the Manufacturer Id in the MCS MID
        /// </summary>
        public int McsManufacturerId { get; private set; }

        /// <summary>
        /// The friendly name of the licence holder
        /// This links to the "Manufacturer" field from products
        /// </summary>
        public string Name { get; private set; }

        public IReadOnlyCollection<LicenceHolderLink> LicenceHolderLinks => _licenceHolderLinks;


        #endregion

        #region Private fields

        private List<LicenceHolderLink> _licenceHolderLinks;

        #endregion

        #region Constructors

        /// <summary>
        /// Default EF constructor
        /// </summary>
        protected LicenceHolder() : base() { }

        /// <summary>
        /// Instantiate a new instance of licence holder
        /// </summary>
        /// <param name="manufacturerId">The Manufacturer Id from the MCS MID</param>
        /// <param name="manufacturerName">The Manufacturer name from the MCS MID</param>
        public LicenceHolder(int manufacturerId, string manufacturerName)
        {
            McsManufacturerId = manufacturerId;
            Name = manufacturerName;
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return HashCode.Combine(McsManufacturerId, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as LicenceHolder);
        }

        public bool Equals(LicenceHolder other)
        {
            return other is not null &&
                   McsManufacturerId == other.McsManufacturerId;
        }

        public override string ToString()
        {
            return $"{McsManufacturerId}-{Name}";
        }
        #endregion
        #region Behaviours

        // TODO: Used temporary for unit testing until we replace LicenceHolderLinkRepository Create with EF update via Organisation entity
        public void AddLicenceHolderLink(LicenceHolderLink link)
        {
            _licenceHolderLinks ??= new List<LicenceHolderLink>();
            _licenceHolderLinks.Add(link);
        }

        #endregion
    }
}
