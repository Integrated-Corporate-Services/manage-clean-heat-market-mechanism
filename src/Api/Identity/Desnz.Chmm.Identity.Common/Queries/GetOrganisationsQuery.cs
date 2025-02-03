using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetOrganisationsQuery : IRequest<ActionResult<List<ViewOrganisationDto>>>
{
}
