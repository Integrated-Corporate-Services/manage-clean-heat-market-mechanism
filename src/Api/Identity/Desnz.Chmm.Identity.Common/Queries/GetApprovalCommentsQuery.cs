using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetApprovalCommentsQuery : IRequest<ActionResult<OrganisationApprovalCommentsDto>>
    {
        public Guid OrganisationId { get; private set; }

        public GetApprovalCommentsQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
