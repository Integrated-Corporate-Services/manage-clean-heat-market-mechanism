using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetContactableManufacturersQuery : IRequest<ActionResult<List<OrganisationContactDetailsDto>>>
    {
        public GetContactableManufacturersQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public Guid OrganisationId { get; }
    }
}
