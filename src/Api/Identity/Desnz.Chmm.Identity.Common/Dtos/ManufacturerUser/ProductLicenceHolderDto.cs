namespace Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser
{
    public class ProductLicenceHolderDto
    {
        public ProductLicenceHolderDto(Guid id, int manufacturerId)
        {
            ManufacturerId = manufacturerId;
            Id = id;
        }

        public int ManufacturerId { get; private set; }
        public Guid Id { get; private set; }
    }
}
