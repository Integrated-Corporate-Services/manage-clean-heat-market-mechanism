using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries;

public class GetActiveOrganisationsQuery : IRequest<ActionResult<List<ViewOrganisationDto>>>
{
}