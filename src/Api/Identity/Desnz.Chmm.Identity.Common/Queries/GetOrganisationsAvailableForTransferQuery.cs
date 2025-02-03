using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetOrganisationsAvailableForTransferQuery : IRequest<ActionResult<List<ViewOrganisationDto>>>
    {
        public Guid OrganisationId { get; private set; }

        public GetOrganisationsAvailableForTransferQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
