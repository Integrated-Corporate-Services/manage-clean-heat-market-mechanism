using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetRejectionCommentsQuery : IRequest<ActionResult<OrganisationRejectionCommentsDto>>
    {
        public Guid OrganisationId { get; private set; }

        public GetRejectionCommentsQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
