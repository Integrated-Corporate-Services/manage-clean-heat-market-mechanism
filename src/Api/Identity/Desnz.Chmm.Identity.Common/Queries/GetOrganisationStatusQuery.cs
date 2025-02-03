using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetOrganisationStatusQuery : IRequest<ActionResult<OrganisationStatusDto>>
    {
        public Guid OrganisationId { get; private set; }

        public GetOrganisationStatusQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
