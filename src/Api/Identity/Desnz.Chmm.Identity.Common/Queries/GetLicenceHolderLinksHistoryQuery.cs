using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetLicenceHolderLinksHistoryQuery : IRequest<ActionResult<List<LicenceHolderLinkDto>>>
{
    public Guid OrganisationId { get; private set; }

    public GetLicenceHolderLinksHistoryQuery(Guid organisationId) => OrganisationId = organisationId;
}