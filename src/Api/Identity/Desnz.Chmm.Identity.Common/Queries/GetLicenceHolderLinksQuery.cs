using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Get all of the linked licence holders for a specific organisation
/// </summary>
public class GetLicenceHolderLinksQuery : IRequest<ActionResult<List<LicenceHolderLinkDto>>>
{
    public Guid OrganisationId { get; private set; }

    public GetLicenceHolderLinksQuery(Guid organisationId) => OrganisationId = organisationId;
}
