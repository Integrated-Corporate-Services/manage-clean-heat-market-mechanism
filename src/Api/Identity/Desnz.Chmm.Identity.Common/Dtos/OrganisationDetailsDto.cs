using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desnz.Chmm.Identity.Common.Dtos
{
    public class OrganisationDetailsDto
    {
        public Guid? Id { get; set; }
        public string? Status { get; set; }
        public bool IsOnBehalfOfGroup { get; set; }

        public ResponsibleUndertakingDto ResponsibleUndertaking { get; set; }

        public OrganisationAddressDto Address { get; set; }

        public OrganisationAddressDto LegalCorrespondenceAddress { get; set; }

        public bool IsFossilFuelBoilerSeller { get; set; }

        public bool IsNonSchemeParticipant { get; set; }

        public HeatPumpsDto HeatPumps { get; set; }

        public ManufacturerUserDto Applicant { get; set; }

        public ManufacturerUserDto ResponsibleOfficer { get; set; }

        public CreditContactDetailsDto CreditContactDetails { get; set; }
    }
}
