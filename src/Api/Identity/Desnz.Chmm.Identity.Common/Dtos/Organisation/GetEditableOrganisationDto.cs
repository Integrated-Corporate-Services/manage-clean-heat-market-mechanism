using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;

namespace Desnz.Chmm.Identity.Common.Dtos.Organisation
{
    public class GetEditableOrganisationDto : OrganisationDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public List<EditOrganisationAddressDto> Addresses { get; set; }
        public List<EditManufacturerUserDto> Users { get; set; }
        public string[] OrganisationStructureFileNames { get; set; }
    }
}
