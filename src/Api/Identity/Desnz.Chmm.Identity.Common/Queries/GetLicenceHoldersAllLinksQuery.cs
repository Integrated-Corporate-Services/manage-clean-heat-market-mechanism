using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetLicenceHoldersAllLinksQuery : IRequest<ActionResult<List<LicenceHolderLinkDto>>>
{
}
