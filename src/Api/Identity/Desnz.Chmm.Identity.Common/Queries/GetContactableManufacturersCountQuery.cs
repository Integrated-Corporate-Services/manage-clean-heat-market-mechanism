using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetContactableManufacturersCountQuery : IRequest<ActionResult<int>>
    {
        public GetContactableManufacturersCountQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public Guid OrganisationId { get; }
    }
}
