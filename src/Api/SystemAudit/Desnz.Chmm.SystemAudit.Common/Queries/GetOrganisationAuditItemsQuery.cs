using Desnz.Chmm.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.SystemAudit.Common.Queries
{
    public class GetOrganisationAuditItemsQuery : IRequest<ActionResult<List<AuditItemDto>>>
    {
        public Guid OrganisationId { get; private set; }

        public GetOrganisationAuditItemsQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
