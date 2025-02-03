using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;

namespace Desnz.Chmm.Identity.Common.Dtos.Organisation
{
    public class CreateOrganisationDto : OrganisationDto
    {
        public List<CreateOrganisationAddressDto> Addresses { get; set; }
        public List<CreateManufacturerUserDto> Users { get; set; }
    }
}
