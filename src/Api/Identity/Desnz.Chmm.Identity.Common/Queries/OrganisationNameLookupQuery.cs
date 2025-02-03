using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class OrganisationNameLookupQuery : IRequest<ActionResult<List<OrganisationNameDto>>>
    {
        public List<Guid> Ids { get; private set; }

        public OrganisationNameLookupQuery(List<Guid> ids)
        {
            Ids = ids;
        }
    }
}
