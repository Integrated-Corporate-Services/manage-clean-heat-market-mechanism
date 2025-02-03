using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetLicenceHoldersAllLinksQueryHandler : BaseRequestHandler<GetLicenceHoldersAllLinksQuery, ActionResult<List<LicenceHolderLinkDto>>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public GetLicenceHoldersAllLinksQueryHandler(
        ILogger<BaseRequestHandler<GetLicenceHoldersAllLinksQuery, ActionResult<List<LicenceHolderLinkDto>>>> logger,
        IOrganisationsRepository organisationsRepository
        ) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    /// <summary>
    /// Handles the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<List<LicenceHolderLinkDto>>> Handle(GetLicenceHoldersAllLinksQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.GetAll(includeLicenceHolderLinks: true);
        return organisation.SelectMany(i => i.GetOngoingLicenceHolderLinksHistory()).ToList();
    }
}

